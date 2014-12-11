using UnityEngine;
using System.Collections;

public class LevelSelectorBehavior : MonoBehaviour
{
    public float speed = 10;

    //Events
    void Update()
    {
        rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
    }
}
