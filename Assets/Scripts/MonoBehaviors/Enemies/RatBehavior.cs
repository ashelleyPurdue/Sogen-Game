using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlatformCharacterMotor))]
public class RatBehavior : MonoBehaviour
{

    private PlatformCharacterMotor myMotor;

    private bool awake = false;

    //Events

    void Awake()
    {
        myMotor = GetComponent<PlatformCharacterMotor>();
    }

    void OnWillRenderObject()
    {
        awake = true;
    }

	// Update is called once per frame
	void Update ()
    {
        if (awake)
        {
            myMotor.ControllerInput = new Vector2(-1, 0);
        }
	}
}
