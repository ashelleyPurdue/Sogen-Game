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
            string entranceName = entrance.entranceName;

            CourseManager.AddCheckpoint(new Checkpoint(courseName, checkpointName, LevelPersistence.GetCurrentLevelName(), entranceName));

            checkpoint = CourseManager.GetCheckpoint(courseName, checkpointName);
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        //Activate the checkpoint when the player touches it.
        if (TagList.ObjectHasTag(other, "Player"))
        {
            if (!checkpoint.Activated)
            {
                checkpoint.Activate();
                CourseManager.SetActiveCheckpoint(checkpoint);
                
                //Create the text
                Vector3 pos = GetComponent<LevelEntranceBehavior>().dropOffPoint.position;
                EffectManager.Instance.TextFade("checkpoint_text", pos, 3, 2, 0.25f, 0.25f, 4);
            }
        }
    }
}
