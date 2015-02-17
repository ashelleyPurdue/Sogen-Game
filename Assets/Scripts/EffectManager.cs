using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{    
    private static EffectManager instance = null;
    public static EffectManager Instance
    {
        get
        {
            //If there is no instance, create one.
            if (instance == null)
            {
                GameObject obj = new GameObject();
                instance = obj.AddComponent<EffectManager>();
                Object.DontDestroyOnLoad(instance);
            }

            return instance;
        }
    }

    public enum State {none, tempPausing, paused}
    private static State currentState = State.none;
 
    private delegate void StateMethod();
    private Dictionary<State, StateMethod> stateMethods = new Dictionary<State, StateMethod>();

    private static float timer = 0f;

    private static float oldTimescale = 1f;

    //Events

    void Awake()
    {
        //Add all the finite state machine methods

        stateMethods.Add(State.none, WhileNone);
        stateMethods.Add(State.tempPausing, WhileTempPausing);
        stateMethods.Add(State.paused, WhilePaused);
    }

	void Update ()
    {
        stateMethods[currentState]();
	}
 
    void OnGUI()
    {
        if (currentState == State.paused)
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 100, 100), "Paused.");
        }
    }
    
    //Interface methods
    
    public void PauseUnpause()
    {
        //Pauses or unpauses the game.
        
        if (currentState == State.none)
        {
            currentState = State.paused;
            oldTimescale = Time.timeScale;
        }
        else if (currentState == State.paused)
        {
            currentState = State.none;
            Time.timeScale = oldTimescale;
        }
    }
    
    public void TempPause(float time)
    {
        //Pauses the action for a certain amount of time.
        timer = time;
        currentState = State.tempPausing;

        if (Time.timeScale > 0)
        {
            oldTimescale = Time.timeScale;
        }
        Time.timeScale = 0f;
    }
 
    public void WhipStar(Vector3 pos)
    {
        //Creates a whipstar at the specified position
        
        GameObject star = new GameObject("whipstar");
        star.AddComponent<WhipStarBehavior>();
        star.transform.position = pos;
    }
    
    //State Machine methods

    private void WhileNone()
    {
    }
    
    private void WhileTempPausing()
    {
        //Count down
        timer -= Time.unscaledDeltaTime;

        //Unpause when time is up
        if (timer <= 0)
        {
            Time.timeScale = oldTimescale;
            currentState = State.none;
        }
    }

    private void WhilePaused()
    {
        Time.timeScale = 0;
    }
}
