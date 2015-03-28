using UnityEngine;
using System.Collections;

public class CheckpointFlameAnimation : MonoBehaviour
{
    public bool visible = false;
    
    private float timer = 0f;
    
    private float targetScale = 0;
    
    private const float MAX_GROW_SPEED = 4;
    
    //Events
    
    void Awake()
    {
        transform.localScale = new Vector3(1, 0, 0);
    }
    
    void Update()
    {
        //If we're visible, animate.  Else, shrink.
        if (visible)
        {
            timer += Time.deltaTime;
            
            if (timer >= Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }
            
            targetScale = 0.5f + Mathf.Abs(Mathf.Sin(timer) / 2f);
        }
        else
        {
            targetScale = 0f;
        }
        
        //Move toward the target scale.
        transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(1, targetScale, 0), MAX_GROW_SPEED * Time.deltaTime); 
    }
}
