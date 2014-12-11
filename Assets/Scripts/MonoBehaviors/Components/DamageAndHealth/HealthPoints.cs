using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthPoints : MonoBehaviour
{
	
	public bool useDefaultHitDetection = true;  	//If this is checked, any collision with a damage source that also
												//has this checked will automatically be registered as a hit by this component
												//and will attempt to DealDamage();
												//Uncheck this if you want the game object to take damage in a
												//different way.
	
	public bool useDefaultDeathBehavior = true; 	//If this is checked, then the object will be destroyed upon death.
									//Uncheck this if you want something else to happen upon death.

	public bool hasInfiniteHealth = false;		//If this is checked, then the object will take damage and broadcast events like normal, but it will never run out of health.

	public int maxHealth = 1;
	public int startingHealth = 1;
	
	public List<DamageTags> vulnerableTo; 		//A list of damage sources that this object is vulnerable to.  If this list is empty, invulnerableTo is used instead.
	public List<DamageTags> invulnerableTo; 	//A list of damage sources that this object will ignore.  This list is only used if vulnerableTo is empty.
	public List<DamageSource> invulnerableToObjects; //A list of sepecif damage sources that this object will ignore.

	public float cooldownTime = 1f; //The amount of time the object is invincible for after taking damage.
	
	private bool isCoolingDown = false;
	
	private float cooldownTimer = 0f;

	private float previousHealth = 0f;
	private float currentHealth = 0f;
	
	//Events
	
	void Awake()
	{
	
		//Initialize the vulnerability lists if they haven't already been.
		
		if (vulnerableTo == null)
		{
			vulnerableTo = new List<DamageTags>();
		}
		
		if (invulnerableTo == null)
		{
		
			invulnerableTo = new List<DamageTags>();
		}

		if (invulnerableToObjects == null)
		{
			invulnerableToObjects = new List<DamageSource>();
		}
		
	}
	
	void Start()
	{
		
		currentHealth = startingHealth;
		previousHealth = startingHealth;
	}
	
	void Update()
	{
	
		//Count down until the object can be damaged again.
		if (isCoolingDown == true)
		{
			cooldownTimer += Time.deltaTime;
			
			if (cooldownTimer >= cooldownTime)
			{
				isCoolingDown = false;
				cooldownTimer = 0f;
			}
		}
		
		//Make sure that the object doesn't exceed max health.
		if (currentHealth > maxHealth)
		{
			currentHealth = maxHealth;
		}
		
		//Trigger death events
		if (currentHealth <= 0 && currentHealth != previousHealth)
		{
			
			transform.BroadcastMessage("OnDead", SendMessageOptions.DontRequireReceiver);
			transform.SendMessageUpwards("OnDead", SendMessageOptions.DontRequireReceiver);
			currentHealth = 0;
		}
		
		//Update the previous health
		previousHealth = currentHealth;
		
	}
	
	void OnTriggerStay(Collider other)
	{
	
		if (useDefaultHitDetection)
		{
			CollisionFunction(other.transform);
		}
	
	}
	
	void OnCollisionStay(Collision collision)
	{
	
		if (useDefaultHitDetection)
		{
			CollisionFunction(collision.transform);
		}
		
	}

    void OnTriggerStay2D(Collider2D other)
    {
        
        if (useDefaultHitDetection)
        {
            CollisionFunction(other.transform);
        }
        
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        
        if (useDefaultHitDetection)
        {
            CollisionFunction(collision.transform);
        }
        
    }
	
	void OnDead()
	{
		//Destroy the object if enabled.
		
		if (useDefaultDeathBehavior)
		{
			Destroy(transform.gameObject);
		}
		
	}
	
	//Misc functions
	
	private void CollisionFunction(Transform other)
	{
		//Attempt to take damage from a colliding damage source.
		
		DamageSource otherSource = other.GetComponent<DamageSource>();
		
		if (otherSource != null && otherSource.useDefaultHitDetection)
		{
			DealDamage(otherSource);
		}
		
	}
	
	//Interface
	
	//Vulnerability
	public bool IsVulnerableTo(DamageSource source)
	{
		//Returns whether or not this object is vulnerable to a given damage source.
		bool output = false;

		if (isCoolingDown)
		{
			output = false; //If the object is cooling down, then it is already invulnerable
		}
		else
		{
			//If the vulnerable to list is not empty, then use it.  Else, use the vulnerable to lists
			if (vulnerableTo.Count != 0)
			{
				output = ListUtils.ListsOverlap<DamageTags>(source.damageTags, vulnerableTo);
			}
			else
			{
				bool isSpecificObject = invulnerableToObjects.Contains(source);
				bool damageTagsOverlap = ListUtils.ListsOverlap<DamageTags>(invulnerableTo, source.damageTags);

				output = !(isSpecificObject || damageTagsOverlap);
			}
		}

		return output;
	}
	
	public void AddInvulnerableTo(DamageTags tagToAdd)
	{
		//Adds a damage tag to the invulnerable list
		
		invulnerableTo.Add(tagToAdd);
		
		invulnerableTo = ListUtils.RemoveDuplicateItems<DamageTags>(invulnerableTo);
	}

	public void AddInvulnerableTo(DamageSource sourceToAdd)
	{
		invulnerableToObjects.Add(sourceToAdd);
	}

	public void RemoveInvulnerableTo(DamageTags tagToRemove)
	{
		//Removes a damage tag from the invulnerable list.
		
		invulnerableTo.Remove(tagToRemove);
	}

	public void RemoveInvulnerableTo(DamageSource sourceToRemove)
	{
		//Removes a specific object from the invulnerable to list

		invulnerableToObjects.Remove(sourceToRemove);
	}

	public void AddVulnerableTo(DamageTags tagToAdd)
	{
		//Adds a damage tag to the vulnerable list
		
		vulnerableTo.Add(tagToAdd);
		
		vulnerableTo = ListUtils.RemoveDuplicateItems<DamageTags>(vulnerableTo);
	}

	public void RemoveVulnerableTo(DamageTags tagToRemove)
	{
		//Removes a damage tag from the invulnerable list.
		
		vulnerableTo.Remove(tagToRemove);
	}
	
	
	//Damage
	public void DealDamage(DamageSource source)
	{
	
		if (source.isHot == true)
		{
			
			if (IsVulnerableTo(source) && source.CanHurt(this))
			{
				
				//Subtract the damage from the health(unless infinite health is enabled)
				if (!hasInfiniteHealth)
				{
					currentHealth -= source.damageAmount;
				}
	
				//Broadcast messages for events
				transform.root.BroadcastMessage("OnTakeDamage", source, SendMessageOptions.DontRequireReceiver);
				source.transform.BroadcastMessage("OnDealDamage", this, SendMessageOptions.DontRequireReceiver);
				
				//Start cooling down
				StartCoolingDown();
			}
			else
			{
				
				//Broadcast the OnResistDamage event and the OnResisted event
				
				transform.BroadcastMessage("OnResistDamage", source, SendMessageOptions.DontRequireReceiver);
				source.transform.BroadcastMessage("OnResisted", this, SendMessageOptions.DontRequireReceiver);
				
			}
			
		}
		
	}
	
	public void Heal(float healAmount)
	{
	
		currentHealth += healAmount;
	}
	
	public void StartCoolingDown()
	{
		//Start cooling down.	
		
		cooldownTimer = 0f;
		isCoolingDown = true;
	}
	
	public bool IsCoolingDown()
	{
	
		return isCoolingDown;
	}
	
	public void SetHealth(float newHealth)
	{
		//Sets the health.  If you want to heal or hurt the object, please use DealDamage() or Heal() instead of directly setting the health.
		//Use this for things like remembering the player's health across scenes.
		
		currentHealth = newHealth;
	}
	
	public float GetHealth()
	{
		
		return currentHealth;	
	}
	
}
