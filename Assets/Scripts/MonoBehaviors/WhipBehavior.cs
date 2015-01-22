﻿using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class WhipBehavior : MonoBehaviour
{
    //Inspector variables
    public float whipScale = 1.5f;          //The whip's x-scale when fully extended.
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

    void FixedUpdate()
    { 
        FiniteStateMachine();
    }

    void OnDealDamage()
    {
        EffectManager.Instance.TempPause(0.25f);
    }
 
    void LateUpdate()
    {
        //Damage everything in the queue if we weren't blocked.
        
        Debug.Log(blockedThisFrame);
        
        if (!blockedThisFrame)
        {
            Debug.Log("Damaging.");
            foreach (HealthPoints hp in thingsToHurtThisFrame)
            {
                hp.DealDamage(myDamageSource);
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

        newScale.y = 1;
        if (endAngle > startAngle)
        {
            newScale.y = -1;
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

        spriteRenderer.sprite = swingingFrame;

        transform.localPosition = Vector3.Lerp(initialLocalPos, blockedEndingPos, timer / blockedAnimationTime);
        transform.localScale = Vector3.Lerp(blockedStartScale, Vector3.zero, timer / blockedAnimationTime);

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
        if (otherHealth != null && otherHealth.useDefaultHitDetection && !thingsToHurtThisFrame.Contains(otherHealth))
        {
            thingsToHurtThisFrame.Add(otherHealth);
        }
        
        //Check if the whip is blocked.
        DamageBlocker otherBlocker = other.GetComponent<DamageBlocker>();
        if (otherBlocker!= null)
        {
            bool thisBlocksMe = myDamageSource.CheckIfBlocked(otherBlocker);
            
            if (thisBlocksMe)
            {
                blockedThisFrame = true;
            }
        }
    }
}
