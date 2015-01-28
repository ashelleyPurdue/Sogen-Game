using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockFlashEffect : MonoBehaviour
{
    public float flashTime = 0.25f;
    
    public List<Renderer> renderers = new List<Renderer>();
    
    private float timer = 0f;
    private bool flashing = false;
    
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();
    private Dictionary<Renderer, Material> originalMaterial = new Dictionary<Renderer, Material>();
    
    private Color flashColor = Color.cyan;
    
    //Events
    
    void Awake()
    {
        //Add all materials to the list
        
        foreach (Renderer r in renderers)
        {
            originalColors.Add(r, r.material.color);
            originalMaterial.Add(r, r.material);
        }
    }
    
    void OnDamageBlocked()
    {
        //Start the flash effect
        timer = 0f;
        flashing = true;
        
        Debug.Log("Whip blocked.");
    }
    
    void Update()
    {
        if (flashing)
        {
            //Increment the timer
            timer += Time.deltaTime;
            
            //For every renderer, tween from the flash material to the original material.
            foreach (Renderer r in renderers)
            {   
                Debug.Log("About to create new color.");
                
                if(!originalColors.ContainsKey(r))
                {
                    originalColors.Add(r, r.material.color);
                    originalMaterial.Add(r, r.material);
                }
                
                Material newMat = new Material(Shader.Find("Sprites/Default"));
                newMat.mainTexture = originalMaterial[r].mainTexture;
                
                newMat.color = Color.Lerp(flashColor, Color.white, timer / flashTime);
                
                r.material = newMat;
                    
                Debug.Log(r.material.color);
            }
            
            //If we're out of time, set the colors to their original and stop flashing
            if (timer >= flashTime)
            {
                foreach (Renderer r in renderers)
                {
                    r.material = originalMaterial[r];
                }
                
                timer = 0f;
                flashing = false;
            }
        }
    }
}
