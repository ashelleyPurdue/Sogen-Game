using UnityEngine;
using System.Collections;

public class Checkpoint
{
    //Properties

    public string CourseName
    {
        get { return courseName;}
    }

    public string CheckpointName
    {
        get { return checkpointName;}
    }

    public string SceneName
    {
        get { return sceneName;}
    }

    public string EntranceName
    {
        get {return entranceName;}
    }

    public bool Activated
    {
        get {return activated;}
    }

    //Fields

    private string courseName;      //The name of the course that this checkpoint is in.
    private string checkpointName;  //The name of this checkpoint

    private string sceneName;       //The name of the scene that this checkpoint is in.
    private string entranceName;

    private bool activated = false;

    public Checkpoint(string courseName, string checkpointName, string sceneName, string entranceName)
    {
        this.courseName = courseName;
        this.checkpointName = checkpointName;
        this.sceneName = sceneName;
        this.entranceName = entranceName;

        //Add this checkpoint to the system
        CourseManager.AddCheckpoint(this);
    }


    //Interface

    public void Activate()
    {
        //Activates the checkpoint.  Note that the checkpoint cannot be "un-activated".
        activated = true;
        
        Debug.Log("Activated checkpoint " + courseName + "/" + checkpointName);
    }

    public void ReturnTo()
    {
        //Starts playing the level from this checkpoint
        LevelPersistence.ChangeLevel(sceneName, entranceName);
    }

    public string GenerateSaveString()
    {
        return "";
    }
}
