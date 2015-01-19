using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class SidescrollerCameraBehavior : MonoBehaviour
{
    private const float MAX_CAMERA_SPEED = 30f;

    public float deadzoneLeftBound = - 10;
    public float deadzoneRightBound = -5;

    public Transform target;

    public float Width
    {
        get { return 2f * camera.orthographicSize * camera.aspect;}
    }

    private float lastTargetX;

    private Transform leftBoundpost = null;
    private Transform rightBoundpost = null;

    private Vector3 targetPosition;

    //Events

	void Awake ()
    {
        //Add a taglist with the tag "camera" if there is not one already
        if (!TagList.ObjectHasTag(this, "camera"))
        {
            TagList myTagList = GetComponent<TagList>();

            if (myTagList == null)
            {
                myTagList = gameObject.AddComponent<TagList>();
            }

            myTagList.AddTag("camera");
        }

        //If there is no assigned target, search for the player and set him as the target.
        if (target == null)
        {
            target = TagList.FindOnlyObjectWithTag("Player");
        }

        //Move to the player
        if (!TargetInDeadzone(targetPosition.x))
        {
            Vector3 newPos = transform.position;
            newPos.x = target.position.x;
            transform.position = newPos;
        }

        //Set the target position to here.
        targetPosition = transform.position;
	}

    void Start()
    {
        transform.parent = null;
    }

	void FixedUpdate ()
    {
        //If the player is outside the deadzone, scroll horizontally until he's instead the deadzone.

        //Move the target back in to the deadzone
        float xPos = targetPosition.x;
        if (!TargetInDeadzone(targetPosition.x))
        {
            float increment = 0.01f * Mathf.Sign(target.position.x - (deadzoneLeftBound + xPos));

            xPos = Utils.GuessValue(targetPosition.x, TargetInDeadzone, increment, false);

            Vector3 newPos = targetPosition;
            newPos.x = xPos;
            targetPosition = newPos;
        }

        //Move the camera back inside the boundposts
        KeepInBoundposts();

        //Move toward the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, MAX_CAMERA_SPEED * Time.deltaTime);
	}

    void OnDrawGizmos()
    {
        float height = 10;
        
        Vector2 pos = new Vector2(targetPosition.x, targetPosition.y);
        
        Vector2 pointA = new Vector2(deadzoneLeftBound, height / 2);
        Vector2 pointB = new Vector2(deadzoneRightBound, -height / 2);
        
        pointA += pos;
        pointB += pos;
        
        Debug.DrawLine(new Vector3(pointA.x, pointA.y, 0), new Vector3(pointB.x, pointB.y));
    }

    //Misc methods

    public void SetBoundposts(Transform left, Transform right)
    {
        //Sets the boundposts

        leftBoundpost = left;
        rightBoundpost = right;
    }

    private bool TargetInDeadzone(float xPos)
    {
        //Returns if the target would be in the deadzone if the camera's xposition were xPos

        //Calculate the area of the deadzone
        float height = 10;

        Vector2 pos = new Vector2(xPos, targetPosition.y);

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

    private void KeepInBoundposts()
    {
        //Keeps the border of the camera inside the boundposts
        float halfWidth = Width / 2;

        float newX = targetPosition.x;

        //Left boundpost
        if (leftBoundpost != null)
        {
            float borderX = targetPosition.x - halfWidth;

            //If the border is too far to the left, move it back.
            if (borderX < leftBoundpost.position.x)
            {
                float difference = leftBoundpost.position.x - borderX;
                newX = targetPosition.x + difference;
            }
        }

        //Right boundpost
        if (rightBoundpost != null)
        {
            float borderX = targetPosition.x + halfWidth;

            //If the border is too far to the right, move it back
            if (borderX > rightBoundpost.position.x)
            {
                float difference = rightBoundpost.position.x - borderX;
                newX = targetPosition.x + difference;
            }
        }

        //Update the position
        targetPosition = new Vector3(newX, targetPosition.y, targetPosition.z);
    }

   
}
