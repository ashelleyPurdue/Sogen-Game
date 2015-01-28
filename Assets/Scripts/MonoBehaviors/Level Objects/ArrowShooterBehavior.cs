using UnityEngine;
using System.Collections;

public class ArrowShooterBehavior : MonoBehaviour
{
    
    public Transform arrowPoint;
 
    public float waitTime = 3f;
    public float warningTime = 0.5f;
    
    public float arrowSpeed = 10f;
    
    public enum State {waiting, warning};
    private State currentState;
    
    private ArrowBehavior myArrow = null;
    
    private float timer = 0f;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (currentState == State.waiting)
        {
            //Start warning after a bit
            
            myArrow = null;
            
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                //Move to the next state
                timer -= waitTime;
                currentState = State.warning;
                
                //Create the arrow
                myArrow = ((GameObject)Instantiate(Resources.Load("arrow_prefab"))).GetComponent<ArrowBehavior>();
                myArrow.transform.position = arrowPoint.position;
                myArrow.transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z);
            }
        }
        else if (currentState == State.warning)
        {
            //After warning for a bit, fire.
            
            timer += Time.deltaTime;
            if (timer >= warningTime)
            {
                //Fire
                float theta = transform.eulerAngles.z * Mathf.Deg2Rad;
                myArrow.rigidbody2D.velocity = arrowSpeed * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
                
                //Move to the next state
                timer = 0f;
                currentState = State.waiting;
            }
        }
	}
}
