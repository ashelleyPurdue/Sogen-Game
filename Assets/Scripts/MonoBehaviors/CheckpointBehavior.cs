using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LevelEntranceBehavior))]

public class CheckpointBehavior : MonoBehaviour 
{
    public string courseName;
    public string checkpointName;
    
    public CheckpointFlameAnimation flame;
    
    private Checkpoint checkpoint;

    //Events

    public void Start()
    {
        CheckNamesFilled();
        
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
   
            //Get the checkpoint that we just created.
            checkpoint = CourseManager.GetCheckpoint(courseName, checkpointName);
        }
        
        //Check to make sure everything mathces.
        CheckNamesMatch();
    }
    
    void Update()
    {
        //Show/hide the falme
        if (flame != null)
        {
            flame.visible = checkpoint.Activated;
        }
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
    
    private void CheckNamesMatch()
    {
        //Throws an error if the names in the checkpoint system don't match the names here.
        
        string nameStr = "Checkpoint named " + checkpointName + " ";
        string idStr = "\nInstance id: " + GetInstanceID();
        
        //Check scene name
        if ( !checkpoint.SceneName.Equals( LevelPersistence.GetCurrentLevelName() ) )
        {
            Debug.LogError(nameStr + "already exists in another scene.  Please use a different name." + idStr);
            Application.Quit();
        }
        
        //Check course name
        if ( !checkpoint.CourseName.Equals(courseName) )
        {
            Debug.LogError(nameStr + "is already registered to course \"" + checkpoint.CourseName + "\".\nThere must be a duplicate checkpoint." + idStr);
            Application.Quit();
        }
        
        //Check for entrance name.
        string registeredEntranceName = GetComponent<LevelEntranceBehavior>().entranceName;
        if ( !checkpoint.EntranceName.Equals( registeredEntranceName ) )
        { 
            Debug.LogError(nameStr + "is already registered with entrance name \"" + registeredEntranceName + "\"." + idStr);
            Application.Quit();
        }
    }
    
    private void CheckNamesFilled()
    {
        //Throw an error if the course name doesn't exist
        if (courseName.Equals(""))
        {
            Debug.LogError("Checkpoint has blank course name.");
            Application.Quit();
        }
        
        //Throw an error if the checkpoint name doesn't exist.
        if (checkpointName.Equals(""))
        {
            Debug.LogError("Checkpoint has blank name.");
            Application.Quit();
        }
    }
    
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
