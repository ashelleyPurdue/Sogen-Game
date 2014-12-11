using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LevelEntranceBehavior))]

public class CheckpointBehavior : MonoBehaviour 
{
    //CURRENT TASK: Making the player return to the previous checkpoint when dieing

    public string courseName;
    public string checkpointName;

    private Checkpoint checkpoint;

    //Events

    public void Start()
    {
        //Get the checkpoint

        try
        {
            checkpoint = CourseManager.GetCheckpoint(courseName, checkpointName);
        }
        catch (CheckpointNonexistantException e)
        {
            //Create the checkpoint if it does not exist

            LevelEntranceBehavior entrance = GetComponent<LevelEntranceBehavior>();
            int entranceNumber = entrance.entranceNumber;

            CourseManager.AddCheckpoint(new Checkpoint(courseName, checkpointName, LevelPersistence.GetCurrentLevelName(), entranceNumber));

            checkpoint = CourseManager.GetCheckpoint(courseName, checkpointName);
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        //Activate the checkpoint when the player touches it.
        if (TagList.ObjectHasTag(other, "Player"))
        {
            checkpoint.Activate();
            CourseManager.SetActiveCheckpoint(checkpoint);
        }
    }
}
