using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TagList : MonoBehaviour {
	
	public static List<TagList> taggedObjects = new List<TagList>();
	
	public bool debugMode = false; //Check this to enable debugMode, which displays all tags as strings in the inspector.
	
	private bool debugModeWasEnabled = false; //Stores whether or not debug mode was enabled the last frame.
	
	public List<string> tags; //For use in the inspector; everything in this list will be converted to a tagID and added to the real tagList.
	
	private List<int> tagList = new List<int>();
	
	//Events
	void Awake()
	{
		
		//Add all tags assigned in the inspector to the tagList
		
		if (tags != null)
		{
			foreach (string tagString in tags)
			{
				
				AddTag(tagString);
			}
		}
		
		//Delete the temporary string tag list.
		tags = null;
		
		//Add this object to the list of tagged objects.
		taggedObjects.Add(this);
		
	}

	void Update()
	{
		DebugMode();
	}

	void OnDestroy()
	{
	
		//Remove this object from the list of tagged objects
		for (int i = 0; i < taggedObjects.Count; i++)
		{
		
			if (taggedObjects[i] == transform.gameObject)
			{
			
				taggedObjects.RemoveAt(i);
			}
			
		}
		
	}

	void OnLevelEnd()
	{
	
		//When the level ends, clear the tagged object list.
		taggedObjects.Clear ();
	}
	
	//Static functions
	
	public static bool ObjectHasTag(Transform objectToCheck, string tagToFind)
	{
		//Returns if a given object has a tag.  Much cleaner than using GetComponent<>() every time.
		
		TagList list = objectToCheck.GetComponent<TagList>();
		bool output = false;
		
		if (list == null)
		{
			output = false;
		}
		else
		{
			output = list.HasTag(tagToFind);
		}
		
		return output;
		
	}
	
	public static bool ObjectHasTag(GameObject objectToCheck, string tagToFind)
	{
	
		return ObjectHasTag(objectToCheck.transform, tagToFind);
		
	}
	
	public static bool ObjectHasTag(Component objectToCheck, string tagToFind)
	{
	
		return ObjectHasTag(objectToCheck.transform, tagToFind);
	}
	
	public static List<Transform> FindObjectsWithTag(string tagToFind)
	{
		//Returns all objects with a given tag.
		
		List<Transform> outputList = new List<Transform>();
		
		for (int i = 0; i < taggedObjects.Count; i++)
		{

			if (taggedObjects[i] != null)
			{
				if (taggedObjects[i].HasTag(tagToFind))
				{
						
					outputList.Add(taggedObjects[i].transform);
				}
			}
			
		}
		
		return outputList;
	}
	
	public static Transform FindOnlyObjectWithTag(string tagToFind)
	{
		//Returns the only object that has a given tag, or null if none are found.  Throws an error if there are more than one.
		
		List<Transform> objectsFound = FindObjectsWithTag(tagToFind);
		Transform output = null;
		
		if (objectsFound.Count == 1)
		{
			output = objectsFound[0];
		}
		else if (objectsFound.Count > 1)
		{
			throw new TooManyObjectsFoundException(objectsFound.Count);
		}
		
		return output;
	}
	
	
	//Misc functions
	private bool HasTagID(int tagIDToFind)
	{
		//Returns whether or not a tagID can be found in this tagList.
		
		return tagList.Contains(tagIDToFind);
	}
	
	private void DebugMode()
	{
		//When debug mode is enabled, all tags will be shown in the inspector, but more RAM will be used by the game.
		
		//When debug mode is first enabled, sync the string tag list with the real(integer) tagList.
		if (debugMode == true && debugModeWasEnabled == false){
			
			UpdateDebugList();
		}
		
		//When debug mode is disabled, destroy the string tag list to free up memory.  The real(integer) tagList will still exist.
		if (debugMode == false){
			
			tags = null;
		}
		
		//Remember if debug mode was enabled this frame.
		debugModeWasEnabled = debugMode;
		
	}
	
	private void UpdateDebugList()
	{
		//Syncs the string(debug) tag list with the integer(real) tagList.
		//This should only be called in debug mode.
		
		tags = new List<string>();
			
		foreach (int tagID in tagList){
				
			tags.Add(TagDictionary.FindTagName(tagID));
		}
		
	}

	private void DumpTaggedObjects()
	{
		//Creates a bunch of debug.logs, showing the contents of the taggedObjects list.
		Debug.Log("Dumping tagged objects");
		
		foreach (TagList t in taggedObjects)
		{
			Debug.Log(t);
		}

	}

	//Interface
	
	public bool HasTag(string tagToFind)
	{
		//Returns whether the given tag can be found in the list
		
		int tagID = TagDictionary.FindTagID(tagToFind);
		
		return tagList.Contains(tagID);
	}
	
	public void AddTag(string tagToAdd)
	{
		//Add the tagID to the taglist if it isn't already on there.
		
		int tagID = TagDictionary.FindTagID(tagToAdd);
		
		if (!HasTagID(tagID))
		{
		
			tagList.Add(tagID);
		}
		
		//If debug mode is on, add the tag to the string tag list as well.
		if (debugMode)
		{
			
			tags.Add(tagToAdd);
		}
		
	}
	
	public void RemoveTag(string tagToRemove)
	{
	
		int tagID = TagDictionary.FindTagID(tagToRemove);
		
		tagList.Remove(tagID);
		
		//If debug mode is on, remove the tag from the string tag list as well.
		if (debugMode)
		{
			
			tags.Remove(tagToRemove);
		}
		
	}
	
	public void Combine(TagList listToCombineWith)
	{
		//Adds all tags from a given taglist into this one's, skipping any duplicates

		foreach (int tag in listToCombineWith.tagList)
		{
			if (!tagList.Contains(tag))
			{
				tagList.Add(tag);
			}
		}
		
		//If debug mode is on, erase the string tag list and replace it with the new tagList that was formed.
		if (debugMode)
		{
			
			UpdateDebugList();
		}
		
	}
	
	
	//Subclasses
	
	/*
	 * Removed for now.  My idea to "optimize" the tagList was to store the tags as integer IDs and then
	 * look up the tag from the ID.  Didn't work out because if I wanted to add tags in the inspector, I would
	 * have to store them in a string list anyway, essentially rendering this idea useless.  I'm leaving it
	 * in here in case I can find a way to "delete" the temporary string list.
	 *
	 */ 
	private static class TagDictionary
	{
		//Provides methods for converting tag strings to tagIDs.
		
		private static int nextID = 0;
		
		private static Dictionary<int, string> IDToString = new Dictionary<int, string>();
		private static Dictionary<string, int> stringToID = new Dictionary<string, int>();
		
		
		public static string FindTagName(int tagID){
			//Converts a tag number into its string form.
			
			return IDToString[tagID];
		}
		
		public static int FindTagID(string tag){
			//Converts a string tag into its ID, or creates it if it doesn't exists.
			
			if (!stringToID.ContainsKey(tag)){
				CreateTagType(tag);
			}
			
			return stringToID[tag];
		}
		
		private static void CreateTagType(string tag){
			//Adds a new type of tag to the tag dictionary.
			
			IDToString.Add(nextID, tag);
			stringToID.Add(tag, nextID);
			
			nextID++;
		}
	}
	
	public class TooManyObjectsFoundException : System.Exception{
		
		public int numberFound;
			
		public TooManyObjectsFoundException(int num){
			numberFound = num;
		}
			
	}
}
