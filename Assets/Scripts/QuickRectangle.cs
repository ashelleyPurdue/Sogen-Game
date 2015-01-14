using UnityEngine;
using System.Collections;
using UnityEditor;

public class QuickRectangle : MonoBehaviour
{
    public Texture texture;
    public bool autoStretchTexture = false;

    public Vector3 pointA = new Vector3(0, 1, 0);
    public Vector3 pointB = new Vector3(1, 0, 0);

    private BoxCollider2D boxCol;

    private bool fixedShader = false;

    private MeshFilter filter;

	// Use this for initialization
	void Start()
    {
        //Automatically fix rectangles that have been placed using the Diffuse shader.
        if (!fixedShader)
        {
            renderer.sharedMaterial.shader = Shader.Find("Unlit/Texture");
            fixedShader = true;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    //Misc methods

    private void CreateMesh()
    {
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

    private void UpdateCollider()
    {
        //Update the collider
        boxCol = GetComponent<BoxCollider2D>();
        
        boxCol.center = (pointA + pointB) / 2;
        boxCol.size = new Vector2(Mathf.Abs(pointA.x - pointB.x), Mathf.Abs(pointA.y - pointB.y));
    }

    private void UpdateTexture()
    {
        if (texture != null)
        {
            renderer.sharedMaterial.SetTexture(0, texture);
        }

        //Create the UVs
        Vector2[] uvs = new Vector2[4];
        for (int i = 0; i < filter.sharedMesh.vertices.Length; i++)
        {
            uvs[i] = new Vector2(filter.sharedMesh.vertices[i].x, filter.sharedMesh.vertices[i].y);
        }

        filter.sharedMesh.uv = uvs;
        
        //Update the texture tiling.
        if (false)
        {
            Vector2 inverseSize = new Vector2(1 / boxCol.size.x, 1 / boxCol.size.y);
            renderer.sharedMaterial.SetTextureScale("_MainTex", inverseSize);
        }

    }

    public void UpdateMesh()
    {
        //Create the filter's mesh if it doesn't exist.
        if (filter == null || filter.sharedMesh == null)
        {
            filter = GetComponent<MeshFilter>();
            filter.sharedMesh = new Mesh();
            filter.sharedMesh.vertices = new Vector3[4];

            if (renderer.sharedMaterial == null)
            {
                renderer.sharedMaterial = new Material(Shader.Find("Unlit/Texture"));
                fixedShader = true;
            }
        }
        
        CreateMesh();
        UpdateCollider();
        UpdateTexture();
    }
}

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
