using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTotalTime : TextBase
{
    protected override void OnEnable()
    {
        this.UpdateTime();
    }
    protected virtual void UpdateTime()
    {
        float time = 120f - TimeManager.Instance.RemainingTime;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        string timeStamp = string.Format("{0:00}:{1:00}", minutes, seconds);
        this.text.SetText("Total time: " + timeStamp);
    }
}
