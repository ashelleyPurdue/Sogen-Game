using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class SidescrollerCameraBehavior : MonoBehaviour
{
    private const float MAX_CAMERA_SPEED = 30f;
    private const float BACK_DISTANCE = -100f;  //The camera's z position
    
    public float deadzoneLeftBound = - 10;
    public float deadzoneRightBound = -5;

    public Transform target;

    public float Height
    {
        get { return 2 * camera.orthographicSize; }
    }

    public float Width
    {
        get { return 2f * camera.orthographicSize * camera.aspect;}
    }
    
    private float lastTargetX;

    private Transform leftBoundpost = null;
    private Transform rightBoundpost = null;

    private Vector3 targetPosition;

    private BoxCollider2D myTrigger;
 
    private bool lockedInPlace = false;
    
    private int framesToJumpToPlayer = 1;    //At the start of the level, the camera has this many frames to jump to the player, to account for the player using different entrances.
    
    //Events

	void Awake ()
    {
        transform.parent = null;
        
        //Get the box collider, creating it if there is none.
        myTrigger = GetComponent<BoxCollider2D>();
        if (myTrigger == null)
        {
            myTrigger = gameObject.AddComponent<BoxCollider2D>();
        }

        //Configure the box collider.
        myTrigger.isTrigger = true;
        myTrigger.size = new Vector2(Width, Height);

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

        //Set the target position to here.
        targetPosition = transform.position;
        
        //Set the z position
        transform.position = Utils.SetVector(transform.position, null, null, BACK_DISTANCE);
	}
 
    void Update()
    {
        //Move the real position and target position to the player for the first few frames.
        if (framesToJumpToPlayer > 0)
        {
            if (!TargetInDeadzone(targetPosition.x))
            {
                Debug.Log("Moving camera to player position.");
                
                Vector3 newPos = transform.position;
                newPos.x = target.position.x;
                
                transform.position = newPos;
                targetPosition = transform.position;
            }
            
            framesToJumpToPlayer--;
        }
        
        //Move toward the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, MAX_CAMERA_SPEED * Time.deltaTime);
    }
    
	void FixedUpdate ()
    {
        //If the player is outside the deadzone, scroll horizontally until he's instead the deadzone.
  
        if (!lockedInPlace)
        {
            //Move the target back in to the deadzone
            float xPos = targetPosition.x;
            if (!TargetInDeadzone(targetPosition.x))
            {
                float increment = 0.01f * Mathf.Sign(target.position.x - (deadzoneLeftBound + xPos));
       
                try
                {
                    xPos = Utils.GuessValue(targetPosition.x, TargetInDeadzone, increment, false);
                }
                catch(GuessValueMaxIterationException e)
                {
                    Debug.Log("Camera unable to find player.");
                }
    
                Vector3 newPos = targetPosition;
                newPos.x = xPos;
                targetPosition = newPos;
            }
    
            //Move the camera back inside the boundposts
            KeepInBoundposts();
        }

	}

    void OnDrawGizmos()
    {
        //Draw the deadzone
        float height = 10;
        
        Vector2 pos = new Vector2(targetPosition.x, targetPosition.y);
        
        Vector2 pointA = new Vector2(deadzoneLeftBound, height / 2);
        Vector2 pointB = new Vector2(deadzoneRightBound, -height / 2);
        
        pointA += pos;
        pointB += pos;

        Debug.DrawLine(new Vector3(pointA.x, pointA.y, 0), new Vector3(pointB.x, pointB.y));
    }

    //Misc methods
    
    public bool InView(Bounds bounds)
    {
        //Returns if bounds is within the view

        bool xBounds = (bounds.min.x <= collider2D.bounds.max.x) && (bounds.max.x >= collider2D.bounds.min.x);
        bool yBounds = (bounds.min.y <= collider2D.bounds.max.y) && (bounds.max.y >= collider2D.bounds.min.y);

        return xBounds && yBounds;
    }
 
    public void LockInPlace(float xPos)
    {
        //Locks the camera's target position in one place
        lockedInPlace = true;
        targetPosition.x = xPos;
    }
    
    public void Unlock()
    {
        //Lets the camera move freely again.
        lockedInPlace = false;
    }
    
    public void SetBoundposts(Transform left, Transform right)
    {
        //Sets the boundposts

        leftBoundpost = left;
        rightBoundpost = right;
    }
    
    public void SetLeftBoundpost(Transform boundpost)
    {
        leftBoundpost = boundpost;
    }
    
    public void SetRightBoundpost(Transform boundpost)
    {
        rightBoundpost = boundpost;
    }
    
    public Transform GetLeftBoundpost()
    {
        return leftBoundpost;
    }
    
    public Transform GetRightBoundpost()
    {
        return rightBoundpost;
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
  
        bool movedLeft = false;
        bool movedRight = false;
        
        //Left boundpost
        if (leftBoundpost != null)
        {
            float borderX = targetPosition.x - halfWidth;

            //If the border is too far to the left, move it back.
            if (borderX < leftBoundpost.position.x)
            {
                float difference = leftBoundpost.position.x - borderX;
                newX = targetPosition.x + difference;
                
                movedLeft = true;
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
                
                movedRight = true;
            }
        }
  
        //If the camera was moved both ways, then keep it in the middle
        if (movedLeft && movedRight)
        {
            newX = (rightBoundpost.position.x + leftBoundpost.position.x) / 2;
        }
        
        //Update the position
        targetPosition = new Vector3(newX, targetPosition.y, targetPosition.z);
    }

   
}
