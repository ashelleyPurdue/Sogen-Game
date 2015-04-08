using UnityEngine;
using System.Collections;

public class LevelEntranceBehavior : MonoBehaviour
{
	public Transform dropOffPoint;						//The point that the player is moved to when the level starts with this entrance

	public bool useDefaultActivationBehavior = true;	//If checked, this entrance will be used if the player touches it.

	public string targetLevel;
	public string targetEntranceName;

	public string entranceName;
 
    public bool CheckedForDuplicates { get { return checkedForDuplicates; } }
    private bool checkedForDuplicates = false;
    
    private bool playerHasLeft = true;

	//Events
 
    void Start()
    {
        CheckDuplicates();
    }
    
	void OnLevelWasLoaded()
	{
		//Move the player to the dropoff point, if this was the entrance used.

		if (LevelPersistence.entranceUsed.Equals(entranceName))
		{
			Transform player = TagList.FindOnlyObjectWithTag("Player");

            if (player != null)
            {
                //Move the player to the dropoff point
			    player.position = dropOffPoint.position;
			    player.rotation = dropOffPoint.rotation;

                //If the player is touching the entrance after being moved to the dropoff point, set playerHasLeft to false.
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
                Collider2D[] hits = Physics2D.OverlapAreaAll(pos2D, pos2D + GetComponent<BoxCollider2D>().size);

                bool hitPlayer = false;

                foreach (Collider2D hit in hits)
                {
                    if (hit.transform == player)
                    {
                        hitPlayer = true;
                        break;
                    }
                }

                if (hitPlayer)
                {
                    playerHasLeft = false;
                }
            }
            else
            {
                Debug.LogWarning("LevelEntranceBehavior: Tried to move the player" +
                                    "to the entrance, but no transform with the \"Player\"" +
                                    "tag in its TagtList was found."
                                 );
            }
		}
	}

	void OnTriggerEnter(Collider other)
	{
        CollisionFunction(other.transform);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        CollisionFunction(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        PlayerLeaveFunction(other.transform);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerLeaveFunction(other.transform);
    }

    //Misc methods

    private void CollisionFunction(Transform other)
    {
        if (useDefaultActivationBehavior)
        {
            if (TagList.ObjectHasTag(other, "Player") && playerHasLeft)
            {
                UseEntrance();
            }
        }
    }

    private void PlayerLeaveFunction(Transform other)
    {
        //Note that the player has left the collider
        Debug.Log("Player left.");
        if (TagList.ObjectHasTag(other, "Player"))
        {
            playerHasLeft = true;
        }
    }
    
    private void CheckDuplicates()
    {
        //Checks to see if there are any entrances with the same name in this scene, and throws an error if so.
        
        LevelEntranceBehavior[] entrances = GameObject.FindObjectsOfType(typeof(LevelEntranceBehavior)) as LevelEntranceBehavior[];
        
        if (entrances == null)
        {
            return;
        }
        
        //Check all other entrances
        foreach (LevelEntranceBehavior e in entrances)
        {
            if ( (!e.CheckedForDuplicates) && (e != this) && entranceName.Equals(e.entranceName) )
            {
                Debug.Log(entranceName + " == " + e.entranceName);
                
                Debug.LogError("Level entrance name " + entranceName + " is duplicated.\n" +
                    "This id: " + GetInstanceID() +
                    ".  This name: " + name +
                    ".  Original's id: " + e.GetInstanceID() +
                    ".  Original's name: " + e.name);
                
                Application.Quit();
            }
        }
        
        //Mark as checked.
        checkedForDuplicates = true;
    }

	//Interface
	public void UseEntrance()
	{
		//Changes to the target level using the target entrance number

		LevelPersistence.entranceUsed = targetEntranceName;
		LevelPersistence.ChangeLevel(targetLevel);
	}
}
