using UnityEngine;
using System.Collections;

public class CameraGrabberBehavior : MonoBehaviour
{
    private SidescrollerCameraBehavior cam;
    
    void Start()
    {
        cam = TagList.FindOnlyObjectWithTag("camera").GetComponent<SidescrollerCameraBehavior>();
    }
    
    void Update()
    {
        Bounds myBounds = new Bounds(transform.position, Vector3.one);
        if (cam.InView(myBounds))
        {
            cam.LockInPlace(transform.position.x);
        }
    }
}
