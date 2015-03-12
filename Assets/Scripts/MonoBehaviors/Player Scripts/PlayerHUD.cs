using UnityEngine;
using System.Collections;

public class PlayerHUD : MonoBehaviour
{
    private HealthPoints myHealth;
    
    //Events
    
    void Awake()
    {
        myHealth = GetComponent<HealthPoints>();
    }
    
    void OnGUI()
    {
        //Draw the health
        GUILayout.Box("Health: " + myHealth.GetHealth(), new GUILayoutOption[] {});
        GUILayout.Box("Swae: " + PlayerPlatformBehavior.currentSwae, new GUILayoutOption[] {});
    }
}
