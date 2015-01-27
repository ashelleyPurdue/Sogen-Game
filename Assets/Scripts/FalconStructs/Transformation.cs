using UnityEngine;
using System.Collections;

public struct Transformation
{
	public Vector3 position;
	public Quaternion rotation;

	public Transformation(Vector3 position, Quaternion rotation)
	{
		this.position = position;
		this.rotation = rotation;
	}

	public Transformation(Transform objToCopy, bool local)
	{
		//Constructs a Transformation by copying the relevant data from a Transform.

		if (!local)
		{
			position = objToCopy.position;
			rotation = objToCopy.rotation;
		}
		else
		{
			position = objToCopy.localPosition;
			rotation = objToCopy.localRotation;
		}
	}

	public void ApplyTo(Transform targetObject, bool local)
	{
		//Change's a given Transfrom's position and rotation this position/rotation.  If local is true, then it changes the local position/rotation

		if (!local)
		{
			targetObject.position = this.position;
			targetObject.rotation = this.rotation;
		}
		else
		{
			targetObject.localPosition = this.position;
			targetObject.localRotation = this.rotation;
		}
	}

	//Static methods

	public static Transformation Lerp(Transformation from, Transformation to, float t, bool capAtOne)
	{
        //Cap the t at 1, if enabled.
        if (capAtOne && t > 1)
        {
            t = 1;
        }
        
        //Lerp the transformations
		Vector3 pos = Vector3.Lerp(from.position, to.position, t);
		Quaternion rot = Quaternion.Slerp(from.rotation, to.rotation, t);

		return new Transformation(pos, rot);
	}
}
