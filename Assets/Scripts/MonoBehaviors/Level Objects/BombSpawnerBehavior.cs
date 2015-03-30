using UnityEngine;
using System.Collections;

public class BombSpawnerBehavior : MonoBehaviour
{
    public Transform spawnPoint;
    
    public float respawnTime = 1f;  //How long after the bomb is destroyed should it respawn.
    
    private float timer = 0f;
    
    private BombBehavior bomb;
    
    //Events
    
    void Start()
    {
        CreateBomb();
    }
    
    void Update()
    {
        //Create a bomb when the timer is up
        if (!BombExists())
        {
            timer += Time.deltaTime;
            
            if (timer >= respawnTime)
            {
                CreateBomb();
                timer = 0f;
            }
        }
    }
    
    //Misc methods
    
    public bool BombExists()
    {
        //Returns if this spawner's bomb still exists.
        
        if (bomb != null)
        {
            try
            {
                Transform t = bomb.transform;
                
                return true;
            }
            catch (MissingReferenceException e)
            {
                bomb = null;
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    
    private void CreateBomb()
    {
        //Creates a bomb.
        
        //Only go on if there is already a bomb.
        if (BombExists())
        {
            return;
        }
        
        //Instantiate the bomb
        GameObject bombObj = (GameObject)Instantiate(Resources.Load("bomb_prefab"));
        bombObj.transform.position = spawnPoint.position;
        
        bomb = bombObj.GetComponent<BombBehavior>();
    }
}
