using UnityEngine;
using System.Collections;

public class LevelLockBehavior : MonoBehaviour
{
    public string courseName;

    //Events
    void Start()
    {
        //Destroy self if the course is complete
        if(CourseManager.IsCompleted(courseName))
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
