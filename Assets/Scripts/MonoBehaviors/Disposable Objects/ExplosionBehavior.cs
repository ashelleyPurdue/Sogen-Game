using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DamageSource))]
[RequireComponent(typeof(CircleCollider2D))]

public class ExplosionBehavior : MonoBehaviour
{
    public const float GROW_TIME = 0.25f;
    public const float DEFAULT_DAMAGE_DURATION = 0.25f;
    public const float DEFAULT_FADE_TIME = 0.5f;
    
    public float damageDuration = DEFAULT_DAMAGE_DURATION;  //How long after growing is finished to deal damage.
    public float fadeTime = DEFAULT_FADE_TIME;              //How long after growing to fade out for.
    
    public float radius = 1f;
    
    public enum State {growing, fading};
    private State currentState = State.growing;
    
    private float timer = 0f;
    
    private SpriteRenderer sprRenderer = null;
    
    //Static methods
    
    public static ExplosionBehavior CreateExplosion(Vector3 position, float radius, float damageDuration = DEFAULT_DAMAGE_DURATION, float fadeTime = DEFAULT_FADE_TIME)
    {
        //Creates an explosion at the given point.
        
        //Create the object
        GameObject explObj = new GameObject();
        
        explObj.AddComponent<DamageSource>();
        explObj.AddComponent<CircleCollider2D>();
            
        ExplosionBehavior explosion = explObj.AddComponent<ExplosionBehavior>();
        SpriteRenderer sprRend = explObj.AddComponent<SpriteRenderer>();
        
        //Configure the object
        explObj.transform.position = position;
        explosion.radius = radius;
        explosion.damageDuration = damageDuration;
        
        sprRend.sprite = (Sprite)(Resources.Load("explosion", typeof(Sprite)));
        
        //Return the explosion
        return explosion;
    }
    
    //Events
    
    void Start()
    {
        //Configure the collider
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        
        col.radius = 1;
        col.isTrigger = true;
        
        //Add damage tag
        GetComponent<DamageSource>().AddDamageTag(DamageTags.Explosion);
        
        //Get renderer
        sprRenderer = GetComponent<SpriteRenderer>();
    }
    
    void FixedUpdate()
    {
        //Increment the timer.
        timer += Time.deltaTime;
        
        //Finite state machine
        if (currentState == State.growing)
        {
            //While growing
            
            //Lerp size
            float scale = Mathf.Lerp(0, radius, timer / GROW_TIME);
            
            //Move on when time is up
            if (timer >= GROW_TIME)
            {
                timer -= GROW_TIME;
                currentState = State.fading;
                
                scale = radius;
            }
            
            //Update scale
            transform.localScale = new Vector3(scale, scale, 0);
        }
        else if (currentState == State.fading)
        {
            //While fading
            
            //Update alpha
            Color color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), timer / fadeTime);
            sprRenderer.material.color = color;
            
            //disable damage source after damage time is up
            if (timer >= damageDuration)
            {
                GetComponent<DamageSource>().isHot = false;
            }
            
            //Destroy when completely faded AND damage duration is over.
            if (timer >= fadeTime && timer >= damageDuration)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
