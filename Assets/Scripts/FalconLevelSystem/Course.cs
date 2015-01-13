using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class Course
{
    //Properties

    public string Name
    {
        get{return name;}
    }

    public bool Completed
    {
        get{return completed;}
    }

    //Fields

    private string name;
    private bool completed = false;

    private Dictionary<string, Checkpoint> checkpointDictionary = new Dictionary<string, Checkpoint>();

    public Course(string name)
    {
        this.name = name;
    }

    //Interface
    public void Complete()
    {
        //Marks this level as completed.  Note that a level cannot be "un-completed".
        completed = true;
    }

    public void AddCheckpoint(Checkpoint c)
    {
        //Adds a checkpoint to this course, if it isn't in there already.
        if (!checkpointDictionary.ContainsKey(c.CheckpointName))
        {
            checkpointDictionary.Add(c.CheckpointName, c);
        }
    }

    public Checkpoint GetCheckpoint(string checkpointName)
    {
        //Returns the checkpoint with the given name in this course

        //Throw an error if the checkpoint doesn't exist
        if (!checkpointDictionary.ContainsKey(checkpointName))
        {
            throw new CheckpointNonexistantException();
        }

        return checkpointDictionary [checkpointName];
    }

    public List<Checkpoint> GetAllCheckpoints()
    {
        //Returns a list of all checkpoints in the course

        List<Checkpoint> output = new List<Checkpoint>(checkpointDictionary.Values);
        return output;
    }

    public List<Checkpoint> GetActivatedCheckpoints()
    {
        //Returns a list of all activated checkpoints in the given course.
        
        List<Checkpoint> output = new List<Checkpoint>();
        
        foreach (Checkpoint c in checkpointDictionary.Values)
        {
            if (c.Activated)
            {
                output.Add(c);
            }
        }
        
        return output;
    }
}
