using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class SidescrollerCameraBehavior : MonoBehaviour
{

    private float deadzoneWidth = 0.1f;

    public Transform target;

    private Vector2 relativeDeadzoneTL;
    private Vector2 relativeDeadzoneBR;

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

        //Calculate the size of the deadzone
        float height = camera.orthographicSize * 2f;
        float width = camera.aspect * height;

        relativeTopLeft = new Vector2(-width / 2, height / 2);
        relativeBottomRight = new Vector2(width / 2, -height / 2);

        relativeDeadzoneTL = relativeTopLeft;
        relativeDeadzoneTL.x *= deadzoneWidth;

        relativeDeadzoneBR = relativeBottomRight;
        relativeDeadzoneBR.x *= deadzoneWidth;

        //Record the last position of the target
        lastTargetX = target.position.x;
	}

    void Start()
    {
        transform.parent = null;

        //Move to the target if he is off screen
        if (!TargetInDeadzone())
        {
            Vector3 newPos = transform.position;
            newPos.x = target.position.x;
            transform.position = newPos;
        }
    }

	void FixedUpdate ()
    {
        //If the player is outside the deadzone, scroll horizontally until he's instead the deadzone.

        bool oldWay = false;

        //TODO: Use a less-costly algorithm
        float pixelSize = (camera.aspect * camera.orthographicSize * 2f) / camera.pixelWidth;
        float baseIncrement = 0.5f * pixelSize * Mathf.Sign(target.position.x - transform.position.x);

        if (oldWay)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);

            if (TargetInArea(relativeTopLeft + pos2D, relativeBottomRight + pos2D) && !TargetInDeadzone())
            {
                Vector3 pos = transform.position;
                pos.x += baseIncrement;
                transform.position = pos;

                pos2D.x = pos.x;
            }

        } else
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);

            if (TargetInArea(relativeTopLeft + pos2D, relativeBottomRight + pos2D))
            {
                float[] increments = {4 * baseIncrement, baseIncrement};

                float xPos = Utils.GuessValue(transform.position.x, TargetInDeadzone, increments, true);

                Vector3 pos = transform.position;
                pos.x = xPos;
                transform.position = pos;
            }
        }
	}

    void OnDrawGizmos()
    {
        //Draw the deadzone
        Vector3 deadTL3D = new Vector3(relativeDeadzoneTL.x, relativeDeadzoneTL.y, 0);
        Vector3 deadBR3D = new Vector3(relativeDeadzoneBR.x, relativeDeadzoneBR.y, 0);

        Gizmos.DrawSphere(deadTL3D + transform.position, 1f);
        Gizmos.DrawSphere(deadBR3D + transform.position, 1f);

        Gizmos.DrawLine(deadTL3D + transform.position, deadBR3D + transform.position);
    }

    private bool TargetInDeadzone()
    {
        //Returns if the target is completely within the deadzone

        Vector2 position2D = new Vector2(transform.position.x, transform.position.y);
        return TargetInArea(relativeDeadzoneTL + position2D, relativeDeadzoneBR + position2D);
    }

    private bool TargetInDeadzone(float xPos)
    {
        Vector2 position2D = new Vector2(xPos, transform.position.y);
        return TargetInArea(relativeDeadzoneTL + position2D, relativeDeadzoneBR + position2D);
    }

    private bool TargetInArea(Vector2 topLeft, Vector2 bottomRight)
    {
        Collider2D[] hits = Physics2D.OverlapAreaAll(topLeft, bottomRight);

        foreach (Collider2D col in hits)
        {
            if (col.transform == target)
            {
                return true;
            }
        }

        return false;
    }
}
