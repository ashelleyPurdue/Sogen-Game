using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DamageSource))]
[RequireComponent(typeof(CircleCollider2D))]

public class ExplosionBehavior : MonoBehaviour
{
    public float duration = 0.5f;
    
    public float radius = 1f;
    
    private float timer = 0f;
    
    
    //Static methods
    
    public static ExplosionBehavior CreateExplosion(Vector3 position, float radius, float duration)
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
        explosion.duration = duration;
        
        sprRend.sprite = (Sprite)(Resources.Load("explosion", typeof(Sprite)));
        
        //Return the explosion
        return explosion;
    }
    
    //Events
    
    void Awake()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        
        col.radius = radius;
        col.isTrigger = true;
        
        GetComponent<DamageSource>().AddDamageTag(DamageTags.Explosion);
    }
    
    void FixedUpdate()
    {
        //Increment the timer.
        timer += Time.deltaTime;
        
        //Update size
        float scale = Mathf.Lerp(0, radius, timer / duration);
        transform.localScale = new Vector3(scale, scale, 0);
        
        //Destroy when time is up.
        if (timer >= duration)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
