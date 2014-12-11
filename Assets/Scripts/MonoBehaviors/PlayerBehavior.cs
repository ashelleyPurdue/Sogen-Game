using UnityEngine;
using System.Collections;

public class PlayerBehavior : MonoBehaviour
{
    public float speed;

    public void Update()
    {
        //Move
        rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
    }

    public void OnDead()
    {
        //Return to the last used checkpoint.
        CourseManager.ReturnToActiveCheckpoint();
    }
}
