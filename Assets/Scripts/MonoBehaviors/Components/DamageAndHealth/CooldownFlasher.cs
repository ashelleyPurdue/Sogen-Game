using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CooldownFlasher : MonoBehaviour 
{
	public List<Renderer> rendererList = new List<Renderer>();	//This list will contain all renderers that will
									//Be flashed on and off

	public HealthPoints healthpointsToWatch;			//Flashing will only occur if this HealthPoints
									//is currently cooling down.

	private bool visible = true;

	//Events

	void FixedUpdate()
	{

		//Flash if the HealthPoints is cooling down.
		if (healthpointsToWatch.IsCoolingDown())
		{
			visible = !visible;
		}
		else
		{
			visible = true;
		}

		//Update the visibility
		UpdateVisibility();
	}

	//Misc fucntions

	private void UpdateVisibility()
	{
		//Enables/disables the renderers depending on whether or not the object should be visible

		foreach (Renderer r in rendererList)
		{
			r.enabled = visible;
		}
	}
}
