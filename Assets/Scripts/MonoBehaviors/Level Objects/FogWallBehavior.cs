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

    void OnWillRenderObject()
    {
        //When the object is rendered, become the boundpost for the camera.

        if (player.position.x < transform.position.x)
        {
            //Become the right boundpost
            myCamera.SetBoundposts(null, transform);
        } else
        {
            //Become the left boundpost
            myCamera.SetBoundposts(transform, null);
        }
    }
}
