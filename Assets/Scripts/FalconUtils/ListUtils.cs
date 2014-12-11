using System.Collections;
using System.Collections.Generic;

public static class ListUtils
{
	public static bool ListsShareItem<T>(List<T> listOne, List<T> listTwo, T item)
	{
		//Returns whether or not a given tag can be found in both lists
		
		//return HasDamageTag(listOne, item) && HasDamageTag(listTwo, item);	
		return listOne.Contains(item) && listTwo.Contains(item);
	}
	
	public static bool ListsOverlap<T>(List<T> listOne, List<T> listTwo)
	{
		//Returns whether or not two lists share any tags	
		
		bool output = false;
		
		//Loop through each tag in listOne and see if it shares that tag with listTwo
		foreach (T item in listOne)
		{
			
			if (ListsShareItem(listOne, listTwo, item))
			{
				output = true;
				break;
			}
			
		}
		
		return output;
	}
	
	public static List<T> RemoveDuplicateItems<T>(List<T> inputList)
	{
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
