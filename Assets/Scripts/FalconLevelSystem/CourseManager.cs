using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public static class CourseManager
{
    private static Course activeCourse = null;          //The course that is currently being played.
    private static Checkpoint activeCheckpoint = null;  //The checkpoint that the player last reached.

    private static Dictionary<string, Course> courses = new Dictionary<string, Course>();

    private static List<Checkpoint> allCheckpoints = new List<Checkpoint>();


    public static void StartCourse(string courseName, string scene, string entranceName)
    {
        //Starts the given course and moves to the given scene with the given entrance number

        //Throw an exception if we're already in a course
        if (IsPlayingCourse())
        {
            throw new AlreadyInCourseException();
        }

        //Mark the active course
        activeCourse = GetCourse(courseName);

        //Clear the active checkpoint
        ClearActiveCheckpoint();

        //Go to the scene
        LevelPersistence.ChangeLevel(scene, entranceName);
    }

    public static void StartCourse(string courseName, Checkpoint checkpoint)
    {
        //Starts playing the given course at the given checkpoint

        //Throw an error if we're already in a course
        if (IsPlayingCourse())
        {
            throw new AlreadyInCourseException();
        }

        //Throw an error if the checkpoint doesn't belong to the given course
        if (!checkpoint.CourseName.Equals(courseName))
        {
            throw new CheckpointNotInCourseException();
        }

        //Throw an error if the checkpoint isn't activated
        if (!checkpoint.Activated)
        {
            throw new CheckpointNotActiveException();
        }

        //Mark the active course
        activeCourse = GetCourse(courseName);

        //Mark the active checkpoint
        SetActiveCheckpoint(checkpoint);

        //Return to the checkpoint
        checkpoint.ReturnTo();
    }

    public static void SetActiveCheckpoint(string courseName, string checkpointName)
    {
        //Sets the currently active checkpoint
        SetActiveCheckpoint(GetCheckpoint(courseName, checkpointName));
    }

    public static void SetActiveCheckpoint(Checkpoint c)
    {
        //Sets the currently active checkpoint
        activeCheckpoint = c;
    }

    public static void ClearActiveCheckpoint()
    {
        //Clears the currently active checkpoint
        activeCheckpoint = null;
    }

    public static void ReturnToActiveCheckpoint()
    {
        //Returns to the currently active checkpoint.  You must currently be playing a course.

        //Throw an error if there is no active checkpoint.
        if (activeCheckpoint == null)
        {
            throw new NoActiveCheckpointException();
        }

        //Return to the checkpoint
        activeCheckpoint.ReturnTo();
    }

    public static void AddCourse(Course course)
    {
        //Adds a course to the dictionary

        if (!courses.ContainsValue(course))
        {
            courses.Add(course.Name, course);
        }
    }

    public static void CompleteCourse()
    {
        //Marks the given course as completed and returns to the level select screen
  
        Debug.Log("Current course: " + activeCourse.Name);
        
        //Throw an error if we're not currently in a course.
        if (!IsPlayingCourse())
        {
            throw new NoActiveCourseException();
        }

        //Complete the course and return to the level select screen
        ExitCourse();

        activeCourse.Complete();
        activeCourse = null;
    }

    public static void ExitCourse()
    {
        //Exits the currently active course and returns to the level select screen

        //Throw an error if we're not currently in a course
        if (!IsPlayingCourse())
        {
            throw new NoActiveCourseException();
        }

        //Clear the active checkpoint
        ClearActiveCheckpoint();

        //Return to the level select screen
        LevelPersistence.ChangeLevel("LevelSelectScene");
    }

    public static void AddCheckpoint(Checkpoint c)
    {
        //Adds a checkpoint to the system.
        
        allCheckpoints.Add(c);
        
        //Add the checkpoint to its course.
        Course course = CourseManager.GetCourse(c.CourseName);
        course.AddCheckpoint(c);
    }

    private static Course GetCourse(string name)
    {
        //Returns the course with the given name

        //If the course doesn't exist, create it
        if (!courses.ContainsKey(name))
        {
            courses.Add(name, new Course(name));
        }

        return courses[name];
    }

    public static bool IsPlayingCourse()
    {
        //Returns if the we are currently playing a course.
        return activeCourse != null;
    }

    public static bool IsCompleted(string name)
    {
        //Returns if a given course is completed, or false if the course doesn't exist

        if (!courses.ContainsKey(name))
        {
            return false;
        } else
        {
            return GetCourse(name).Completed;
        }
    }

    public static Checkpoint GetCheckpoint(string courseName, string checkpointName)
    {
        //Returns the checkpoint in the given course with the given name

        return GetCourse(courseName).GetCheckpoint(checkpointName);
    }

    public static List<Checkpoint> GetActivatedCheckpoints(string courseName)
    {
        //Returns a list of all checkpoints in the given course.
        
        Course course = GetCourse(courseName);
        return course.GetActivatedCheckpoints();
    }

    public static string GenerateCourseSaveString()
    {
        return "";
    }

    public static string GenerateCheckpointSaveString()
    {
        return "";
    }

    public static void LoadCourseSaveString(string saveString)
    {
        //TODO: Load stuff
    }

    public static void LoadCheckpointSaveString()
    {
        //TODO: Reconstruct all the checkpoints from the list
    }


}

//Exceptions
public class AlreadyInCourseException : System.Exception {}
public class NoActiveCourseException: System.Exception {}

public class NoActiveCheckpointException : System.Exception {}
public class CheckpointNonexistantException : System.Exception {}
public class CheckpointNotInCourseException : System.Exception {}
public class CheckpointNotActiveException : System.Exception {}
