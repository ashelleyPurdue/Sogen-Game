using UnityEngine;
using System.Collections;

public class LevelGoalBehavior : MonoBehaviour
{
    public const string LEVEL_SELECT_SCENE_NAME = "LevelSelectScene";

    public string courseName;   //The name of the course that will be completed when activated
    public int entranceNumber;  //The entrance to use when entering the level select screen.

    public void OnTriggerEnter2D(Collider2D other)
    {
        //If colliding with the player, complete the course and return to the level select screen.

        if (TagList.ObjectHasTag(other, "Player"))
        {
            CourseManager.CompleteCourse();
        }
    }
}
