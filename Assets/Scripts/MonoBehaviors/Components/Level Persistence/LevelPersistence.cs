using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;

public static class LevelPersistence
{
	public static string entranceUsed = null; //The entrance that the player used to get to this level.
	
	private static Dictionary<string, LevelData> levelList = new Dictionary<string, LevelData>();
	
	static LevelPersistence()
	{
		
		//Add the level that the game starts in
		AddLevel(Application.loadedLevelName);
	}
	
	//Interface
	
	public static void ChangeLevel(string levelName)
	{
		//Loads a new level and broadcasts the proper events to all GameObjects

		//Broadcast the OnLevelEnd event.
		BroadcastToAllObjects("OnLevelEnd");

		//Load the level
		Application.LoadLevel(levelName);
		
		//Create a new entry in the level list if one does not already exist.
		AddLevel(levelName);
	}

    public static void ChangeLevel(string levelName, string entranceName)
    {
        //Loads a new level in at a particular entrance
    
        entranceUsed = entranceName;
        ChangeLevel(levelName);
    }

	public static void SaveToFile(string fileName)
	{
		//Saves all persistent data to a text file.

		string textToSave = GenerateSaveString();

		StreamWriter writer = File.CreateText(fileName);
		writer.Write(textToSave);

		writer.Close();
		writer.Dispose();

		Debug.Log(textToSave);
	}

	public static void LoadFromFile(string fileName)
	{
		//Loads from a given file

		//Clear the levelList dictionary
		levelList.Clear();

		//Load an array of command lines from fileName
		string[] lines = ReadLinesFromFile(fileName);

		//Execute those lines
		DataLoaderVM.ExecuteLines(lines);

		Debug.Log("Loaded " + fileName);
	}

	public static string GetCurrentLevelName()
	{
		//Returns the name of the current level.
		return Application.loadedLevelName;
	}

	public static LevelData GetLevel(string levelName)
	{
		//Returns a leveldata with the specified levelName.  Throws an error if it doesn't exist.
		
		LevelData output;
		
		if (LevelExists(levelName)){
			
			output = levelList[levelName];
		}
		else{
			
			throw new LevelNotFound(levelName);
		}
		
		return output;
	}

	public static ObjectData GetObject(int idToFind, string levelName = null){
		//Gets an object in a specified level.  Leave the second parameter blank to use the current level
		
		string name = DefaultLevelName(levelName);
		
		return GetLevel(name).GetObject(idToFind);
	}
	
	public static ObjectData AddObject(int idToAdd, string levelName = null){
		//Adds an object(if it doesn't already exist) to a given level.  Leave level name blank to use current level.	
		
		string name = DefaultLevelName(levelName);
		
		return GetLevel(name).AddObject(idToAdd);
	}
	
	public static bool ObjectExists(int idToFind, string levelName = null){
		//Returns whether or not a given object from a given level exists.

		string name = DefaultLevelName(levelName);
		
		return GetLevel(name).IDExists(idToFind);
		
	}
	
	public static bool LevelExists(string levelToCheck){
		//Returns whether or not a level already exists in the levelList.  Uses the current level name if left blank
		
		string name = DefaultLevelName(levelToCheck);
		
		return levelList.ContainsKey(name);
	}

	
	//Utility functions
	public static void BroadcastToAllObjects(string message){
		//Broadcasts a message to all game objects
		
		Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));
		
		for (int i = 0; i < objects.Length; i++){
		
			GameObject gameObj = (GameObject)objects[i];
			
			gameObj.transform.BroadcastMessage(message, SendMessageOptions.DontRequireReceiver);
			
		}
		
	}
	
	//Misc functions
	public static LevelData AddLevel(string levelName){
		//Adds a new level (if it doesn't already exist) and returns it.
		
		LevelData newLevel;
		
		if (LevelExists(levelName)){
			
			newLevel = GetLevel(levelName);
		}
		else{
			
			newLevel = new LevelData(levelName);
			levelList.Add(levelName, newLevel);
		}
		
		return newLevel;
	}
	
	private static string DefaultLevelName(string levelName){
		//Changes a level name to the current level's name if it is null.
		
		string name;
		
		if (levelName == null){
			
			name = Application.loadedLevelName;
		}
		else{
			
			name = levelName;
		}
		
		return name;
	}

	private static string[] ReadLinesFromFile(string fileName)
	{
		//Read the given text file and return an array of all of the lines in said text file.

		StreamReader reader = File.OpenText(fileName);

		List<string> lines = new List<string>();

		while (!reader.EndOfStream)
		{
			string line = reader.ReadLine();
			lines.Add(line);
		}

		reader.Close();
		reader.Dispose();

		return lines.ToArray();
	}

	//Saving/loading functions

	private static string GenerateSaveString()
	{
		//Returns a string representation of all persistent data

		string output = "";

		foreach (KeyValuePair<string, LevelData> levelPair in levelList)
		{
			//TODO: Append the level's save string

			output += levelPair.Value.GenerateSaveString();
			output += "\n";
		}

		return output;
	}
}


public class LevelData
{
	//Contains all ObjectDatas for a level
	
	public readonly string levelName;
	
	private Dictionary<int, ObjectData> objectList = new Dictionary<int, ObjectData>();
	
	public LevelData(string name)
	{
		levelName = name;
	}

	public ObjectData GetObject(int idToFind)
	{
		//Gets an object in this level with a given ID
		
		ObjectData output;
		
		if (objectList.ContainsKey(idToFind))
		{
			
			output = objectList[idToFind];
		}
		else
		{
			
			throw new IDNotFound(idToFind, levelName);
		}
		
		return output;
	}
	
	public ObjectData AddObject(int idToAdd)
	{
		//Adds a new object to the object list (if its ID doesn't already exist) and returns it
		
		ObjectData newObject;
		
		//First, see if an object with a matching ID already exists.  If it does, then just return it.  If it doesn't, then create it.
		if (IDExists(idToAdd))
		{
			//Debug.Log("ID exists");

			newObject = GetObject(idToAdd);
		}
		else
		{
			//Debug.Log("ID doesn't exist");

			newObject = new ObjectData(idToAdd);
			objectList.Add(idToAdd, newObject);
		}
		
		return newObject;
	}
	
	public bool IDExists(int idToCheck)
	{
		//Returns whether or not an ID already exists
		
		return objectList.ContainsKey(idToCheck);
	}

	public string GenerateSaveString()
	{
		//Returns a string representation of this level.

		string output = "LEVEL " + levelName;
		output += "\n";

		foreach (KeyValuePair<int, ObjectData> objPair in objectList)
		{
			//Append this ObjectData's save string
			output += objPair.Value.GenerateSaveString();
			output += "\n";
		}

		return output;
	}
}


public class ObjectData
{
	//Contains any persistent data for an object
	
	//TODO: If needed, add an integer dictionary.
	//However, you probably won't need an integer greater than 255, so try to stick with bytes if you can.
	private Dictionary<string, string> stringData = new Dictionary<string, string>();
	private Dictionary<string, float> floatData = new Dictionary<string, float>();
	private Dictionary<string, byte> byteData = new Dictionary<string, byte>();
	private Dictionary<string, bool> boolData = new Dictionary<string, bool>();
	
	private bool isBlank = true;
	
	public readonly int objectID;
	
	public ObjectData(int id)
	{
		objectID = id;
	}
	
	//Interface
	public void SetStringValue(string key, string val)
	{
		stringData[key] = val;
		isBlank = false;
	}
	
	public void SetFloatValue(string key, float val)
	{
		floatData[key] = val;
		isBlank = false;
	}
	
	public void SetByteValue(string key, byte val)
	{
		byteData[key] = val;
		isBlank = false;
	}
	
	public void SetBoolValue(string key, bool val)
	{
		boolData[key] = val;
		isBlank = false;
	}


	public string GetStringValue(string key)
	{
		return stringData[key];
	}
	
	public float GetFloatValue(string key)
	{
		return floatData[key];
	}
	
	public byte GetByteValue(string key)
	{
		return byteData[key];
	}
	
	public bool GetBoolValue(string key)
	{
		Debug.Log(key);
		return boolData[key];
	}


	public bool IsBlank()
	{
		//Returns whether or not this object has had data assigned to it yet.
		
		return isBlank;	
	}

	public string GenerateSaveString()
	{
		//Returns a string representation of this object

		string output = "OBJECT " + objectID + "\n";

		//Append all string data
		foreach (KeyValuePair<string, string> stringPair in stringData)
		{
			output += MakeLine ("STRING", stringPair.Key, stringPair.Value);
		}

		//Append all float data
		foreach (KeyValuePair<string, float> floatPair in floatData)
		{
			output += MakeLine ("FLOAT", floatPair.Key, floatPair.Value);
		}

		//Append all byte data
		foreach (KeyValuePair<string, byte> bytePair in byteData)
		{
			output += MakeLine("BYTE", bytePair.Key, bytePair.Value);
		}

		//Append all boolean data
		foreach (KeyValuePair<string, bool> boolPair in boolData)
		{
			output += MakeLine("BOOL", boolPair.Key, boolPair.Value);
		}

		//Return the output
		return output;
	}

	//Misc methods

	private string MakeLine(string command, string key, object value)
	{
		//Make a command line

		string output = "";

		output += command + " ";
		
		//output += "\"";
		output += key;
		//output += "\"";
		
		output += " ";
		
		//output += "\"";
		output += value;
		//output += "\"";
		
		output += "\n";

		return output;
	}
}


public static class DataLoaderVM
{
	private static string workingLevelName;
	private static ObjectData workingObject;

	public static void ExecuteLines(string[] linesToExecute)
	{
		//Executes all lines in the array
		
		for (int i = 0; i < linesToExecute.Length; i++)
		{
			ExecuteLine(linesToExecute[i]);
		}
	}
	
	private static void ExecuteLine(string line)
	{
		//Executes a line

		Debug.Log(line);

		//Break the line into 2 or 3 words
		string[] words = line.Split(' ');

		//Get the command
		string command = words[0];

		//Get the args
		string[] args = new string[words.Length - 1];

		for (int i = 0; i < args.Length; i++)
		{
			args[i] = words[i + 1];
		}

		//Execute the command
		switch (command)
		{
			case "LEVEL": 	LevelCommand(args); 	break;
			case "OBJECT": 	ObjectCommand(args); 	break;
			case "STRING": 	StringCommand(args); 	break;
			case "FLOAT": 	FloatCommand(args);	 	break;
			case "BYTE": 	ByteCommand(args); 		break;
			case "BOOL": 	BoolCommand(args); 		break;
		}
	}

	private static void LevelCommand(string[] args)
	{
		workingLevelName = args[0];
		LevelPersistence.AddLevel(args[0]);
	}

	private static void ObjectCommand(string[] args)
	{
		int id = int.Parse(args[0]);

		workingObject = LevelPersistence.AddObject(id, workingLevelName);
	}

	private static void StringCommand(string[] args)
	{
		workingObject.SetStringValue(args[0], args[1]);
	}

	private static void FloatCommand(string[] args)
	{
		string key = args[0];
		float value = float.Parse(args[1]);

		workingObject.SetFloatValue(key, value);
	}

	private static void ByteCommand(string[] args)
	{
		string key = args[0];
		byte value = byte.Parse(args[1]);
		
		workingObject.SetByteValue(key, value);
	}

	private static void BoolCommand(string[] args)
	{
		string key = args[0];
		bool value = bool.Parse(args[1]);
		
		workingObject.SetBoolValue(key, value);
	}

}


//Exceptions
public class IDNotFound : System.Exception{
	
	public int id;
	public string level;
	
	public IDNotFound(int ID, string lev){
		
		id = ID;
		level = lev;
		
		Debug.Log("ID: " + id + ". Level: " + level);
	}
	
}

public class LevelNotFound : System.Exception{
	
	public string name;
	
	public LevelNotFound(string nam){
		
		name = nam;
		Debug.Log("name: " + name);
	}
	
}