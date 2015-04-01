using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(HealthPoints))]
[RequireComponent(typeof(PlatformCharacterMotor))]

public class BarbarianBehavior : MonoBehaviour
{
    public float visionRadius = 3f;
    
    public float throwSpeed = 10f;
    
    public float spearGrowTime = 0.1f;
    public float aimTime = 0.5f;
    public float waitTime = 0.5f;
    
    private PlayerPlatformBehavior targetPlayer;
    
    private ThrowableBehavior currentSpear;
    
    public enum State {searching, growingSpear, aiming, throwing, waiting};
    private State currentState = State.searching;
    
    private float timer = 0f;
    
    private delegate void StateMethod();
    private Dictionary<State, StateMethod> stateMethods = new Dictionary<State, StateMethod>();
    
    private static Vector3 spearScale = new Vector3(0.5f, 1, 1);
    
    //Events
    
    void Awake()
    {
        //Construct the state methods
        stateMethods.Add(State.searching, WhileSearching);
        stateMethods.Add(State.growingSpear, WhileGrowingSpear);
        stateMethods.Add(State.aiming, WhileAiming);
        stateMethods.Add(State.waiting, WhileWaiting);
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
            currentState = State.waiting;
        }
    }
    
    private void WhileGrowingSpear()
    {
        //Grow the spear
        
        timer += Time.deltaTime;
        
        currentSpear.transform.localScale = Vector3.Lerp(Vector3.zero, spearScale, timer / spearGrowTime);
        
        if (timer >= spearGrowTime)
        {
            currentSpear.transform.localScale = spearScale;
            currentState = State.aiming;
            timer -= spearGrowTime;
        }
    }
    
    private void WhileAiming()
    {
        //Aim
        
        timer += Time.deltaTime;
        
        if (timer >= aimTime)
        {
            ThrowSpear();
            timer -= aimTime;
            currentState = State.waiting;
        }
    }
    
    private void WhileWaiting()
    {
        //Wait, the create another spear to throw.
        
        timer += Time.deltaTime;
        
        if (timer >= waitTime)
        {
            currentState = State.growingSpear;
            timer -= waitTime;
            CreateSpear();
        }
    }
    
    //Misc methods
    
    private void CreateSpear()
    {
        //Throws a spear at the target
        GameObject spear = (GameObject)GameObject.Instantiate(Resources.Load("throwingspear_prefab"));
        spear.transform.position = transform.position + new Vector3(1, 1, 0);
        
        currentSpear = spear.GetComponent<ThrowableBehavior>();
        
        currentSpear.PickUp(transform, new Vector3(0.5f, 0.5f, 0));
    }
    
    private void ThrowSpear()
    {
        //TODO: Use projectile motion equasions to aim.
        
        Vector2 spearPos = Utils.ToVector2(currentSpear.transform.position);
        Vector2 targPos = Utils.ToVector2(targetPlayer.transform.position);
        
        float gravity =  Physics2D.gravity.y * currentSpear.rigidbody2D.gravityScale;
        
        Vector2 velocity;
        
        try
        {
            velocity = PhysicsUtils.AimProjectile2D(spearPos, targPos, throwSpeed, gravity * -1, 0.5f);
        }
        catch (ProjectileCantReachException e)
        {
            //If the projectile can't reach the target, then just throw it as far as you can.
            float angle = Mathf.PI / 4;
            
            velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            velocity *= throwSpeed;
            
            //Make sure it's thrown in the right direction.
            velocity.x *= Mathf.Sign(targPos.x - spearPos.x);
        }
        catch(GuessValueMaxIterationException e)
        {
            //If GuessValue falied, default to 45 degrees and print a warning.
            
            float angle = Mathf.PI / 4;
            
            velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            velocity *= throwSpeed;
            
            //Make sure it's thrown in the right direction.
            velocity.x *= Mathf.Sign(targPos.x - spearPos.x);
            
            Debug.Log("WARNING: GuessValue failed when throwing spear.  Defaulting to 45 degrees.");
        }
        
        currentSpear.Throw(velocity);
        currentSpear = null;
    }
}
