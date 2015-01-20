using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class PlatformCharacterMotor : MonoBehaviour
{

    //Properties

    public Vector2 ControllerInput
    {
        get { return controllerInput;}
        set { controllerInput = value;}
    }

    public bool JumpButton
    {
        get{ return jumpButton;}
        set{ jumpButton = value;}
    }

    //Inspector fields

    public float gravityScale = 1.8f;   //The gravity scale.  This overrides the rigidbody2D's gravity scale.

    public float maxJumpHeight;         //How high it jumps
    public float timeToMaxJumpHeight;   //How long the jump button must be held to reach that height.

    public float maxWalkingSpeed = 4;   //The maximum horizontal speed(relative to the last ground touched).

    public float groundedFriction = 10; //The horizontal deceleration when grounded.

    public float walkingAccel = 1;      //The accelleration due to walking(when grounded).
    public float aerialHorAccel = 1;    //How much character can influence its hspeed when in the air.

    private float magnitude = 0;        //DEBUG: Display the input magnitude
    private float inputSign = 0;        //DEBUG: Display the sign of the x direction.
    public float deltaVelocityDebug = 0;    //DEBUG: Display how much the player's velocity is changing by        

    //Private fields

    private Vector2 lastGroundedVelocity = Vector2.zero;        //The motor's velocity when it was grounded last.
    private Vector2 lastGroundTouchedVelocity = Vector2.zero;   //The velocity of the last ground that was touched
    private Collider2D[] lastGroundTouched;                     //An array storing the ground objects that the character touched when it was grounded last.

    private float groundedCheckDistance = 0.1f;     //How high above the ground it can be while still being considered "grounded".

    private Vector2 controllerInput = Vector2.zero;

    private bool jumpButton = false;
    private bool jumpButtonLastFrame = false;

    private bool attackButton = false;

    private bool canJump = true;
    private bool isJumping = false;
    private float jumpingTime = 0f;     //How long the player has been jumping

    private bool grounded = false;

    //Events
    void Awake()
    {
        //Enforce these settings for the rigidbody
        rigidbody2D.fixedAngle = true;
    }

    void FixedUpdate()
    {
        magnitude = controllerInput.magnitude;  //DEBUG: Display the magnitude in the inspector.

        WalkingControls();
        JumpControls();
        UpdateGrounded();

        //If grounded and not jumping, move the character down until it's touching the ground
        if (IsGrounded() && !isJumping)
        {
            MoveToGround();
        }
    }

    //Misc methods

    private void MoveToGround()
    {
        //Move downwards until the collider is actually touching the ground.

        float yValue = Utils.GuessValue(transform.position.y, ColliderTouchesGround, -0.025f, true);

        Vector3 newPos = transform.position;
        newPos.y = yValue;
        transform.position = newPos;
    }

    private bool ColliderTouchesGround(float yValue)
    {
        //Returns if the collider would be touching the last ground touched at a given y-value.

        bool output = false;

        //Temporarily move the character down to the new y value
        Vector3 oldPos = transform.position;

        Vector3 newPos = oldPos;
        newPos.y = yValue;
        transform.position = newPos;

        //For every collider, check to see if we're touching it.
        foreach (Collider2D c in lastGroundTouched)
        {
            if (collider2D.bounds.Intersects(c.bounds))
            {
                output = true;
                break;
            }
        }

        //Return the character to its original position and return the output.
        transform.position = oldPos;
        return output;
    }

    private void WalkingControls()
    {
        deltaVelocityDebug = 0;     //DEBUG

        //If grounded, walk
        if (IsGrounded())
        {
            //Get the velocity relative to the ground's velocity.
            Vector2 relativeVelocity = rigidbody2D.velocity - lastGroundTouchedVelocity;

            //Accellerate
            float deltaVelocity = walkingAccel * controllerInput.magnitude * Mathf.Sign(controllerInput.x);
            relativeVelocity.x += deltaVelocity * Time.deltaTime;

            //Make sure the relative velocity stays in range.
            relativeVelocity.x = Utils.CapValue(relativeVelocity.x, maxWalkingSpeed, maxWalkingSpeed * -1);
            
            //Apply friction if no direction is being pressed
            if (Mathf.Abs(controllerInput.x) < 0.1f)
            {
                float frict = groundedFriction * Mathf.Sign(relativeVelocity.x);

                if (Mathf.Abs(relativeVelocity.x) - (Mathf.Abs(frict) * Time.deltaTime ) < 0)
                {
                    relativeVelocity.x = 0;
                } else
                {
                    relativeVelocity.x -= frict * Time.deltaTime;
                }
            }

            //Update the velocity
            rigidbody2D.velocity = relativeVelocity + lastGroundTouchedVelocity;
        } else
        {
            //If the player is in the air, allow the player to control their horizontal velocity

            float deltaVelocity = ControllerInput.magnitude * aerialHorAccel * Mathf.Sign(ControllerInput.x);
            deltaVelocityDebug = deltaVelocity;     //DEBUG

            Vector2 newVel = rigidbody2D.velocity;
            newVel.x = rigidbody2D.velocity.x + deltaVelocity * Time.deltaTime;

            //Cap the horizontal velocity

            float min = lastGroundTouchedVelocity.x - maxWalkingSpeed;
            float max = lastGroundTouchedVelocity.x + maxWalkingSpeed;

            newVel.x = Utils.CapValue(newVel.x, max, min);

            //Update the velocity
            rigidbody2D.velocity = newVel;
        }
    }

    private void JumpControls()
    {
        //When the player touches the ground, enable jumping.
        if (IsGrounded())
        {
            //Enable jumping
            canJump = true;
            jumpingTime = 0;
            
            //Disable gravity, so that there isn't any extra friction from the character pushing against the ground.
            rigidbody2D.gravityScale = 0f;
        } else
        {
            rigidbody2D.gravityScale = gravityScale;

            //Only let the player start jumping if they're on the ground
            if (!isJumping)
            {
                canJump = false;
            }
        }

        //If the player can jump, jump.
        if (canJump)
        {
            //If jumping time is up, or the player released the jump button, disable jumping and enable gravity
            if (jumpingTime >= timeToMaxJumpHeight || JumpButtonReleased())
            {
                //Set the vertical velocity to what it was before the player started jumping.
                Vector2 newVelocity = rigidbody2D.velocity;
                newVelocity.y = lastGroundedVelocity.y;
                rigidbody2D.velocity = newVelocity;

                //Stop jumping
                isJumping = false;

                //Disable jumping
                canJump = false;
                jumpingTime = 0;
            }
            else if (jumpButton)
            {
                //Set the vertical velocity
                float velocity = maxJumpHeight / timeToMaxJumpHeight;

                Vector2 newVelocity = rigidbody2D.velocity;
                newVelocity.y = lastGroundedVelocity.y + velocity;
                rigidbody2D.velocity = newVelocity;

                //Note that we are jumping
                isJumping = true;

                //Increment the jump timer
                jumpingTime += Time.deltaTime;
            }
        }

        //Disable gravity while jumping.
        if (isJumping)
        {
            rigidbody2D.gravityScale = 0f;
        }

        //Take remember whether the jump button was pressed this frame or not.
        jumpButtonLastFrame = jumpButton;
    }


    private void UpdateGrounded()
    {
        //Updates stuff related to whether the player is grounded or not.

        Collider2D[] groundHits = GetGroundCollisions();

        //If the player touched any ground, set grounded to true and remember the ground objects we're touching.  Else, set it to false.
        if (groundHits.Length != 0)
        {
            grounded = true;
            lastGroundTouched = groundHits;
        } else
        {
            grounded = false;
        }

        //If we are currently touching ground, update the lastGroundedVelocity
        if (IsGrounded())
        {
            lastGroundedVelocity = rigidbody2D.velocity;
        }

        //Update the last ground touched velocity
        UpdateLastGroundVelocity(groundHits);

    }

    private void UpdateLastGroundVelocity(Collider2D[] groundHits)
    {
        //Updates the velocity of the last ground touched.

        //Find the last "ground" rigidbody touched.
        Rigidbody2D lastRigidbody = null;

        foreach (Collider2D hit in groundHits)
        {
            if (hit != this.collider2D)
            {
                //If this collider has a Rigidbody, set it as the last rigidbody.
                lastRigidbody = hit.GetComponent<Rigidbody2D>();
                if (lastRigidbody != null)
                {
                    break;
                }
            }
        }

        //If we touched a rigidbody, set lastGroundTouchedVelocity to its velocity.  Else, set it to zero.
        if (lastRigidbody != null)
        {
            lastGroundTouchedVelocity = lastRigidbody.velocity;
        } else
        {
            lastGroundTouchedVelocity = Vector2.zero;
        }

    }

    public bool IsGrounded()
    {
        return grounded;
    }

    private Collider2D[] GetGroundCollisions()
    {
        //Returns all "ground" objects the player is touching, if any.

        //Construct the box for the collision check
        float margin = 0.8f;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        //Get the dimensions of the box to use for collision.
        Vector2 boxPos = rigidbody2D.position - new Vector2(boxCollider.size.x * margin / 2, boxCollider.size.y / 2);
        Vector2 boxSize = new Vector2(boxCollider.size.x * margin, groundedCheckDistance * -1);

        /*
        Vector3 boxPos3D = new Vector3(boxPos.x, boxPos.y, 0);
        Vector3 boxSize3D = new Vector3(boxSize.x, boxSize.y, 0);
        Debug.DrawLine(boxPos3D, boxPos3D + boxSize3D);
        */

        //Get all colliders hit by the check
        Collider2D[] hits = Physics2D.OverlapAreaAll(boxPos, boxPos + boxSize);

        //Return only the colliders that don't belong to this object's
        List<Collider2D> groundedHits = new List<Collider2D>();

        foreach (Collider2D c in hits)
        {
            if (c != this.collider2D && !c.isTrigger)
            {
                groundedHits.Add(c);
            }
        }

        return groundedHits.ToArray();
    }

    private bool JumpButtonReleased()
    {
        return (jumpButton == false && jumpButtonLastFrame == true);
    }
}
