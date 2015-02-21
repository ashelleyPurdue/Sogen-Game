using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircleChartRenderer : MonoBehaviour
{
    
    private MeshFilter circleMesh;
    private MeshRenderer circleRend;
    
    public int fullCirclePoints = 30;
    
    public Color color = Color.green;
    private Color lastColor = Color.green;
    
    public float portionFilled = 1;
    private float lastPortionFilled = 1;
    
    void Awake()
    {
        //Create the renderer
        circleRend = gameObject.AddComponent<MeshRenderer>();
        circleRend.material = new Material(Shader.Find("Sprites/Default"));
        circleRend.material.color = color;
        
        //Create a test mesh
        circleMesh = gameObject.AddComponent<MeshFilter>();
        CreateCircle(1);
    }
    
    void Update()
    {
        //Update the portion filled when it's changed
        if (portionFilled != lastPortionFilled)
        {
            UpdateTriangles(portionFilled);
            
            //Recalculate the normals if it went negative
            if (lastPortionFilled < 0)
            {
                circleMesh.mesh.RecalculateNormals();
            }
            
            lastPortionFilled = portionFilled;
        }
        
        //Update the color when it's changed
        if (color != lastColor)
        {
            circleRend.material.color = color;
            lastColor = color;
        }
    }
    
    //Misc methods
    
    private void CreateCircle(float radius)
    {
        //Create a list of vertices
        
        circleMesh.mesh.Clear();
        
        List<int> triangles = new List<int>();
        
        List<Vector3> points = new List<Vector3>();
        points.Add(Vector3.zero);
        
        for (int i = 0; i < fullCirclePoints + 1; i++)
        {
            //Plot the vertex
            float angle = Mathf.Lerp(0, 2 * Mathf.PI, (float)i / fullCirclePoints);
            
            Vector3 point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            points.Add(point);
        }
        
        //Link up the triangles.
        circleMesh.mesh.vertices = points.ToArray();
        UpdateTriangles(portionFilled);
        circleMesh.mesh.RecalculateNormals();
    }
    
    private void UpdateTriangles(float portion)
    {
        //Updates the triangles to only show a certain percentage of the circle.
        
        //Figure out how many points to draw.
        int pointsToDraw = (int)(portion * fullCirclePoints);
        
        if (pointsToDraw > fullCirclePoints)
        {
            pointsToDraw = fullCirclePoints;
        }
        
        //Generate the triangles
        List<int> triangles = new List<int>();

        for (int i = 1; i < pointsToDraw + 1; i++)
        {
            //Add the triangle to the list.
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(i);
        }
        
        //Set the triangles in the mesh
        circleMesh.mesh.triangles = triangles.ToArray();
    }
}
