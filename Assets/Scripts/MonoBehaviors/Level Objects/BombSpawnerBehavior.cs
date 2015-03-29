using UnityEngine;
using System.Collections;

public class BombSpawnerBehavior : MonoBehaviour
{
    public Transform spawnPoint;
    
    private BombBehavior bomb;
    
    //Events
    
    void Start()
    {
        CreateBomb();
    }
    
    void Update()
    {
        CreateBomb();
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
