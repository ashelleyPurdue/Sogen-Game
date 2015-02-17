using UnityEngine;
using System.Collections;

public class LevelSelectorBehavior : MonoBehaviour
{
    public float speed = 10;

    //Events
    void Update()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rigidbody2D.velocity = inputVector * speed;
        Debug.Log(Time.timeScale);
    }
}
