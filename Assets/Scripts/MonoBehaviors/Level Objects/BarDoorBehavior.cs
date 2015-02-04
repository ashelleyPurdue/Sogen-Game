using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CircuitNode))]
public class BarDoorBehavior : MonoBehaviour
{
    public float height = 1;
    public float openCloseTime = 0.25f;

    private CircuitNode node;

    private float targetHeight;

    void Awake()
    {
        node = GetComponent<CircuitNode>();
    }

    void FixedUpdate()
    {
        //Update the target height depending on the powered state
        if (node.IsPowered())
        {
            targetHeight = 0;
        } else
        {
            targetHeight = height;
        }

        //Step towards the target height
        if (transform.localScale.y != targetHeight)
        {
            Vector3 newScale = transform.localScale;
            float speed = (height / openCloseTime) * Mathf.Sign(targetHeight - newScale.y);
       
            newScale.y += speed * Time.deltaTime;

            if (Mathf.Abs(newScale.y - targetHeight) <= speed * Time.deltaTime)
            {
                newScale.y = targetHeight;
            }

            transform.localScale = newScale;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BarDoorBehavior))]
public class BarDoorBehaviorEditor : Editor
{
    void OnSceneGUI()
    {
        BarDoorBehavior door = (BarDoorBehavior)target;

        Vector3 handlePos = door.transform.position;
        handlePos.y -= door.height;
        handlePos.x -= 0.5f;

        door.height = (Handles.PositionHandle(handlePos, Quaternion.identity).y - door.transform.position.y) * -1;

        Vector3 newScale = door.transform.localScale;
        newScale.y = door.height;
        door.transform.localScale = newScale;

        /*
        recTarget.pointA = Handles.PositionHandle(rectPos + recTarget.pointA, Quaternion.identity) - rectPos;
        recTarget.pointB = Handles.PositionHandle(rectPos + recTarget.pointB, Quaternion.identity) - rectPos;
        
        recTarget.UpdateMesh();
        */

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
#endif