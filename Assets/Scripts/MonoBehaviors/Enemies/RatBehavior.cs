using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlatformCharacterMotor))]
[RequireComponent(typeof(BoxCollider2D))]
public class RatBehavior : MonoBehaviour
{

    private PlatformCharacterMotor myMotor;

    private SidescrollerCameraBehavior myCamera;

    private bool awake = false;

    //Events

    void Start()
    {
        myMotor = GetComponent<PlatformCharacterMotor>();
        myCamera = TagList.FindOnlyObjectWithTag("camera").GetComponent<SidescrollerCameraBehavior>();
    }

	// Update is called once per frame
	void Update ()
    {
        //Wake up when you're in the camera's view
        if (myCamera.InView(collider2D.bounds))
        {
            awake = true;
        }

        if (awake)
        {
            myMotor.ControllerInput = new Vector2(-1, 0);
        }
	}
}
