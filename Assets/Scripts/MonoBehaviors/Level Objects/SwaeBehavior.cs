using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CollectableBehavior))]
public class SwaeBehavior : MonoBehaviour
{   
    public const float MAX_ROTSPEED = 360;
    public const float MIN_ROTSPEED = 180;
    
    public const float SWIRL_TIME = 0.25f;
    public const float SWIRL_RADIUS = 1f;
    
    public const float MOVE_SPEED = 10f;
    
    public enum State {notCollected, swirling, movingTowardsPlayer};
    private State currentState = State.notCollected;
    
    private float rotSpeed;
    
    private float timer = 0f;
    
    private Vector3 swirlStartPos;
    private Vector3 targetPos;
    
    private float RotAngle
    {
        get { return transform.localEulerAngles.z;}
        
        set
        {
            transform.localRotation = Quaternion.Euler(0, 0, value);
        }
    }
    
    private float SwirlAngle
    {
        set
        {
            Vector3 offset = new Vector3(Mathf.Cos(value), Mathf.Sin(value), 0) * SWIRL_RADIUS;
            targetPos = swirlStartPos + offset;
        }
    }
    
	//Events
	void Awake ()
    {
	    GetComponent<CollectableBehavior>().useDefaultDestructionBehavior = false;
        
        //Randomize the rotspeed
        rotSpeed = Random.Range(MIN_ROTSPEED, MAX_ROTSPEED);
        
        //Randomize the angle
        RotAngle = Random.Range(0, 360);
        
        //Set the target pos
        targetPos = transform.localPosition;
	}
	
	void Update ()
    {
	    //Rotate
        RotAngle += rotSpeed * Time.deltaTime;
        
        //Move to the target pos
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, MOVE_SPEED * Time.deltaTime);
        
        //Finite state machine
        if (currentState == State.swirling)
        {
            timer += Time.deltaTime;
            SwirlAngle = Mathf.Lerp(0, 180 * Mathf.Deg2Rad, timer / SWIRL_TIME);
            
            if (timer >= SWIRL_TIME)
            {
                //Start moving toward the player
                currentState = State.movingTowardsPlayer;
                targetPos = Vector3.zero;
            }
        }
        else if (currentState == State.movingTowardsPlayer)
        {
            //Destroy when we reach the player
            
            if (transform.localPosition == targetPos)
            {
                GameObject.Destroy(gameObject);
            }
        }
	}
    
    void OnCollected()
    {
        //Start doing a swirl
        currentState = State.swirling;
        
        transform.parent = TagList.FindOnlyObjectWithTag("Player");
        
        swirlStartPos = Vector3.zero;
        timer = 0f;
    }
    
}
