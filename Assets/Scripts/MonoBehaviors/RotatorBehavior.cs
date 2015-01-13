using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class RotatorBehavior : MonoBehaviour
{
    public float rotSpeed;

    void Update()
    {
        rigidbody2D.angularVelocity = rotSpeed * Mathf.Sin(Time.time);
    }
}
