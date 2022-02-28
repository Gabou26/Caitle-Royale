using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitTimer
{
    static float currTIme = 0;
    float endTImer = 0;
    RoyaleGame royale;

    public static void Update(float delta)
    {
        currTIme += delta;
    }

    public WaitTimer()
    {
        royale = RoyaleGame.instance as RoyaleGame;
    }

    /// <summary>
    /// Returns when timer ends or if the game has stopped.
    /// </summary>
    /// <param name="length">duration of wait</param>
    public bool WaitIfPlaying()
    {
        //TImer Condition Check 
        if (currTIme >= endTImer || !royale.IsGamePlaying())
            return true;
        return false;
    }

    public void SetWaitTIme(float length)
    {
        endTImer = currTIme + length;
    }
}
