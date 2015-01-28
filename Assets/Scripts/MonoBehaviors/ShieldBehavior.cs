using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DamageBlocker))]
public class ShieldBehavior : MonoBehaviour
{
    private QuickRectangle textureEffect = null;
    
    //Events
    
    void Awake()
    {
        CreateTextureEffect();
    }
    
    //Misc methods
    
    private void CreateTextureEffect()
    {
        //Creates the texture effect object
        
        //Create the gameobject
        Debug.Log("About to create game object.");
        GameObject effectObject = new GameObject();
        effectObject.transform.parent = transform;
        effectObject.transform.localPosition = new Vector3(-0.2f, -0.53f, -1);     //Magic numbers.
        Debug.Log("Created game object.");
        
        //Create the stuff necessary for the QuickRectangle.
        effectObject.AddComponent<MeshRenderer>();
        effectObject.AddComponent<MeshFilter>();
        effectObject.AddComponent<BoxCollider2D>();
        
        effectObject.collider2D.enabled = false;
        
        Debug.Log(effectObject.GetComponent<MeshRenderer>());
        
        //Configure the QuickRectangle
        textureEffect = effectObject.AddComponent<QuickRectangle>();
        
        Debug.Log(textureEffect.renderer);
        
        textureEffect.pointA = new Vector3(0 * transform.localScale.x, 1.07f * transform.localScale.y, 0);    //More magic numbers
        textureEffect.pointB = new Vector3(0.3f * transform.localScale.x, 0 * transform.localScale.y, 0);
        
        textureEffect.texture = (Texture)Resources.Load("whipShieldTexture");
        
        textureEffect.UpdateMesh();
        
        //Create the texture effect's Block Flasher
        BlockFlashEffect flashEff = textureEffect.gameObject.AddComponent<BlockFlashEffect>();
        flashEff.renderers.Add(textureEffect.renderer);
        
        Debug.Log(textureEffect.renderer);
    }
}
