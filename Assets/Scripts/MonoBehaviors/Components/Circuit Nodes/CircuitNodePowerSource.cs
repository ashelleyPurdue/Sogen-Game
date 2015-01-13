using UnityEngine;
using System.Collections;

public class CircuitNodePowerSource : CircuitNode
{
	//This node is the source of all power for other circuit nodes.  It is similar to a redstone torch in Minecraft.
	
	public bool isPersistent = false; //Check in the inspector to have this circuit node be persistent.  Make sure a PersistenceRememberer is attached if you do.
	private PersistenceRememberer myRememberer;
	
	public bool isEnabled = true; //When this is true, this power source will power other nodes.
	
	//Events
	protected override void Awake()
	{
	
		base.Awake();
		
		if (isPersistent)
		{
			myRememberer = transform.GetComponent<PersistenceRememberer>();
		}
		
	}
	
	void Update () 
	{
	
	}
	
	protected override void UpdatePowerState()
	{
		//If this node is currently POWERING something, then it is considered to be POWERED as well.
		this.isPowered = isEnabled;
	}
	
	void LoadObjectData()
	{
		//Load the enabled state
		isEnabled = myRememberer.data.GetBoolValue("enabled");
		
		//Update the circuitry, telling all output nodes that their powered state is being loaded from a persistence rememberer.
		UpdateCircuitry(true);
	}
	
	void SaveObjectData()
	{
		//Save the enebled state
		myRememberer.data.SetBoolValue("enabled", isEnabled);
	}
	
	//Interface
	public bool IsEnabled()
	{
		//Returns whether or not this node is powering other nodes.
		return isEnabled;
	}
	
	public void TogglePower()
	{
		//Toggles the power
		
		Enable(!isEnabled);
	}
	
	public void Enable(bool setting)
	{
		//Enables/disables the power source.
		
		isEnabled = setting;
		UpdateCircuitry();
	}
	
}
