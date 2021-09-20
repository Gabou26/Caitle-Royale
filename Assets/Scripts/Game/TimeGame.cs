using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimeGame : GameManager
{
    public static float timeMatch;
    [SerializeField] protected TMP_Text timeLabel;

    protected override void Start()
    {
        base.Start();
        ResetTimer();
    }

    public override void StartMatch()
    {
        if(!canStartGame)
            return;
        
        base.StartMatch();
        StartCoroutine(TimerMatch());
    }

    private IEnumerator TimerMatch()
    {
        var count = timeMatch;
        while (count > 0f)
        {
            yield return new WaitForSeconds(0.5f);
            count -= 0.5f;
            var minutes = Mathf.FloorToInt(count / 60f);
            var seconds = (int) (count - minutes * 60f);
            
            if (seconds < 10)
            {
                timeLabel.text = "Time : " + minutes + ":0" + seconds;
            }
            else
            {
                timeLabel.text = "Time : " + minutes + ":" + seconds;   
            }
        }

        ResetTimer();
        StopMatch();
    }

    private void ResetTimer()
    {
        var minutes = Mathf.FloorToInt(timeMatch / 60f);
        var seconds = (int) (timeMatch - minutes * 60f);

        if (seconds < 10)
        {
            timeLabel.text = "Time : " + minutes + ":0" + seconds;
        }
        else
        {
            timeLabel.text = "Time : " + minutes + ":" + seconds;   
        }
    }
}