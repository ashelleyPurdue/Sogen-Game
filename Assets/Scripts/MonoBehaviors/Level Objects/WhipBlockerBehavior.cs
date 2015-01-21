using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircuitNode))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(DamageBlocker))]
public class WhipBlockerBehavior : MonoBehaviour
{
    private CircuitNode node;
    private DamageBlocker blocker;
    //Events

    void Awake()
    {
        node = GetComponent<CircuitNode>();
        blocker = GetComponent<DamageBlocker>();
    }

	// Update is called once per frame
	void Update ()
    {
	    //Enable/disable based on the powered state.
        blocker.isActive = node.IsPowered();
        renderer.enabled = node.IsPowered();
        collider2D.enabled = node.IsPowered();
	}
}
