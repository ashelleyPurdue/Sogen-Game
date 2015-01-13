using UnityEngine;
using System.Collections;

public class ConstantForce2D : MonoBehaviour {

    public Vector2 force;

	void FixedUpdate()
    {
        //rigidbody2D.AddForce(force * Time.deltaTime);
        rigidbody2D.velocity = force;
    }
}
