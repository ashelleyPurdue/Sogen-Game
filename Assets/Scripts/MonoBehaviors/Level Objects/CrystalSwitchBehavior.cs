using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircuitNodePowerSource))]
public class CrystalSwitchBehavior : MonoBehaviour
{
    private Sprite blueSprite;
    private Sprite greenSprite;

    private SpriteRenderer sprRend;
    private CircuitNodePowerSource node;

    void Awake()
    {
        Texture2D blText = (Texture2D)Resources.Load("crystalSwitch_blue");
        Texture2D grText = (Texture2D)Resources.Load("crystalSwitch_green");

        blueSprite = Resources.Load<Sprite>("crystalSwitch_blue");
        greenSprite = Resources.Load<Sprite>("crystalSwitch_green");
        sprRend = GetComponent<SpriteRenderer>();
        node = GetComponent<CircuitNodePowerSource>();
    }

	// Update is called once per frame
	void Update ()
    {
	    if (node.isEnabled)
        {
            sprRend.sprite = greenSprite;
        } else
        {
            sprRend.sprite = blueSprite;
        }
	}

    void OnTakeDamage()
    {
        node.TogglePower();
    }
}
