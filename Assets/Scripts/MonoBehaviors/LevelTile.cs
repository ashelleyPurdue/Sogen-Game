using UnityEngine;
using System.Collections;

public class LevelTile : MonoBehaviour 
{
    public string courseName;
    public string sceneName;


    public void Update()
    {
       // Debug.Log("Update");
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        //If a button is pressed and the level selector is on it, then go to the level.

        //Debug.Log("Trigger stay.");

        if (Input.GetButton("Jump"))
        {
            if (other.GetComponent<LevelSelectorBehavior>() != null)
            {
                CourseManager.StartCourse(courseName, sceneName, 0);
            }
        }
    }
}
