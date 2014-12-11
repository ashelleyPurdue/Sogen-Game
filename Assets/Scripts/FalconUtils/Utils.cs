using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Contains lots of miscellaneous/reusable funcitons

public static class Utils{
	
	public enum Axes {x, y, z};

	[System.Obsolete("Please use the RotationUtils class instead.")]
	public static class Rotation
	{
		
		//Angle distance
		
		public static float AngleDistance(float a, float b, bool alwaysPositive = true)
		{
			//Returns the angle distance between two angles
			//Code stolen and modified from ThirdPersonCamera.js(comes with Unity)
		
			a = Mathf.Repeat(a, 360);
			b = Mathf.Repeat(b, 360);
			
			float output = b - a;
			
			if (alwaysPositive == true)
			{
				output = Mathf.Abs(output);
			}
			
			return output;
		}
		
		//Angle Towards Point
		public static float AngleTowardsPoint2D(Vector2 pointA, Vector2 pointB)
		{
			//Returns the angle towards a point in 2D space in degrees.
			
			float yDist = pointB.y - pointA.y;
			float xDist =  pointB.x - pointA.x;
			
			float output = Mathf.Rad2Deg * Mathf.Atan2(xDist * -1, yDist * -1);
			
			return output;
			
		}
		
		public static float AngleTowardsPoint2D(Vector3 pointA, Vector3 pointB)
		{
		
			return AngleTowardsPoint2D(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));
			
		}
	
		//LookAtFlat
		public static Vector3 LookAtFlat(Vector3 startPos, Vector3 endPos)
		{
			//Creates a normalized vector that points from startPos to endPos, ignoring the y-components
			Vector3 output = endPos - startPos;
			
			output.y = 0;
			output.Normalize();
			
			return output;
		}
			
			
		
		//ChangeEuler
		public static void ChangeEuler(ref Quaternion rotationToChange, float? x, float? y, float? z)
		{
			//Changes the euler angles of a given quaternion.  If you want an angle to stay the same, use null instead of a value.
			//Example: changes the x and z values, but not the y: ChangeEuler(transform.rotationToChange, 90, null, 180);
			
			Quaternion myRot = rotationToChange;
			Vector3 myEuler = myRot.eulerAngles;
			
			if (x != null){
				myEuler.x = (float)x;
			}
			
			if (y != null){
				myEuler.y = (float)y;
			}
			
			if (z != null){
				myEuler.z = (float)z;
			}
			
			myRot.eulerAngles = myEuler;
			rotationToChange = myRot;
			
		}	
	
		public static void ChangeEuler(ref Quaternion rotationToChange, Vector3 newEuler){
			ChangeEuler(rotationToChange, newEuler.x, newEuler.y, newEuler.z);
		}
		
		public static void ChangeEuler(Transform transformToChange, float? x, float? y, float? z, bool useLocalRotation = false){
		
			Quaternion myRot;
			
			//Store the rotation in a temp variable
			if (useLocalRotation == true){
				myRot = transformToChange.localRotation;
			}
			else{
				myRot = transformToChange.rotation;
			}
			
			//Use ChangeEuler on that variable
			ChangeEuler(ref myRot, x, y, z);
			
			//Apply the new rotation to the tranfrom
			if (useLocalRotation == true){
				transformToChange.localRotation = myRot;
			}
			else{
				transformToChange.rotation = myRot;
			}
			
		}
		
		public static void ChangeEuler(Transform transformToChange, Vector3 newEuler, bool useLocalRotation){
		
			ChangeEuler(transformToChange, newEuler.x, newEuler.y, newEuler.z, useLocalRotation);
		}
		
		public static void ChangeEuler(Rigidbody rigidBodyToChange, float? x, float? y, float? z){
			//Rather than directly changing the transform, this version will instead use a rigidbody's MoveRotation function.
			
			Quaternion myRot = rigidBodyToChange.rotation;
			ChangeEuler(ref myRot, x, y, z);
			
			rigidBodyToChange.MoveRotation(myRot);
			
		}
		
		public static Quaternion ChangeEuler(Quaternion inputQuaternion, float? x, float? y, float? z){
			
			Quaternion output = inputQuaternion;
			ChangeEuler(ref output, x, y, z);
			
			return output;
		}
		
		public static Quaternion ChangeEuler(Quaternion inputQuaternion, Vector3 newEuler){
			
			Quaternion output = inputQuaternion;
			ChangeEuler(ref output, newEuler);
			
			return output;
		}

	}

	[System.Obsolete("Please use the CollisionUtils class instead.")]
	public static class CollisionChecking
	{
	
		//TagSpherecast
		
		public static bool TagSpherecast(float radius, Vector3 point, string tagToFind, ref Transform transformFound)
		{
			//Spherecasts and checks if an object has been found with a specified tag
			
			Collider[] colliderList = Physics.OverlapSphere(point, radius);
			bool output = false;
				
			for (int i = 0; i < colliderList.Length; i++)
			{
				
				Collider col = colliderList[i];
				
				TagList tags = col.transform.GetComponent<TagList>();
				
				if (tags != null && tags.HasTag(tagToFind))
				{
					output = true;
					
					//Store a reference to the found object in the specified variable.
					//Reference-ception!
					transformFound = col.transform;
						
					break;
				}
					
				else
				{
					//Make sure transformFound always gets assigned to something
					transformFound = null;
				}
			
			}	
			
			//Return the output
			return output;
		}
		
		public static bool TagSpherecast(float radius, Vector3 point, string tagToFind)
		{
			//Checks to see if a player has been found within a certain radius of a given point.
			
			Transform dummyTransform = null;
			
			return TagSpherecast(radius, point, tagToFind, ref dummyTransform);
		}
		
		//SpherecastForObject
		public static bool SpherecastForObject(float radius, Vector3 point, Transform objectToCheck)
		{
			//Returns true if the specified object was found in a spherecast
			
			bool output = false;
			
			Collider[] collidingObjects = Physics.OverlapSphere(point, radius);
			
			foreach (Collider collider in collidingObjects)
			{
				if (collider.transform == objectToCheck)
				{
					output = true;
					break;
				}
			}
			
			return output;
		}
	
	}

	[System.Obsolete("Please use the ListUtils class instead.")]
	public static class Lists{
		
		public static bool ListsShareItem<T>(List<T> listOne, List<T> listTwo, T item){
		//Returns whether or not a given tag can be found in both lists
			
			//return HasDamageTag(listOne, item) && HasDamageTag(listTwo, item);	
			return listOne.Contains(item) && listTwo.Contains(item);
		}

		public static bool ListsOverlap<T>(List<T> listOne, List<T> listTwo){
			//Returns whether or not two lists share any tags	
			
			bool output = false;
			
			//Loop through each tag in listOne and see if it shares that tag with listTwo
			foreach (T item in listOne){
			
				if (ListsShareItem(listOne, listTwo, item)){
					output = true;
					break;
				}
				
			}
			
			return output;
		}
		
		public static List<T> RemoveDuplicateItems<T>(List<T> inputList){
			//Removes all duplicate items from a list.
	
			List<T> outputList = new List<T>();
			
			//Loop through the inputList and add each item to the output list if it hasn't already been added.
			foreach (T item in inputList)
			{
			
				if (!outputList.Contains(item))
				{
					outputList.Add(item);
				}
				
			}
			
			return outputList;
		}
		
	}
	
	//---Cap value---
	public static float CapValue(float inputValue, float maxValue, float minValue = 0){
		//Keeps a number in between two values
		
		float output = inputValue;
		
		if (output > maxValue){
			output = maxValue;
		}
		
		if (output < minValue){
			output = minValue;
		}
		
		return output;
	}
	
	public static int CapValue(int inputValue, int maxValue, int minValue = 0){
		return (int)CapValue((float)inputValue, (float)maxValue, (float)minValue);
	}
	
	public static double CapValue(double inputValue, double maxValue, double minValue = 0){
	
		return (double)CapValue((float)inputValue, (float)maxValue, (float)minValue);
	}
	
	public static Vector2 CapValue(Vector2 inputValue, Vector2 maxValue, Vector2 minValue){
		//Caps the individual components of a vector2.  To cap the magnitude, use CapMagnitude
		
		Vector2 outputValue = new Vector2(0, 0);

		outputValue.x = CapValue(inputValue.x, maxValue.x, minValue.x);
		outputValue.y = CapValue(inputValue.y, maxValue.y, minValue.y);
			
		return outputValue;
	}
	
	
	//---Cap magnitude---
	public static Vector2 CapMagnitude(Vector2 inputVector, float maxMagnitude, float minMagnitude = 0){
		//Keeps the magnitude of a vector within the specified min and max range.
		
		Vector2 outputVector = inputVector;
		
		if (outputVector.sqrMagnitude > maxMagnitude * maxMagnitude){
			outputVector.Normalize();
			outputVector *= maxMagnitude;
		}
		
		if (outputVector.sqrMagnitude < minMagnitude * minMagnitude){
			outputVector.Normalize();
			outputVector *= minMagnitude;
		}
		
		return outputVector;
	}
	
	
	//---Repeat value---
	public static float RepeatValue(float inputValue, float maxValue, float minValue){
		//Just like Mathf.Repeat, but tries to keep it between two given values, including negative numbers
		
		float output = inputValue;
		
		//Make sure the min is less than the max
		if (minValue > maxValue){
			
			float holder = minValue;
			
			minValue = maxValue;
			maxValue = holder;
		}
		
		//keep adding or subtracting maxValue to the number until it's within the range.
		/*while (!( output <= maxValue && output >= minValue)){
			
			float start = output;
			
			if (output > maxValue){
				output -= maxValue - minValue;
			}
			
			if (output < minValue){
				output += maxValue - minValue;
			}
			
		}*/
		
		while (output > maxValue){
			output -= maxValue - minValue;
		}
		while (output < minValue){
			output += maxValue - minValue;
		}
		
		return output;
		
	}
	
	
	//---Tween Value---
	public static float TweenValue(float inputValue, float targetValue, float speed){
		//Tweens a variable towards a value, given a current value, a target value, and a speed.
		//USAGE: variableToTween = Utils.TweenValue(variableToTween, targetValue, speed); 
		
		float output = inputValue;
			
		//Shift everything so that it's as if the target were zero.
		output -= targetValue;

		//If it's close enough to zero, then just set it to zero.
		if (Mathf.Abs(output) - speed < 0){
    		output = 0;
    		}
		else{
    		output = (Mathf.Abs(output) - speed) * Mathf.Sign(output);
    		}
    
		//Shift val back up to whatever target was before.
		output += targetValue;
		
		return output;
		
	}
	
	public static float TweenValue(float startValue, float endValue, float timeToTake, float timeAlreadyTaken){
		//Tweens a variable towards a value, given the starting value, ending value, the time to take, and the time already taken.
		//USAGE: variableToTween = Utils.TweenValue(startValue, endValue, timeToTake, timeAlreadyTaken);
		
		float percentComplete = timeAlreadyTaken / timeToTake;
		float difference = endValue - startValue;
		
		//Make sure the percent complete doesn't go over 1.00
		if (percentComplete > 1){
			percentComplete = 1;
		}

		float output = startValue + (percentComplete * difference);
		
		return output;
		
	}

	
	//PingPongPlus
	
	public static float PingPongPlus(float bottomValue, float topValue, float currentTime, float maxTime){
		//Bounces back and forth between two values over time.  currentTime is the current time.  maxTime is the time it should take for one "ping pong" to happen.
		
		float output = (Mathf.PingPong(currentTime, maxTime) * (topValue - bottomValue)) + bottomValue;
		
		return output;
	}
	
	//---To Vector2---
	public static Vector2 ToVector2(Vector3 inputVector){
		//Takes a Vector3 and turns it into a vector2, where x = x and y = z.
		
		return new Vector2(inputVector.x, inputVector.z);
	}
	
	
	//---To Vector3---
	public static Vector3 ToVector3(Vector2 inputVector){
		//Takes a Vector2 and turns it into a vector3, where x = x and y = z.
		
		return new Vector3(inputVector.x, 0f, inputVector.y);
	}
	
	
	//---GetCoordinate---
	
	public static float GetCoordinate(Vector3 inputVector, Axes coordinateToFind){
		//Returns a given coordinate of a Vector	
		
		float output = 0f;
		
		switch (coordinateToFind){
			
			case Axes.x:
				output = inputVector.x;
				break;
				
			case Axes.y:
				output = inputVector.y;
				break;
				
			case Axes.z:
				output = inputVector.z;
				break;
		}
		
		return output;
	}
	
	public static float GetCoordinate(Vector2 inputVector, Axes coordinateToFind){
		//Returns a given coordinate of a vector
		
		float output;
		
		if (coordinateToFind == Axes.z){
			
			throw new AxisNotInVectorException(inputVector, coordinateToFind);
		}
		else{
			
			output = GetCoordinate(new Vector3(inputVector.x, inputVector.y, 0), coordinateToFind);
		}
		
		return output;
		
	}
	
	
	//---SetVector---
	public static Vector3 SetVector(Vector3 inputVector, float? x, float? y, float? z){
		//Sets the coordinates of a vector.  Use "null" to indicate that a coordinate should not be changed.	
		
		Vector3 outputVector = inputVector;
		
		if (x != null){
			outputVector.x = (float)x;
		}
		
		if (y != null){
			outputVector.y = (float)y;
		}
		
		if (z != null){
			outputVector.z = (float)z;
		}
		
		return outputVector;
	}
	
	public static void SetVector(ref Vector3 inputVector, float? x, float? y, float? z){
		//Sets the coordinates of a vector.  Use "null" to indicate that a coordinate should not be changed.	
		
		inputVector = SetVector(inputVector, x, y, z);
	}


	//---Weighted Choice---

}

public class AxisNotInVectorException : System.Exception{
	
	public Vector2 vectorRetrieved;
	public Utils.Axes axisNotFound;
	
	public AxisNotInVectorException(Vector2 vect, Utils.Axes ax){
	
		vectorRetrieved = vect;
		axisNotFound = ax;
	}
	
}