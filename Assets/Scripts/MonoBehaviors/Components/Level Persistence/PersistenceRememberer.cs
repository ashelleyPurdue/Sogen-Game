using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistenceRememberer : MonoBehaviour {
	
	private static List<PersistenceRememberer> remembererList = new List<PersistenceRememberer>();
	
	protected float locationHash;
	
	protected int myID;

	private static bool hasAssignedIDs = false;

	private bool awoken = false; //DEBUG;

	public ObjectData data;
	
	//Events
	
	void Awake()
	{
	
		//Find this rememberer's location hash
		locationHash = 1000 * transform.position.x + transform.position.y + .001f * transform.position.z;


		//Log an error if the location hash overlaps with another one.

		foreach (PersistenceRememberer item in remembererList)
		{
			if (item != null)
			{
				if (item.locationHash == this.locationHash)
				{
					
					//Debug.LogError("Persistence rememberers " + Text + " and " + item.Text + " share a location hash of " + locationHash + ".  Please move one of them just a little bit.");
				}
			}
		}
		
		//Add this object to the list of persistence rememberers
		Debug.Log("Adding object with hash " + locationHash + "with ID " + GetInstanceID() + "to list");
		remembererList.Add(this);

		//DEBUG
		if (awoken)
		{
			Debug.Log("Awake is being called twice for the same object");
		}

		awoken = true;
	}
	
	void Start()
	{
		//Assign all persistence rememberers an ID, if it hasn't already been done.
		if (!hasAssignedIDs)
		{
			AssignIDs();
			hasAssignedIDs = true;

			//DEBUG
			DumpRemembererList();
		}

		//Loads the object data, creating it if it doesn't exist.
		data = LevelPersistence.AddObject(myID);

		//If the object already existed(IE: isn't blank), then tell all other components to load their data.
		if (!data.IsBlank())
		{
			transform.BroadcastMessage("LoadObjectData", SendMessageOptions.DontRequireReceiver);
		}
	
	}
	
	void OnLevelEnd()
	{

		//Tell all other components to save their data
		transform.BroadcastMessage("SaveObjectData", SendMessageOptions.DontRequireReceiver);
	
		//Reset hasAssignedIDs to false.
		hasAssignedIDs = false;
		
		//Clear out the rememberer list
		remembererList.Clear();
	
	}

	//Misc functions
	
	//Static functions
	private static void AssignIDs()
	{
		//Assigns IDs to each persistence rememberer based on their location hash

		Debug.Log("Assigning ids");

		//Sort the rememberer list based on the location hashes(that is, if there's even anything to sort)
		if (remembererList.Count != 0)
		{
			remembererList = SortRemembererList(remembererList);
		}
		
		//Give each rememberer an ID based on its location in the list
		for (int i = 0; i < remembererList.Count; i++)
		{
			remembererList[i].myID = i;
		}

	}


	/*private static List<PersistenceRememberer> SortRemembererList(List<PersistenceRememberer> listToSort)
	{
		//DOES NOT WORK.

		//Sort the rememberer list based on the location hashes.	
		
		List<PersistenceRememberer> outputList = new List<PersistenceRememberer>();
		List<PersistenceRememberer> copiedList = new List<PersistenceRememberer>();

		//Check to see if the computer is disobeying my commands.
		Debug.Log("rememberer list count: " + listToSort.Count);

		//Clone the original rememberer list
		foreach (PersistenceRememberer item in listToSort)
		{
			copiedList.Add(item);
		}

		Debug.Log("copied list count: " + copiedList.Count);

		//Sort the list
		for (int i = 0; i < remembererList.Count; i++)
		{
			
			//Move the item with the smallest hash from the copied list to the output list
			float smallestHash = GetSmallestHash(copiedList);
			
			for (int o = 0; o < copiedList.Count; o++)
			{
				
				if (copiedList[o].locationHash == smallestHash)
				{
					
					outputList.Add(copiedList[o]);
					copiedList.RemoveAt(o);
				}
			}
			
		}

		//DEBUG: See if the list is actually sorted
		string message = "hashes in order:\n";
		foreach (PersistenceRememberer r in outputList)
		{
			message += r.locationHash + "\n";
		}
		Debug.Log(message);
		
		return outputList;
	}*/


	private static List<PersistenceRememberer> SortRemembererList(List<PersistenceRememberer> listToSort)
	{
		//Sorts the rememberer list from lowest hash to highest hash

		List<PersistenceRememberer> outputList = new List<PersistenceRememberer>();

		//For each item, place it in the spot where it belongs

		foreach (PersistenceRememberer item in listToSort)
		{

			bool foundBiggerHash = false;

			//Loop through outputList until an item with a hash larger than this one is found.
			for (int i = 0; i < outputList.Count; i++)
			{
				//If a larger hash is found, insert this item into that hash's spot.
				if (outputList[i].locationHash >= item.locationHash)
				{
					foundBiggerHash = true;
					outputList.Insert(i, item);
					break;
				}
			}

			//If no bigger hash was found, then add this one to the end of the list
			if (!foundBiggerHash)
			{
				outputList.Add(item);
			}

		}

		//Return the output lsit
		return outputList;
	}

	private static float GetSmallestHash(List<PersistenceRememberer> listToCheck)
	{
		//Returns the value of the smallest position hash in a given list.

		Debug.Log("list to check count: " + listToCheck.Count);

		float smallestHash = listToCheck[0].locationHash;
		
		foreach (PersistenceRememberer item in listToCheck)
		{
			
			if (item.locationHash < smallestHash)
			{
				smallestHash = item.locationHash;
			}
		}
		
		return smallestHash;
	}

	private static void DumpRemembererList()
	{
		DumpRemembererList(remembererList);
	}

	private static void DumpRemembererList(List<PersistenceRememberer> list)
	{
		//Debug.logs the contents of remembererList

		string message = "Dumping hashes of rememberer list \n";

		foreach (PersistenceRememberer r in list)
		{
			message += r.locationHash + "\n";
		}

		Debug.Log(message);
	}
}