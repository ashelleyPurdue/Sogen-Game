using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LevelEntranceBehavior))]

public class CheckpointBehavior : MonoBehaviour 
{
    //CURRENT TASK: Making the player return to the previous checkpoint when dieing

    public string courseName;
    public string checkpointName;
 
    public CheckpointFlameAnimation flame;
    
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
    
    void Update()
    {
        //Show/hide the falme
        flame.visible = checkpoint.Activated;
    }
    
    public void OnTriggerStay2D(Collider2D other)
    {
        //Activate the checkpoint when the player touches it.
        if (TagList.ObjectHasTag(other, "Player"))
        {
            if (!checkpoint.Activated)
            {
                Activate();
            }
        }
    }
    
    //Misc methods
    
    private void Activate()
    {
        //Activate the checkpoint
        checkpoint.Activate();
        CourseManager.SetActiveCheckpoint(checkpoint);
        
        //Refill the player's health
        HealthPoints playerHP = TagList.FindOnlyObjectWithTag("Player").GetComponent<HealthPoints>();
        playerHP.SetHealth(playerHP.maxHealth);
        
        //Create the text
        Vector3 pos = GetComponent<LevelEntranceBehavior>().dropOffPoint.position;
        EffectManager.Instance.TextFade("checkpoint_text", pos, 0.5f, 2, 0.25f, 0.25f, 4);
    }
}
