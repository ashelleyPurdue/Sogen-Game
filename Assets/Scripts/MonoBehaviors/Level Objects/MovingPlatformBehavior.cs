using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatformBehavior : MonoBehaviour
{
    public enum State {moving, pausing};

    public Vector3 relEndPos = Vector3.zero;    //The ending position of the platform, relative to the platform's starting position.

    public float movementTime = 1f;             //How long it should take the platform to reach its ending position
    public float pauseTime = 1f;                //How long the platform should pause when it reaches its destination

    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 targetPos;

    private float speed;

    private float timer = 0f;
    private State currentState = State.pausing;

    private float speedDebug = 0f;
    //Events

    void Awake()
    {
        //Calculate the starting and ending positions
        startPos = transform.position;
        endPos = transform.position + relEndPos;

        //Calculate what speed the platform should start moving at so that it reaches the target, accounting for its deceleration
        speed = Mathf.PI * Vector3.Distance(startPos, endPos) / movementTime;

        //Start with the target pos being the endPos
        targetPos = startPos;

        //Set the rigidbody's settings
        rigidbody2D.mass = float.MaxValue;
        rigidbody2D.isKinematic = true;
        rigidbody2D.gravityScale = 0f;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + relEndPos);
    }

    void FixedUpdate()
    {

        if (currentState == State.moving)
        {
            //While moving

            timer += Time.deltaTime;

            //Aim at the target.
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
            Vector2 target2D = new Vector2(targetPos.x, targetPos.y);

            //Smoothly accelerate and decelerate so the player doesn't fly off.
            float currentSpeed = speed * Mathf.Sin((timer / movementTime) * Mathf.PI) / 2;
            speedDebug = currentSpeed;

            rigidbody2D.velocity = currentSpeed * (target2D - pos2D).normalized;

            //Pause and change targets when we reach the target
            if (timer >= movementTime)
            {
                rigidbody2D.position = target2D;
                rigidbody2D.velocity = Vector2.zero;

                timer = 0f;
                currentState = State.pausing;
            }
        } else
        {
            //While pausing

            //Start moving again when timer is up
            timer += Time.deltaTime;

            if (timer >= pauseTime)
            {
                //Change the state
                currentState = State.moving;
                timer = 0f;

                //Change the target
                if (targetPos.Equals(startPos))
                {
                    targetPos = endPos;
                }else
                {
                    targetPos = startPos;
                }
            }
        }

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MovingPlatformBehavior))]
public class MovingPlatformBehaviorEditor : Editor
{
    void OnSceneGUI()
    {
        MovingPlatformBehavior platTarget = (MovingPlatformBehavior)target;  
        Vector3 platPos = platTarget.transform.position;
        
        platTarget.relEndPos = Handles.PositionHandle(platPos + platTarget.relEndPos, Quaternion.identity) - platPos;
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
#endif