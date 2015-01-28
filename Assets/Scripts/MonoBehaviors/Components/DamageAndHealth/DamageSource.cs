using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(TagList))]

public class DamageSource : MonoBehaviour
{
	
	public bool useDefaultHitDetection = true; //If a damage source AND a HealthPoints both have this checked,
											   //then the damage source will deal damage upon collision.  Uncheck this
											   //if you want it to behave differently.
	public bool isHot = true; //If a damage source is "hot", it means that the damage source is "active", that it can currently deal damage.
	
	public List<DamageTags> damageTags = new List<DamageTags>();
	
	public List<HealthPoints> objectsToIgnore = new List<HealthPoints>();
	
	public List<HealthPoints> objectsToHurt = new List<HealthPoints>();
	
	public float damageAmount = 1f;
	
	//Events
	void Start () 
	{
		
		//Add the "Damage Source" tag to the tag list
		transform.GetComponent<TagList>().AddTag("Damage Source");
		
	}
	
    //Collision events
	void OnTriggerEnter(Collider other)
	{
		CollisionFunction(other.transform);
	}

	void OnTriggerStay(Collider other)
	{
		CollisionFunction(other.transform);
	}

	void OnCollisionStay(Collision collision)
	{
		CollisionFunction(collision.transform);
	}

	void OnCollisionEnter(Collision collision)
	{
		CollisionFunction(collision.transform);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        CollisionFunction(other.transform);
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        CollisionFunction(other.transform);
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        CollisionFunction(collision.transform);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionFunction(collision.transform);
    }
	
	//Interface functions
	
	public void AddDamageTag(DamageTags tagToAdd)
	{
		//Add a damage tag to the list of tags
		
		damageTags.Add(tagToAdd);
		damageTags = ListUtils.RemoveDuplicateItems(damageTags);
		
	}

	[System.Obsolete("Attach the DamageKnockback component to the object you want knocked-back instead!")]
	public Vector3 GetKnockbackForce(Transform sender, float force, float verticalAngle = 0f)
	{
		//Returns how much knockback force should be applied
		
		Vector3 output = (sender.position - transform.position) * force;
		output.y = force * Mathf.Sin(Mathf.Deg2Rad * verticalAngle);
		
		output = output.normalized * force;
		
		return output;
		
	}
	
	public bool CanHurt(HealthPoints objectToCheck)
	{
		//Returns whether or not this damage source can harm an object(ignoring the other object's invulnerabilities)
		
		bool output;
		
		
		//If the ignore list is empty, then use the hurt list.  Else, use the ignore list.  If both are empty, then return true.
		
		if (objectsToIgnore.Count == 0 && objectsToHurt.Count == 0)
		{

			output = true;
		}
		else if (objectsToIgnore.Count == 0)
		{
			
			output = objectsToHurt.Contains(objectToCheck);
		}
		else
		{
			
			output = !objectsToIgnore.Contains(objectToCheck);
		}

		//Return the output
		
		return output;
	}
	
	public void CombineTags(List<DamageTags> listToMergeWith)
	{
		damageTags.AddRange(listToMergeWith);
		damageTags = ListUtils.RemoveDuplicateItems(damageTags);
	}
	
	public void CombineTags(DamageSource damageSourceToMergeWith)
	{
		CombineTags(damageSourceToMergeWith.damageTags);
	}

	public void CombineIgnore(DamageSource damageSourceToMergeWith)
	{
		//Combines the ignore list from another damage source

		List<HealthPoints> otherIgnore = damageSourceToMergeWith.objectsToIgnore;

		objectsToIgnore.AddRange(otherIgnore);

		ListUtils.RemoveDuplicateItems(objectsToIgnore);
	}

	public void CombineHurt(DamageSource damageSourceToMergeWith)
	{
		//Combines the ignore list from another damage source
		
		List<HealthPoints> otherHurt = damageSourceToMergeWith.objectsToHurt;
		
		objectsToIgnore.AddRange(otherHurt);
		
		ListUtils.RemoveDuplicateItems(objectsToHurt);
	}

	//Static functions
	
	public static bool HasDamageTag(List<DamageTags> list, DamageTags damageTagToFind)
	{
		//Returns whether or not this damage source has a particular damage tag on it.
		
		bool output = false;
		
		foreach (DamageTags tag in list)
		{
			
			if (tag == damageTagToFind)
			{
				output = true;
				break;
			}
			
		}
		
		return output;
	}
	
	//Private fucntions

	private void CollisionFunction(Transform other)
	{
		if (useDefaultHitDetection)
        {
            //Broadcast a collision event
            transform.SendMessageUpwards("OnDamageSourceCollision", other, SendMessageOptions.DontRequireReceiver);
			
            if (isHot)
            {
                //Check to see if this damage source was blocked.
                DamageBlocker blocker = other.GetComponent<DamageBlocker>();

                if (blocker != null)
                {
                    CheckIfBlocked(blocker);
                }
            }
        }
	}

	public bool CheckIfBlocked(DamageBlocker blocker)
	{
		//Raises an event if this damage source is blocked by blocker
		
		if (blocker.Blocks(this))
		{
			transform.BroadcastMessage("OnBlocked", SendMessageOptions.DontRequireReceiver);
			transform.SendMessageUpwards("OnBlocked", SendMessageOptions.DontRequireReceiver);
            
            blocker.SendBlockEvent();
            
            return true;
		}
        else
        {
            return false;
        }
	}
}
