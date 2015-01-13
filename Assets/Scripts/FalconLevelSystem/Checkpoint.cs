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

    public int EntranceNumber
    {
        get {return entranceNumber;}
    }

    public bool Activated
    {
        get {return activated;}
    }

    //Fields

    private string courseName;      //The name of the course that this checkpoint is in.
    private string checkpointName;  //The name of this checkpoint

    private string sceneName;   //The name of the scene that this checkpoint is in.
    private int entranceNumber;

    private bool activated = false;

    public Checkpoint(string courseName, string checkpointName, string sceneName, int entranceNumber)
    {
        this.courseName = courseName;
        this.checkpointName = checkpointName;
        this.sceneName = sceneName;
        this.entranceNumber = entranceNumber;

        //Add this checkpoint to the system
        CourseManager.AddCheckpoint(this);
    }


    //Interface

    public void Activate()
    {
        //Activates the checkpoint.  Note that the checkpoint cannot be "un-activated".
        activated = true;
    }

    public void ReturnTo()
    {
        //Starts playing the level from this checkpoint
        LevelPersistence.ChangeLevel(sceneName, entranceNumber);
    }

    public string GenerateSaveString()
    {
        return "";
    }
}
