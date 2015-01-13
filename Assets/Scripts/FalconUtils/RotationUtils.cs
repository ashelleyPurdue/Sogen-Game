using UnityEngine;
using System.Collections;

public static class RotationUtils
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
		
		if (x != null)
		{
			myEuler.x = (float)x;
		}
		
		if (y != null)
		{
			myEuler.y = (float)y;
		}
		
		if (z != null)
		{
			myEuler.z = (float)z;
		}
		
		myRot.eulerAngles = myEuler;
		rotationToChange = myRot;
		
	}	
	
	public static void ChangeEuler(ref Quaternion rotationToChange, Vector3 newEuler)
	{
		ChangeEuler(rotationToChange, newEuler.x, newEuler.y, newEuler.z);
	}
	
	public static void ChangeEuler(Transform transformToChange, float? x, float? y, float? z, bool useLocalRotation = false)
	{
		
		Quaternion myRot;
		
		//Store the rotation in a temp variable
		if (useLocalRotation == true)
		{
			myRot = transformToChange.localRotation;
		}
		else
		{
			myRot = transformToChange.rotation;
		}
		
		//Use ChangeEuler on that variable
		ChangeEuler(ref myRot, x, y, z);
		
		//Apply the new rotation to the tranfrom
		if (useLocalRotation == true)
		{
			transformToChange.localRotation = myRot;
		}
		else
		{
			transformToChange.rotation = myRot;
		}
		
	}
	
	public static void ChangeEuler(Transform transformToChange, Vector3 newEuler, bool useLocalRotation)
	{
		
		ChangeEuler(transformToChange, newEuler.x, newEuler.y, newEuler.z, useLocalRotation);
	}
	
	public static void ChangeEuler(Rigidbody rigidBodyToChange, float? x, float? y, float? z){
		//Rather than directly changing the transform, this version will instead use a rigidbody's MoveRotation function.
		
		Quaternion myRot = rigidBodyToChange.rotation;
		ChangeEuler(ref myRot, x, y, z);
		
		rigidBodyToChange.MoveRotation(myRot);
		
	}
	
	public static Quaternion ChangeEuler(Quaternion inputQuaternion, float? x, float? y, float? z)
	{
		
		Quaternion output = inputQuaternion;
		ChangeEuler(ref output, x, y, z);
		
		return output;
	}
	
	public static Quaternion ChangeEuler(Quaternion inputQuaternion, Vector3 newEuler)
	{
		
		Quaternion output = inputQuaternion;
		ChangeEuler(ref output, newEuler);
		
		return output;
	}
}
