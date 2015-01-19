﻿using UnityEngine;
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
            }

            return instance;
        }
    }

    public enum State {none, tempPausing}
    private State currentState = State.none;

    private delegate void StateMethod();
    private Dictionary<State, StateMethod> stateMethods = new Dictionary<State, StateMethod>();

    private float timer = 0f;

    private float oldTimescale = 1f;

    //Events

    void Awake()
    {
        //Add all the finite state machine methods

        stateMethods.Add(State.none, WhileNone);
        stateMethods.Add(State.tempPausing, WhileTempPausing);
    }

	void Update ()
    {
        stateMethods[currentState]();
	}

    //Interface methods
    public void TempPause(float time)
    {
        //Pauses the action for a certain amount of time.
        timer = time;
        currentState = State.tempPausing;

        oldTimescale = Time.timeScale;
        Time.timeScale = 0f;
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
}