using UnityEngine;
using System.Collections;

public class LevelEntranceBehavior : MonoBehaviour
{
	public Transform dropOffPoint;						//The point that the player is moved to when the level starts with this entrance

	public bool useDefaultActivationBehavior = true;	//If checked, this entrance will be used if the player touches it.

	public string targetLevel;
	public int targetEntranceNumber;

	public int entranceNumber;

    private bool playerHasLeft = true;

	//Events

	void OnLevelWasLoaded()
	{
		//Move the player to the dropoff point, if this was the entrance used.

		if (LevelPersistence.entranceUsed == entranceNumber)
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

	//Interface
	public void UseEntrance()
	{
		//Changes to the target level using the target entrance number

		LevelPersistence.entranceUsed = targetEntranceNumber;
		LevelPersistence.ChangeLevel(targetLevel);
	}
}
