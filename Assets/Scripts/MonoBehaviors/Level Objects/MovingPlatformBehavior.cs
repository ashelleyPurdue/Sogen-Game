using UnityEngine;
using System.Collections;

using UnityEditor;

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

    //Events

    void Awake()
    {
        //Calculate the starting and ending positions
        startPos = transform.position;
        endPos = transform.position + relEndPos;

        //Calculate the speed
        speed = Vector3.Distance(startPos, endPos) / movementTime;

        //Start with the target pos being the endPos
        targetPos = endPos;

        //Set the rigidbody's settings
        rigidbody2D.mass = float.MaxValue;
        rigidbody2D.gravityScale = 0f;
        rigidbody2D.isKinematic = true;
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

            //Aim at the target.
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
            Vector2 target2D = new Vector2(targetPos.x, targetPos.y);

            rigidbody2D.velocity = speed * (target2D - pos2D).normalized;

            //Pause and change targets when we reach the target
            timer += Time.deltaTime;
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
