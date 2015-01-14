using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(HealthPoints))]
[RequireComponent(typeof(DamageSource))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class BatBehavior : MonoBehaviour
{
    public enum BatState {sleeping, approaching, pausing, attacking, recovering};
    private BatState currentState = BatState.sleeping;

    private delegate void StateMethod();
    private Dictionary<BatState, StateMethod> stateMethods = null;

    public float visionRange = 5f;
    public float approachSpeed = 1f;
    public float targetDistance = 2f;

    public float pauseTime = 0.5f;
    public float attackTime = 0.5f;
    public float recoverTime = 1f;

    private float timer = 0f;

    private Vector3 velocity;

    private Transform target;

    //Events

    void Awake()
    {
        //Set up the finite state machine
        if (stateMethods == null)
        {
            stateMethods = new Dictionary<BatState, StateMethod>();

            stateMethods.Add(BatState.sleeping, WhileSleeping);
            stateMethods.Add(BatState.approaching, WhileApproaching);
            stateMethods.Add(BatState.pausing, WhilePausing);
            stateMethods.Add(BatState.attacking, WhileAttacking);
            stateMethods.Add(BatState.recovering, WhileRecovering);
        }

        //Make sure the rigidbody is kinematic and that the collider is a trigger and all of that.
        rigidbody2D.isKinematic = true;
        collider2D.isTrigger = true;
    }

    void FixedUpdate()
    {
        stateMethods [currentState]();
    }

    //FSM
    private void WhileSleeping()
    {
        //Start approaching when the player is in range.

        rigidbody2D.velocity = Vector2.zero;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, visionRange);
        foreach (Collider2D c in hits)
        {
            //If it is the player, then start approaching.
            if (c.GetComponent<PlayerPlatformBehavior>() != null)
            {
                currentState = BatState.approaching;
                target = c.transform;
            }
        }
    }

    private void WhileApproaching()
    {
        //Get to a certain distance to the player.  Start pausing when close enough.

        Vector3 diff3D = target.position - transform.position;
        Vector2 diff2D = new Vector2(diff3D.x, diff3D.y);

        if (diff2D.sqrMagnitude > targetDistance * targetDistance)
        {
            rigidbody2D.velocity = approachSpeed * diff2D.normalized;
        } else
        {
            currentState = BatState.pausing;
            timer = 0f;
        }
    }

    private void WhilePausing()
    {
        //Try to maintain a certain distance from the player for a certain amount of time.  Then attack.

        Vector3 diff3D = target.position - transform.position;
        Vector2 diff2D = new Vector2(diff3D.x, diff3D.y);

        Vector2 velocity = diff2D.normalized * approachSpeed;

        //Reverse the velocity if too close to the player.
        if (diff2D.sqrMagnitude < targetDistance * targetDistance)
        {
            velocity *= -1;
        }

        //Update the velocity
        rigidbody2D.velocity = velocity;

        //Attack after a certin amount of time
        timer += Time.deltaTime;
        if (timer >= pauseTime)
        {
            currentState = BatState.attacking;
            timer = 0f;

            float attackSpeed = targetDistance * 2 / attackTime;
            rigidbody2D.velocity = diff2D.normalized * attackSpeed;
        }
    }

    private void WhileAttacking()
    {
        //keep moving in this direction until attack time is over

        timer += Time.deltaTime;
        if (timer >= attackTime)
        {
            timer = 0f;
            rigidbody2D.velocity = Vector2.zero;
            currentState = BatState.recovering;
        }
    }

    private void WhileRecovering()
    {
        //Stand still before approaching again.
        timer += Time.deltaTime;
        if (timer >= recoverTime)
        {
            timer = 0f;
            currentState = BatState.approaching;
        }
    }
}
