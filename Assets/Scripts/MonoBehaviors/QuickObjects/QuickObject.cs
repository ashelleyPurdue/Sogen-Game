using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public abstract class QuickObject : MonoBehaviour
{
    public const string SHADER = "Unlit/Transparent";

    public Texture texture;
    public bool autoStretchTexture = false;
    
    private string shaderString = "";
    
    protected MeshFilter filter;
    
    //Events
    
    void Start()
    {  
        //Automatically fix rectangles that have been placed using the wrong shader
        if (!shaderString.Equals(SHADER))
        {
            renderer.sharedMaterial.shader = Shader.Find(SHADER);
            shaderString = SHADER;
        }
    }
    
    //Abstract methods
    
    protected abstract void CreateMesh();
    protected abstract void UpdateCollider();
    
    //Private methods
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
        if (autoStretchTexture)
        {
            UpdateTextureScale();
        }

    }

    //Public methods
    
    public void UpdateMesh()
    {
        //Create the mesh's material if it doesn't exist.
        if (renderer.sharedMaterial == null)
        {
            renderer.sharedMaterial = new Material(Shader.Find(SHADER));
            shaderString = SHADER;
        }
        
        CreateMesh();
        UpdateCollider();
        UpdateTexture();
    }
            
    //Overridden methods
    protected virtual void UpdateTextureScale()
    {
    }
}
