using UnityEngine;
using System.Collections;

public class AndGateNode : CircuitNode
{
	protected override void UpdatePowerState()
	{
		//Only be powered if all input nodes are powered

		isPowered = true;

		foreach (CircuitNode node in outputList)
		{
			if (!node.IsPowered())
			{
				isPowered = false;
				break;
			}
		}

		//If this node is inverted, then invert the result.
		if (isInverted)
		{
			isPowered = !isPowered;
		}
	}
}
