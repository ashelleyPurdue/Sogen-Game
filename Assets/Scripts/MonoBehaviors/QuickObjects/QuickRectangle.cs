using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class QuickRectangle : QuickObject
{
    public Vector3 pointA = new Vector3(0, 1, 0);
    public Vector3 pointB = new Vector3(1, 0, 0);

    private BoxCollider2D boxCol;

    //Misc methods

    protected override void CreateMesh()
    {
        //Create the filter's mesh if it doesn't exist.
        if (filter == null || filter.sharedMesh == null)
        {
            filter = GetComponent<MeshFilter>();
            filter.sharedMesh = new Mesh();
            filter.sharedMesh.vertices = new Vector3[4];
        }
        
        //Create the vertices
        Vector3[] vertices = new Vector3[4];
        
        vertices [0] = pointA;
        vertices [1] = new Vector3(pointB.x, pointA.y);
        vertices [2] = pointB;
        vertices [3] = new Vector3(pointA.x, pointB.y);
        
        //Update the mesh
        filter.sharedMesh.Clear();
        filter.sharedMesh.vertices = vertices;
        
        filter.sharedMesh.triangles = new int[] {0, 1, 3, 2, 3, 1};
        filter.sharedMesh.RecalculateNormals();
    }

    protected override void UpdateCollider()
    {
        //Update the collider
        boxCol = GetComponent<BoxCollider2D>();
        
        boxCol.center = (pointA + pointB) / 2;
        boxCol.size = new Vector2(Mathf.Abs(pointA.x - pointB.x), Mathf.Abs(pointA.y - pointB.y));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(QuickRectangle))]
public class QuickRectangleEditor : Editor
{
    void OnSceneGUI()
    {
        QuickRectangle recTarget = (QuickRectangle)target;

        Vector3 rectPos = recTarget.transform.position;

        recTarget.pointA = Handles.PositionHandle(rectPos + recTarget.pointA, Quaternion.identity) - rectPos;
        recTarget.pointB = Handles.PositionHandle(rectPos + recTarget.pointB, Quaternion.identity) - rectPos;

        recTarget.UpdateMesh();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
#endif