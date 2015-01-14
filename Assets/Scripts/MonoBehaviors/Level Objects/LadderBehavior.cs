using UnityEngine;
using UnityEditor;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class LadderBehavior : MonoBehaviour
{
    public float height;

    void Awake()
    {
        collider2D.isTrigger = true;
    }
}

[CustomEditor(typeof(LadderBehavior))]
public class LadderBehaviorEditor : Editor
{
    void OnSceneGUI()
    {
        LadderBehavior ladder = (LadderBehavior)target;
        
        Vector3 handlePos = ladder.transform.position;
        handlePos.y -= ladder.height;
        handlePos.x -= 0.5f;
        
        ladder.height = (Handles.PositionHandle(handlePos, Quaternion.identity).y - ladder.transform.position.y) * -1;
        
        Vector3 newScale = ladder.transform.localScale;
        newScale.y = ladder.height;
        ladder.transform.localScale = newScale;
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}