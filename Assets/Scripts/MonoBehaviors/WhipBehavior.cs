using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class WhipBehavior : MonoBehaviour
{
    //Inspector variables
    public float swingTime;
    public float hangTime;
    public float damageStartDelay = 0.1f;   //How long after the swinging state starts to enable damaging
    public float damageTimeHanging = 0.1f;  //How long after the hanging state starts to disable damaging

    public float animationAngle = 180;  //The number of degrees the whip turns before cracking.

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

    public enum WhipState {idle, swinging, hanging};
    private WhipState currentState = WhipState.idle;

    private float startAngle;
    private float endAngle;

    private float timer = 0f;

    private SpriteRenderer spriteRenderer;
    private DamageSource myDamageSource;

    //Events

    void Awake()
    {
        //Get the sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        myDamageSource = GetComponent<DamageSource>();
    }

    void Update()
    {
        FiniteStateMachine();
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

        //Set the sprite
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = swingingFrame;

        //Enable the damage source after a delay
        collider2D.enabled = (timer >= damageStartDelay);
        myDamageSource.isHot = (timer >= damageStartDelay);

        //Make the "further out" part of the whip be oriented with the direction it's swinging.
        float scale = 1;
        if (endAngle > startAngle)
        {
            scale = -1;
        }

        transform.localScale = new Vector3(1, scale, 1);

        //Tween the angles
        float angle = Mathf.Lerp(startAngle, endAngle, timer / swingTime);

        //Move to the hanging state
        if (timer >= swingTime)
        {
            currentState = WhipState.hanging;
            timer -= swingTime;

            angle = endAngle;
        }

        //Update the rotation
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void WhileHanging()
    {
        //Hang for a short while, and deal damage for an even shorter while.

        //Increment the timer
        timer += Time.deltaTime;

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

    //Misc methods
}
