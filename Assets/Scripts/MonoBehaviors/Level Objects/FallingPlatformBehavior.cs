using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class FallingPlatformBehavior : MonoBehaviour
{
    public float fallSpeed = 1f;

    private bool falling = false;

    //Events

    void Awake()
    {
        //Add a trigger.

        BoxCollider2D normalCollider = GetComponent<BoxCollider2D>();
        BoxCollider2D newTrigger = gameObject.AddComponent<BoxCollider2D>();

        newTrigger.isTrigger = true;
        newTrigger.center = normalCollider.center;
        newTrigger.size = new Vector2(normalCollider.size.x, normalCollider.size.y + 0.25f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Rigidbody2D>() != null)
        {
            falling = true;
        }
    }

    void Update()
    {
        if (falling)
        {
            rigidbody2D.isKinematic = false;
            rigidbody2D.velocity = new Vector2(0, fallSpeed * -1);
        }
    }
}
