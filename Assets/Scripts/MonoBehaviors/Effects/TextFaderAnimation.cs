using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]

public class TextFaderAnimation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    public float stayScale = 1;
    
    public float growTime = 0.25f;
    public float stayTime = 3f;
    public float fadeOutTime = 0.25f;
    public float fadeOutScale = 2f;
    
    public float Scale
    {
        get {return transform.localScale.x;}
        set {transform.localScale = new Vector3(value, value, 0);}
    }
    
    public float Alpha
    {
        get
        {
            return spriteRenderer.color.a;
        }
        
        set
        {
            Color c = spriteRenderer.color;
            c.a = value;
            spriteRenderer.color = c;
        }
    }
    
    public enum State {growing, staying, fading};
    private State currentState = State.growing;
    
    private float timer = 0f;
    
    //Events
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        //Increment the timer
        timer += Time.deltaTime;
        
        //Finite state machine
        
        if (currentState == State.growing)
        {
            //Grow
            Scale = Mathf.Lerp(0, stayScale, timer / growTime);
            
            //Move to the next state
            if (timer >= growTime)
            {
                Scale = stayScale;
                
                timer -= growTime;
                currentState = State.staying;
            }
            
        }
        else if (currentState == State.staying)
        {
            //Move to the next state after staying
            if (timer >= stayTime)
            {
                timer -= stayTime;
                currentState = State.fading;
            }
        }
        else if (currentState == State.fading)
        {
            //Grow
            Scale = Mathf.Lerp(stayScale, fadeOutScale, timer / fadeOutTime);
            
            //Fade
            Alpha = Mathf.Lerp(1, 0, timer / fadeOutTime);
            
            //Destroy after time is up.
            if (timer >= fadeOutTime)
            {
                GameObject.Destroy(gameObject, 0.1f);
            }
        }
    }
}
