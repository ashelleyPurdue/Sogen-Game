using UnityEngine;
using System.Collections;

public class FogWallBehavior : MonoBehaviour
{
    private SidescrollerCameraBehavior myCamera;
    private Transform player;

    //Events

    void Start()
    {
        myCamera = TagList.FindOnlyObjectWithTag("camera").GetComponent<SidescrollerCameraBehavior>();
        player = TagList.FindOnlyObjectWithTag("Player");
    }

    void Update()
    {
        //When the object is in view, become the boundpost for the camera.

        if (myCamera.InView(collider2D.bounds))
        {

            if (player.position.x < transform.position.x)
            {
                //Become the right boundpost
                myCamera.SetRightBoundpost(transform);
                
                if (myCamera.GetLeftBoundpost() == transform)
                {
                    myCamera.SetLeftBoundpost(null);
                }
            } else
            {
                //Become the left boundpost
                myCamera.SetLeftBoundpost(transform);
                
                if (myCamera.GetRightBoundpost() == transform)
                {
                    myCamera.SetRightBoundpost(null);
                }
            }
        }
    }
}
