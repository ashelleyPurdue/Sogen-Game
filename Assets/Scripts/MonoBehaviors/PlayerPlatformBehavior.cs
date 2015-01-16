using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlatformCharacterMotor))]

public class PlayerPlatformBehavior : MonoBehaviour
{
    public enum State {free, climbing};
    private State currentState = State.free;
    private delegate void StateMethod();
    private Dictionary<State, StateMethod> stateMethods = new Dictionary<State, StateMethod>();

    public WhipBehavior myWhip;

    public float movementDeadzone = 0.5f;   //The player will not move if the analog stick's magnitude is less than this.

    public float climbSpeed = 1f;

    public bool startInCourse = false;  //If checked, the player will start out in the sepcified course
    public string courseToStartIn;

    private PlatformCharacterMotor motor;

    private float mouseX;
    private float mouseY;

    //Events

    void Awake()
    {
        motor = GetComponent<PlatformCharacterMotor>();

        //Add the state methods
        stateMethods.Add(State.free, WhileFree);
        stateMethods.Add(State.climbing, WhileClimbing);
    }

    void Update()
    {
        stateMethods[currentState]();
    }

    void OnDead()
    {
        Debug.Log("Leaving!");
        CourseManager.ReturnToActiveCheckpoint();
    }

    //Misc methods

    private void PlatformControls()
    {
        //Relay the controls to the motor
        Vector2 controllerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        if (controllerInput.sqrMagnitude >= movementDeadzone * movementDeadzone)
        {
            motor.ControllerInput = controllerInput;
        } else
        {
            motor.ControllerInput = Vector2.zero;
        }
        
        motor.JumpButton = Input.GetButton("Jump");
    }

    private void WhipControls()
    {
        //When left clicking, whip in the direction the mouse is pointing

        Vector2 controllerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            Vector2 mousePos2D = new Vector2(mousePos3D.x, mousePos3D.y);
            Vector2 whipPos2D = new Vector2(myWhip.transform.position.x, myWhip.transform.position.y);
            
            Vector2 difference = (whipPos2D - mousePos2D) * -1;
            
            float angle = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg);
            
            myWhip.StartWhipping(angle);
        } else if (Input.GetButton("Attack")) //When pressing the whip button, whip in the direction the analog stick is held.
        {
            float angle = Mathf.Atan2(controllerInput.y, controllerInput.x) * Mathf.Rad2Deg;
            myWhip.StartWhipping(angle);
        }
    }

    private void FlipOnWhip()
    {
        //Proceed if the player is whipping in the air
        if (!motor.IsGrounded() && (myWhip.CurrentState == WhipBehavior.WhipState.swinging || myWhip.CurrentState == WhipBehavior.WhipState.hanging))
        {
            float whipAngle = myWhip.CurrentEndAngle * Mathf.Deg2Rad;

            //Proceed if the player is whipping *down*
            if (Mathf.Sin(whipAngle) < -0.5f)
            {
                //Flip the player on his side.
                Vector3 newRot = new Vector3(0, 0, 90 * Mathf.Sign(Mathf.Cos(whipAngle) * -1));
                transform.rotation = Quaternion.Euler(newRot);
            }
        } else
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private void DismountLadder()
    {
        currentState = State.free;
    }

    private bool TouchingLadder()
    {
        //Returns the ladder that the player is touching, if any
        return LadderTouching() != null;
    }

    private LadderBehavior LadderTouching()
    {
        //Returns the ladder that is touching, if any.
        Collider2D[] hits = Physics2D.OverlapAreaAll(collider2D.bounds.max, collider2D.bounds.min);

        foreach (Collider2D c in hits)
        {
            LadderBehavior ladder = c.GetComponent<LadderBehavior>();
            if (ladder != null)
            {
                return ladder;
            }
        }

        return null;
    }

    //State methods

    private void WhileFree()
    {
        motor.enabled = true;

        PlatformControls();
        WhipControls();
        FlipOnWhip();

        //Start climbing when moving up or down on a ladder.
        const float deadzone = 0.3f;
        const float radius = 0.1f;
        if (Mathf.Abs(Input.GetAxis("Vertical")) > deadzone)
        {
            //If there is a ladder nearby, mount it.
            if (TouchingLadder())
            {
                //Mount the ladder
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.gravityScale = 0f;
                motor.enabled = false;

                currentState = State.climbing;
            }
        }
    }

    private void WhileClimbing()
    {
        motor.enabled = false;

        //Allow whipping
        WhipControls();

        //Climb the ladder.
        rigidbody2D.velocity = new Vector2(0, climbSpeed * Input.GetAxis("Vertical"));

        //Dismount if pressing jump or not touching a ladder
        if (Input.GetButtonDown("Jump") || !TouchingLadder())
        {
            DismountLadder();
        }
    }

}
