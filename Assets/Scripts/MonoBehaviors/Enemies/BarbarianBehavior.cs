using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(HealthPoints))]
[RequireComponent(typeof(PlatformCharacterMotor))]

public class BarbarianBehavior : MonoBehaviour
{
    public float visionRadius = 3f;
    
    public float throwSpeed = 10f;
    public float throwDelay = 1f;
    
    private PlayerPlatformBehavior targetPlayer;
    
    private ThrowableBehavior currentSpear;
    
    public enum State {searching, aiming, throwing};
    private State currentState = State.searching;
    
    private float timer = 0f;
    
    private delegate void StateMethod();
    private Dictionary<State, StateMethod> stateMethods = new Dictionary<State, StateMethod>();
    
    //Events
    
    void Awake()
    {
        //Construct the state methods
        stateMethods.Add(State.searching, WhileSearching);
        stateMethods.Add(State.aiming, WhileAiming);
    }
    
    void Update()
    {
        stateMethods[currentState]();
    }
    
    //State methods
    
    private void WhileSearching()
    {
        //Search for a player.
        bool foundplayer = false;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(Utils.ToVector2(transform.position), visionRadius);
        
        foreach (Collider2D c in hits)
        {
            PlayerPlatformBehavior p = c.GetComponent<PlayerPlatformBehavior>();
            if (p != null)
            {
                foundplayer = true;
                targetPlayer = p;
                break;
            }
        }
        
        //If we found a player, start attacking him.
        if (foundplayer)
        {
            currentState = State.aiming;
        }
    }
    
    private void WhileAiming()
    {
        //Aim
        
        if (currentSpear == null)
        {
            CreateSpear();
        }
        
        timer += Time.deltaTime;
        
        if (timer > throwDelay)
        {
            ThrowSpear();
            timer -= throwDelay;
        }
    }
    
    //Misc methods
    
    private void CreateSpear()
    {
        //Throws a spear at the target
        //TODO: Replace the ball with a spear
        //TODO: Use projectile motion equasions to aim.
        
        GameObject spear = (GameObject)GameObject.Instantiate(Resources.Load("throwingspear_prefab"));
        spear.transform.position = transform.position + new Vector3(1, 1, 0);
        
        currentSpear = spear.GetComponent<ThrowableBehavior>();
        
        currentSpear.PickUp(transform, new Vector3(1, 1, 0));
    }
    
    private void ThrowSpear()
    {
        Vector2 velocity = Utils.ToVector2(targetPlayer.transform.position) - Utils.ToVector2(currentSpear.transform.position);
        velocity.Normalize();
        velocity *= throwSpeed;
        
        currentSpear.Throw(velocity);
        currentSpear = null;
    }
}
