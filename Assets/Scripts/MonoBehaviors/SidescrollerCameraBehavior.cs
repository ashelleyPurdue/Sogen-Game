using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class SidescrollerCameraBehavior : MonoBehaviour
{

    public float deadzoneLeftBound = - 10;
    public float deadzoneRightBound = -5;

    public bool inDeadzone = false;

    public Transform target;

    private Vector2 relativeTopLeft;
    private Vector2 relativeBottomRight;

    private float lastTargetX;

	void Awake ()
    {
        //If there is no assigned target, search for the player and set him as the target.
        if (target == null)
        {
            target = TagList.FindOnlyObjectWithTag("Player");
        }

        //Move to the player
        if (!TargetInDeadzone(transform.position.x))
        {
            Vector3 newPos = transform.position;
            newPos.x = target.position.x;
            transform.position = newPos;
        }
	}

    void Start()
    {
        transform.parent = null;
    }

	void FixedUpdate ()
    {
        //If the player is outside the deadzone, scroll horizontally until he's instead the deadzone.

        //Move the target back in to the deadzone
        float xPos = transform.position.x;
        if (!TargetInDeadzone(transform.position.x))
        {
            float increment = 0.01f * Mathf.Sign(target.position.x - (deadzoneLeftBound + xPos));

            xPos = Utils.GuessValue(transform.position.x, TargetInDeadzone, increment, false);

            Vector3 newPos = transform.position;
            newPos.x = xPos;
            transform.position = newPos;
        }

        inDeadzone = TargetInDeadzone(transform.position.x);    //DEBUG
	}

    private bool TargetInDeadzone(float xPos)
    {
        //Returns if the target would be in the deadzone if the camera's xposition were xPos

        //Calculate the area of the deadzone
        float height = 10;

        Vector2 pos = new Vector2(xPos, transform.position.y);

        Vector2 pointA = new Vector2(deadzoneLeftBound, height / 2);
        Vector2 pointB = new Vector2(deadzoneRightBound, -height / 2);

        pointA += pos;
        pointB += pos;

        //Check to see if the target is within that area
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB);

        foreach (Collider2D c in colliders)
        {
            if (c.transform == target)
            {
                return true;
            }
        }

        //Return false if the target was not found in the area
        return false;
    }

    void OnDrawGizmos()
    {
        float height = 10;
        
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        
        Vector2 pointA = new Vector2(deadzoneLeftBound, height / 2);
        Vector2 pointB = new Vector2(deadzoneRightBound, -height / 2);

        pointA += pos;
        pointB += pos;
         
        Debug.DrawLine(new Vector3(pointA.x, pointA.y, 0), new Vector3(pointB.x, pointB.y));
    }
}
