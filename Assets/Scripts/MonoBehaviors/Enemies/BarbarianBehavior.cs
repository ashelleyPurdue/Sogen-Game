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
        
        //DEBUG AIM PROJECTILE
        
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
        GameObject spear = (GameObject)GameObject.Instantiate(Resources.Load("throwingspear_prefab"));
        spear.transform.position = transform.position + new Vector3(1, 1, 0);
        
        currentSpear = spear.GetComponent<ThrowableBehavior>();
        
        currentSpear.PickUp(transform, new Vector3(1, 1, 0));
    }
    
    private void ThrowSpear()
    {
        //TODO: Use projectile motion equasions to aim.
        
        Vector2 spearPos = Utils.ToVector2(currentSpear.transform.position);
        Vector2 targPos = Utils.ToVector2(targetPlayer.transform.position);
        
        float gravity =  Physics2D.gravity.y * currentSpear.rigidbody2D.gravityScale;
        
        Vector2 velocity = PhysicsUtils.AimProjectile2D(spearPos, targPos, throwSpeed, gravity * -1);
        
        currentSpear.Throw(velocity);
        currentSpear = null;
    }
}
