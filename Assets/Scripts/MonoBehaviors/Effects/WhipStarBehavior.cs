using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class WhipStarBehavior : MonoBehaviour
{
    public float minLineLength = 0.25f;
    public float maxLineLength = 1.5f;
    
    public float lineWidth = 0.1f;
    public int numLines = 5;
    
    public float holdDuration = 0.1f;
    public float fadeDuration = 0.5f;
    
    private float timer = 0f;
    private bool fading = false;
    
    private List<LineRenderer> lines = new List<LineRenderer>();
    
    //Events
    
    void Awake()
    {
        for (int i = 0; i < numLines; i++)
        {
            //Determine the length of the line.
            float length = Random.Range(minLineLength, maxLineLength);
            
            //Determine the angle of the line
            float angle;
            
            //If we  don't already have a line in every quadrant, then restrict this line to being in the next unused quadrant.
            //Otherwise, let the angle be in any quadrant.
            if (i < 4)
            {
                Debug.Log("" + (i + 1) + "th quadrant.");
                float NINETY_DEGREES = Mathf.PI / 2;
                angle = Random.Range(0, NINETY_DEGREES) + (NINETY_DEGREES * i);
            }
            else
            {
                angle = Random.Range(0, 2 * Mathf.PI);
            }
            
            CreateLine(length, angle);
        }
    }
    
    void Update()
    {
        //Hold for a bit, then fade out.
        
        timer += Time.deltaTime;
        
        if (!fading)
        {
            //Start fading after a bit.
            if (timer >= holdDuration)
            {
                timer -= holdDuration;
                fading = true;
            }
        }
        else
        {
            //Fade out over time.
            Color newColor = new Color(1, 0, 0, Mathf.Lerp(1, 0, timer / fadeDuration));
            foreach(LineRenderer line in lines)
            {
                line.SetColors(newColor, newColor);
            }
            
            //When time is up, destroy.
            if (timer >= fadeDuration)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
    
    //Misc methods
    
    private void CreateLine(float length, float angle)
    {
        //Creates a new line for the star.
        //Angle is in radians
        
        //Create the line gameObject
        GameObject lineObj = new GameObject("whipstarLine");
        lineObj.transform.parent = transform;
        lineObj.transform.localPosition = Vector3.zero;
        
        //Create the line renderer
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        
        //Configure the line
        Vector3 point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        
        line.SetVertexCount(2);
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, point);
        line.SetWidth(lineWidth, lineWidth);
        
        //Configure the line's material
        line.material = new Material(Shader.Find("Sprites/Default"));
        //line.material.color = Color.white;
        line.SetColors(Color.red, Color.red);
        
        //Add the line to the list
        lines.Add(line);
    }
}
