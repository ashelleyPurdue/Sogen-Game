using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public static class RandomUtils
{
	public static int WeightedChoice(float[] weights)
	{
		//Returns a randomly selected index number, based on the weights given in the array.
		//The weights DO NOT need to add up to 1.
		
		//Imagine that all of the choices have been lined up on a number line right next to each other, taking up space the size of their weights.
		//We're going to randomly pick a point on that number line, and whichever "space" it falls into will be the result.
		
		int output = -1;
		
		//Find where each space ends
		
		float[] endPoints = new float[weights.Length];
		
		float totalWeight = 0;
		for (int i = 0; i < weights.Length; i++)
		{
			totalWeight += weights[i];
			endPoints[i] = totalWeight;
		}
		
		//Roll a random number
		float rand = Random.Range(0, totalWeight);
		
		//Find out which space it landed in
		
		for (int i = 0; i < endPoints.Length; i++)
		{
			//Find out where this space starts
			float startPoint = endPoints[i] - weights[i];
			
			//Break and return if the rolled number is in between startPoint and endPoint
			if ( (rand > startPoint) && (rand < endPoints[i]) )
			{
				output = i;
				break;
			}
		}
		
		return output;
	}
}

public class FuzzedRandomChooser
{
	//Numbers that haven't been selected in a while have a higher chance of being chosen the next time.

	public float weightBonus;	//How much extra weight each item gets every time it is not used.

	private List<ItemWeightPair> items = new List<ItemWeightPair>();

	public FuzzedRandomChooser(float weightBonus)
	{
		this.weightBonus = weightBonus;
	}

	//Interface

	public void AddItem(int item, float baseWeight)
	{
		//Adds a possible item to the system

		ItemWeightPair newItem = new ItemWeightPair(item, baseWeight, weightBonus);
		items.Add(newItem);
	}

	public void RemoveItem(int item)
	{
		//Removes a possible item from the system

		ItemWeightPair itemToRemove = null;

		foreach (ItemWeightPair pair in items)
		{
			if (pair.Item == item)
			{
				itemToRemove = pair;
				break;
			}
		}

		items.Remove(itemToRemove);
	}

	public float[] GetWeights()
	{
		//Returns all weights in the items list

		float[] weights = new float[items.Count];

		for (int i = 0; i < items.Count; i++)
		{
			weights[i] = items[i].Weight;
		}

		return weights;
	}

	public int Choose()
	{
		//Chooses an item

		//Find the chosen object.
		int index = RandomUtils.WeightedChoice(GetWeights());

		ItemWeightPair chosenItem = items[index];

		//Reset the count for the chosen item
		chosenItem.ResetCounter();

		//Increment the count for all other items
		foreach (ItemWeightPair item in items)
		{
			if (item != chosenItem)
			{
				item.IncrementCounter();
			}
		}

		//Return the object
		return chosenItem.Item;
	}

	//Misc methods

	//Private classes

	private class ItemWeightPair
	{
		private float baseWeight;
		private float weightBonus;
		private int timesSinceLastUse = 0;
		private int item;

		public int Item
		{
			get
			{
				return item;
			}
		}

		public float Weight
		{
			get
			{
				return baseWeight + (weightBonus * timesSinceLastUse);
			}
		}

		public ItemWeightPair(int item, float baseWeight, float weightBonus)
		{
			this.baseWeight = baseWeight;
			this.weightBonus = weightBonus;
			this.item = item;
		}

		public void IncrementCounter()
		{
			//Increments the use counter
			timesSinceLastUse++;
		}

		public void ResetCounter()
		{
			timesSinceLastUse = 0;
		}
	}
}
