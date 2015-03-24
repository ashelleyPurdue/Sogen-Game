using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircuitNode : MonoBehaviour 
{
	public List<CircuitNode> outputList = new List<CircuitNode>();
	private List<CircuitNode> inputList = new List<CircuitNode>();
	
	public bool isInverted = false;
	
	protected bool isPowered = false;
    
	//Events
	protected virtual void Awake ()
	{
		//Tell all output nodes to add this node as an input.
		foreach (CircuitNode outputNode in outputList)
		{
			outputNode.AddInput(this);
		}
	}
	
	void Start()
	{
		UpdateCircuitry();
	}
	
	void Update () 
	{
		
	}
	
	public void OnDrawGizmos()
	{
		//Draw lines to all output nodes.

		for(int i = 0; i < outputList.Count; i++)
		{
			CircuitNode outputNode = outputList[i];

			Gizmos.DrawLine(transform.position, outputNode.transform.position);
		}
	}
	
	public void OnDestroy()
	{
		//Tell all output nodes to remove this node as an input
		foreach (CircuitNode outputNode in outputList)
		{
			outputNode.RemoveInput(this);
		}
	}
	
	//Interface
	
	public void AddInput(CircuitNode inputToAdd)
	{
		//Adds an input node to the inputList.

		inputList.Add(inputToAdd);
	}

	public void RemoveInput(CircuitNode nodeToRemove)
	{
		//Removes a given node from the input list
		//Call this if the game object containing the input node is destroyed

		//Remove the node
		inputList.Remove(nodeToRemove);

		//Update the circuitry now that we have one less node
		UpdateCircuitry();
	}

	protected virtual void UpdatePowerState()
	{
		/*Updates whether or not this node is powered.
		 * Can be overridden by child classes to create different "rules" for determining if the node should be powered.
		 * For instance, you can have an AndGateNode that inherits from CircuitNode that can only be powered if all inputs
		 * are also powered.
		 */
		
		//Loop through each input node.  If any one of them is powered, then this node is also powered.
		//If none of them are powered, then this node is not powered.
		
		isPowered = false;
		foreach (CircuitNode inputNode in inputList)
		{
			if (inputNode.IsPowered())
			{
				isPowered = true;
				break;
			}
		}
		
		//If this node is inverted, then invert the power state.
		if (isInverted)
		{
			isPowered = !isPowered;
		}
		
	}
	
	public void UpdateCircuitry(bool loadedPower = false)
	{
		//Updates the power state of this node and tells all output nodes to do the same.
		
        bool prevPowered = isPowered;
        
		//Update this node's power state
        UpdatePowerState();
		
		//Tell all output nodes to update *their* circuitry.
		foreach (CircuitNode outputNode in outputList)
		{
			outputNode.UpdateCircuitry();
		}
		
		//If the circuit node is loading its powered-state from a persistence rememberer, then broadcast the LoadPowerState() event.
		if (loadedPower)
		{
			transform.BroadcastMessage("LoadPowerState", IsPowered(), SendMessageOptions.DontRequireReceiver);
		}
        
        //If the powered state ended up changing, send an event.
        if (prevPowered != isPowered)
        {
            try
            {
                transform.BroadcastMessage("OnPoweredChanged", SendMessageOptions.DontRequireReceiver);
            }
            catch (MissingReferenceException e)
            {
                //Do nothing, because this object has been destroyed.
            }
        }

	}
	
	public bool IsPowered()
	{
		//Returns wheter or not this node is powered
		return isPowered;
	}
	
}
