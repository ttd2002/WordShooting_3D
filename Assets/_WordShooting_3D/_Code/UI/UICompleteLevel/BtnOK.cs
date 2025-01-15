using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnOK : ButtonBase
{
    protected override void OnClick()
    {
        this.SaveSingleGameHistory();
        ScenesManager.Instance.BackToMenuFromGameplay();
    }

    private void SaveSingleGameHistory()
    {
        int minutes = Mathf.FloorToInt((120f - TimeController.Instance.RemainingTime) / 60);
        int seconds = Mathf.FloorToInt((120f - TimeController.Instance.RemainingTime) % 60);
        string totalTime = string.Format("{0:00}:{1:00}", minutes, seconds);
        int totalScore = ScoreController.Instance.TotalScore;
        SingleGameHistory singleGameHistory = new SingleGameHistory(totalScore, totalTime, DateTime.Now);
        SingleHistoryController.Instance.SaveSingleGameHistory(singleGameHistory);
    }

}
