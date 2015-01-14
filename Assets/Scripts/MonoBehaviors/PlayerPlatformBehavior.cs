using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlatformCharacterMotor))]

public class PlayerPlatformBehavior : MonoBehaviour
{
    public WhipBehavior myWhip;

    public float movementDeadzone = 0.5f;   //The player will not move if the analog stick's magnitude is less than this.

    public bool startInCourse = false;  //If checked, the player will start out in the sepcified course
    public string courseToStartIn;

    private PlatformCharacterMotor motor;

    private float mouseX;
    private float mouseY;

    //Events

    void Awake()
    {
        motor = GetComponent<PlatformCharacterMotor>();
    }

    void Update()
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

        //When left clicking, whip in the direction the mouse is pointing
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

        //Do a flip when whipping down and in the air.
        FlipOnWhip();
    }

    void OnDead()
    {
        Debug.Log("Leaving!");
        CourseManager.ReturnToActiveCheckpoint();
    }

    //Misc methods

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

}
