using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CollectableBehavior))]
public class SwaeBehavior : MonoBehaviour
{
	// Use this for initialization
	void Awake ()
    {
	    GetComponent<CollectableBehavior>().useDefaultDestructionBehavior = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
