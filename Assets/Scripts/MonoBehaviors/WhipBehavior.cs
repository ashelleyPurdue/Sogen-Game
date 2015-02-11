using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class WhipBehavior : MonoBehaviour
{
    //Inspector variables
    
    public float extendedXscale = 1.5f;         //The whip's x-scale when fully extended.
    public float extendedYscale = 1f;           //The whip's y-scale when fully extended.
    
    public float swingingYScale = 1f;           //The whip's y-scale when swinging.      
    
    public float swingTime;
    public float hangTime;
    public float damageStartDelay = 0.1f;   //How long after the swinging state starts to enable damaging
    public float damageTimeHanging = 0.1f;  //How long after the hanging state starts to disable damaging
 
    public float swingingHitboxScale = 0.75f;   //How much to stretch the hitbox while swinging(but not hanging)
    
    public float blockedAnimationTime = 0.5f;   //How long the whip should be in the "blockedAnimation" state for.

    public float animationAngle = 180;  //The number of degrees the whip turns before cracking.

    public float blockedEndingY = -0.5f;    //The y-value of the whip's position at the end of the blocked animation

    public Sprite swingingFrame;    //The picture of the whip while it's swinging.
    public Sprite hangingFrame;     //The picture of the whip when it is perfectly straight.

    public WhipState CurrentState
    {
        get { return currentState;}
    }

    public float CurrentEndAngle
    {
        get { return endAngle; }
    }

    //Private fields

    private Vector3 initialLocalPos;
 
    private float fullHitboxSize;
    private float fullHitboxCenter;
    
    public enum WhipState {idle, swinging, hanging, blockedAnimation};
    private WhipState currentState = WhipState.idle;

    private float startAngle;
    private float endAngle;

    private float blockedStartAngle;
    private Vector3 blockedEndingPos;
    private Vector3 blockedStartScale;

    private float timer = 0f;

    private SpriteRenderer spriteRenderer;
    private DamageSource myDamageSource;
    private BoxCollider2D myCollider;
    
    private List<HealthPoints> thingsToHurtThisFrame = new List<HealthPoints>();
    private bool blockedThisFrame = false;
    
    //Events

    void Awake()
    {
        //Get the sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        myDamageSource = GetComponent<DamageSource>();
  
        myCollider = GetComponent<BoxCollider2D>();
        
        initialLocalPos = transform.localPosition;
        blockedEndingPos = initialLocalPos;
        blockedEndingPos.y = blockedEndingY;
        
        fullHitboxSize = myCollider.size.x;
        fullHitboxCenter = myCollider.center.x;
        
        //Disable default hit detection
        myDamageSource.useDefaultHitDetection = false;
    }
 
    void Update()
    {
        //Debug: draw the whipstar raycast
        float myAngle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        
        Vector3 myPos = new Vector3(transform.position.x, transform.position.y, 0);
        Vector3 myDir = new Vector3(Mathf.Cos(myAngle), Mathf.Sin(myAngle), 0);
        
        Debug.DrawRay(myPos, myDir);
    }
    
    void FixedUpdate()
    { 
        FiniteStateMachine();
    }

    void OnDealDamage()
    {
        //Pause the game temporarily
        EffectManager.Instance.TempPause(0.1f);
    }
 
    void LateUpdate()
    {
        //Damage everything in the queue if we weren't blocked.
        
        if (!blockedThisFrame)
        {
            foreach (HealthPoints hp in thingsToHurtThisFrame)
            {
                //Deal the damage
                hp.DealDamage(myDamageSource);
                
                //Create the whipstar
                CreateWhipstar(hp);
            }
        }
        
        blockedThisFrame = false;
        thingsToHurtThisFrame.Clear();
    }
    
    void OnBlocked()
    {
        if (timer > 0f)
        {
            //Go into the blocked animation
            transform.localRotation = Quaternion.Euler(0, 0, endAngle);
            blockedStartScale = transform.localScale;
    
            currentState = WhipState.blockedAnimation;
            timer = 0f;
            
            //Clear the list of things to hurt this frame
            thingsToHurtThisFrame.Clear();
            
            //Do a screen effect to show the block
            EffectManager.Instance.TempPause(0.05f);
            spriteRenderer.sprite = hangingFrame;
        }
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
    
    //Interface

    public void StartWhipping(float endAngle)
    {
        //Makes the whip attack such that it lands at the specified angle.

        if (currentState == WhipState.idle)
        {
            this.endAngle = endAngle;

            //Calculate the starting angle
            if (endAngle < 90 && endAngle > -90)
            {
                startAngle = endAngle + animationAngle;
            }
            else
            {
                startAngle = endAngle - animationAngle;
            }

            //Reset the timer and change to the swinging state
            timer = 0f;
            currentState = WhipState.swinging;
        }
    }

    //Finite state machine

    private void FiniteStateMachine()
    {
        //Finite state machine

        switch (currentState)
        {
            case WhipState.idle:
                WhileIdle();
                break;

            case WhipState.swinging:
                WhileSwinging();
                break;

            case WhipState.hanging:
                WhileHanging();
                break;

            case WhipState.blockedAnimation:
                WhileBlockedAnimation();
                break;
        }
    }

    private void WhileIdle()
    {
        //Disable the collider and sprite and damage source
        spriteRenderer.enabled = false;
        collider2D.enabled = false;
        myDamageSource.isHot = false;
    }

    private void WhileSwinging()
    {
        //Increment the timer
        timer += Time.deltaTime;
  
        //Halve the size of the hitbox
        myCollider.size = new Vector2(fullHitboxSize * swingingHitboxScale, myCollider.size.y);
        myCollider.center = new Vector2(fullHitboxCenter - fullHitboxSize * (1 - swingingHitboxScale), myCollider.center.y);
        
        //Set the sprite
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = swingingFrame;

        //Enable the damage source after a delay
        collider2D.enabled = (timer >= damageStartDelay);
        myDamageSource.isHot = (timer >= damageStartDelay);

        //Make the "further out" part of the whip be oriented with the direction it's swinging.
        Vector3 newScale = Vector3.one;

        newScale.y = swingingYScale;
        if (endAngle > startAngle)
        {
            newScale.y = swingingYScale * -1;
        }

        //Make the whip stretch outwards, starting short and ending long.
        newScale.x = Mathf.Pow(Mathf.Lerp(0, Mathf.Sqrt(1.5f), timer / swingTime), 2);

        //Tween the angles
        float angle = Mathf.Lerp(startAngle, endAngle, timer / swingTime);

        //Move to the hanging state
        if (timer >= swingTime)
        {
            currentState = WhipState.hanging;
            timer -= swingTime;

            angle = endAngle;
        }

        //Update the scale.
        transform.localScale = newScale;

        //Update the rotation
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void WhileHanging()
    {
        //Hang for a short while, and deal damage for an even shorter while.

        //Increment the timer
        timer += Time.deltaTime;
  
        //Set the scale
        transform.localScale = new Vector3(extendedXscale, extendedYscale, 1);
        
        //Change the hitbox to full size.
        myCollider.size = new Vector2(fullHitboxSize, myCollider.size.y);
        myCollider.center = new Vector2(fullHitboxCenter, myCollider.center.y);
        
        //Set the sprite
        spriteRenderer.sprite = hangingFrame;

        //Enable the collider and damage source until the "damage period" is over.
        collider2D.enabled = (timer < damageTimeHanging);
        myDamageSource.isHot = (timer < damageTimeHanging);

        //Move to idle
        if (timer >= hangTime)
        {
            timer = 0f;
            currentState = WhipState.idle;
        }
    }

    private void WhileBlockedAnimation()
    {
        //TODO:  Do a short animation of the whip going limp or something.

        //Disable the damage
        myDamageSource.isHot = false;
        collider2D.enabled = false;

        //Do the animation
        timer += Time.deltaTime;
  
        transform.localPosition = Vector3.Lerp(initialLocalPos, blockedEndingPos, timer / blockedAnimationTime);
        transform.localScale = Vector3.Lerp(blockedStartScale, Vector3.zero, timer / blockedAnimationTime);
  
        //Change the sprite
        spriteRenderer.sprite = swingingFrame;
        
        //Move on when time is up
        if (timer >= blockedAnimationTime)
        {
            transform.localPosition = initialLocalPos;
            timer = 0f;
            currentState = WhipState.idle;
        }
      
    }
    
    //Misc methods
    
    private void CollisionFunction(Transform other)
    {
        //If the other object has a HealthPoints and uses default hit detection, add it to the list of things to hurt this frame.
        HealthPoints otherHealth = other.GetComponent<HealthPoints>();
        if (
                otherHealth != null
            
                && otherHealth.useDefaultHitDetection
                && myDamageSource.CanHurt(otherHealth)
                && otherHealth.IsVulnerableTo(myDamageSource)
            
                && !thingsToHurtThisFrame.Contains(otherHealth)
            )
        {
            thingsToHurtThisFrame.Add(otherHealth);
        }
        
        //Check if the whip is blocked.
        DamageBlocker otherBlocker = other.GetComponent<DamageBlocker>();
        if (otherBlocker!= null)
        {
            //Verify the block with a raycast.
            bool collisionVerified = VerifyBlockCollision(otherBlocker);

            
            //If the block was verified, note that we have been blocked this frame.
            bool thisBlocksMe = collisionVerified && myDamageSource.CheckIfBlocked(otherBlocker);
            
            if (thisBlocksMe)
            {
                blockedThisFrame = true;
                otherBlocker.SendBlockEvent();
            }
        }
    }
    
    private bool VerifyBlockCollision(DamageBlocker otherBlocker)
    {
        //Verifies if the whip was actually blocked by doing a proper raycast.
        RaycastHit2D[] hits = Raycast(true);
        
        foreach (RaycastHit2D h in hits)
        {
            if (h.transform.GetComponent<DamageBlocker>() != otherBlocker)
            {
                return true;
            }
        }
        
        return false;
    }
 
    private RaycastHit2D[] Raycast(bool enableTriggers)
    {
        //Performs a raycast from the start of the whip to the tip of the whip
        float length = myCollider.bounds.extents.x * transform.localScale.x * 2;
        float theta = transform.localEulerAngles.z * Mathf.Deg2Rad;
        
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 forward2D = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
  
        //Enable raycasts to hit triggers temporarily
        bool oldTriggerVal = Physics2D.raycastsHitTriggers;
        Physics2D.raycastsHitTriggers = enableTriggers;
        
        //Do the raycast.
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos2D, forward2D, length);
        
        //Revert to the old physics trigger setting
        Physics2D.raycastsHitTriggers = oldTriggerVal;
        
        return hits;
        
    }
    
    private Vector3 FindIntersectionPoint(RaycastHit2D[] hits, Transform other)
    {
        //Finds the position to put the whipstar given an array of hits.
        
        Vector3 output = Vector3.zero;
        
        foreach (RaycastHit2D h in hits)
        {
            if (h.transform == other)
            {
                output = new Vector3(h.point.x, h.point.y, 0);
                break;
            }
        }
        
        return output;
    }
    
    private void CreateWhipstar(HealthPoints other)
    {
        //Creates a whipstar at the point where the whip touches other.

        //Find the intersection point

        RaycastHit2D[] hits = Raycast(true);
        Vector3 intersectionPoint = FindIntersectionPoint(hits, other.transform);
        
        //If we didn't find the intersection point with the first raycast, try again with an easier raycast
        if (intersectionPoint == Vector3.zero)
        {
            Vector2 myPos2D = new Vector2(transform.position.x, transform.position.y);
            Vector2 otherPos2D = new Vector2(other.transform.position.x, other.transform.position.y);
            Vector2 myDir = (otherPos2D - myPos2D).normalized;
            hits = Physics2D.RaycastAll(myPos2D, myDir);
            
            intersectionPoint = FindIntersectionPoint(hits, other.transform);
        }
        
        //Create the whipstar at the intersection point
        EffectManager.Instance.WhipStar(intersectionPoint);
    }
}
