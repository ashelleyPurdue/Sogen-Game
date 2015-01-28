using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TagList))]

public class DamageBlocker : MonoBehaviour
{
	//Fields
	
	public List<DamageTags> tagsToBlock = new List<DamageTags>();
	
	public bool isActive = false;
	
	private TagList tagList;
	
	void Awake()
	{
		tagList = transform.GetComponent<TagList>();
		tagList.AddTag("Blocker");
	}
		
	
	//Methods
	
    public void SendBlockEvent()
    {
        //Sends the OnDamageBlocked event.
        
        transform.BroadcastMessage("OnDamageBlocked", SendMessageOptions.DontRequireReceiver);
        transform.SendMessageUpwards("OnDamageBlocked", SendMessageOptions.DontRequireReceiver);
    }
    
	public bool Blocks(DamageTags tag)
	{
		//Returns whether or not this blocker blocks a given damage tag.
		
		bool output = false;
		
		if (isActive)
		{
			output = tagsToBlock.Contains(tag);
		}
		
		return output;
	}
    
	public bool Blocks(DamageSource source)
	{
		//Returns if this blocker blocks a given damage source.
		
		bool output = false;
		
		if (isActive)
		{
			output = ListUtils.ListsOverlap<DamageTags>(tagsToBlock, source.damageTags);
		}
		
		return output;
	}

}
