using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(QuickRectangle))]
public class LadderBehavior : MonoBehaviour
{
    void Awake()
    {
        collider2D.isTrigger = true;
        transform.position = new Vector3(transform.position.x, transform.position.y, 1);
    }
}