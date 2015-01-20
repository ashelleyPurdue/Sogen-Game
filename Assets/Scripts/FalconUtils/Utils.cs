using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Contains lots of miscellaneous/reusable funcitons

public static class Utils
{
	public enum Axes {x, y, z};

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
	public static Vector2 CapMagnitude(Vector2 inputVector, float maxMagnitude, float minMagnitude = 0)
    {
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
		while (output > maxValue){
			output -= maxValue - minValue;
		}
		while (output < minValue){
			output += maxValue - minValue;
		}
		
		return output;
		
	}
	
	//---Tween Value---
    //DO NOT USE ANYMORE.  USE Math.Lerp INSTEAD
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
	public static Vector3 SetVector(Vector3 inputVector, float? x = null, float? y = null, float? z = null){
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
	
	public static void SetVector(ref Vector3 inputVector, float? x = null, float? y = null, float? z = null){
		//Sets the coordinates of a vector.  Use "null" to indicate that a coordinate should not be changed.	
		
		inputVector = SetVector(inputVector, x, y, z);
	}

    //---Guess Value---
    public delegate bool ConditionMethod(float number);     //this method returns true if the given number satisfies the condition
    public static float GuessValue(float startingValue, ConditionMethod SatisfiesCondition, float[] increments, bool undershoot = true, int maxIterations = 1000)
    {
        //Returns the lowest value that satisfies the given condition method
        /*
         * startingValue: the value that the algorithm will start on 
         * Compare: a delegate pointing to a method that returns if a given value is too high, too low, or just right.
         * increments: An array of increments.  These should get progressively smaller
         */
        float output = startingValue;

        //Use every increment in the list to get increasingly more precise
        for (int i = 0; i < increments.Length; i++)
        {
            //Get closer using the new increment
            output = GuessValue(output, SatisfiesCondition, increments[i], true, maxIterations);
        }

        //If undershoot is false, then make sure the output "goes over"
        if (!undershoot)
        {
            output += increments [increments.Length - 1];
        }

        return output;
    }

    public static float GuessValue(float startingValue, ConditionMethod SatisfiesCondition, float increment, bool undershoot, int maxIterations = 1000)
    {
        //Returns a value that is very close to making SatisfiesCondition true.
        //If undershoot is true, then the answer will be slightly "too small" and SatsifiesCondition will return false if this value is used.
        //If undershoot is false, then answer will be slightly "too big" and SatisfiesCondition will return true if this value is used.
        float output = startingValue;

        int iterations = 0;

        while (iterations < maxIterations)
        {
            iterations++;

            output += increment;

            if (SatisfiesCondition(output))
            {
                if (undershoot)
                {
                    output -= increment;
                }
                return output;
            }
        }

        //If too many iterations have happened, return the output.
        throw new GuessValueMaxIterationException();
        return output;
    }
}

//Exceptions
public class AxisNotInVectorException : System.Exception{
	
	public Vector2 vectorRetrieved;
	public Utils.Axes axisNotFound;
	
	public AxisNotInVectorException(Vector2 vect, Utils.Axes ax){
	
		vectorRetrieved = vect;
		axisNotFound = ax;
	}
	
}

public class GuessValueMaxIterationException : System.Exception
{
    public GuessValueMaxIterationException() : base("GuessValue has exceeded max iterations.")
    {
    }
}