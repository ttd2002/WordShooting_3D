using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonAbstract<TimeManager>
{
    protected string gameDuration;
    public string GameDuration => gameDuration;
    [SerializeField] protected float remainingTime = 120f;
    public float RemainingTime => remainingTime;

    protected bool gameEnded = false;
    protected virtual void Update()
    {
        this.UpdateTime();
        this.CheckEndGame();
    }
    protected virtual void UpdateTime()
    {
        if (this.remainingTime > 0)
        {
            this.remainingTime -= Time.deltaTime;
        }
        else if (this.remainingTime < 0)
        {
            this.remainingTime = 0;
            this.EndGame();
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        this.gameDuration = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public virtual void EndGame()
    {
        gameEnded = true;
    }
    public virtual void CheckEndGame()
    {
        if (!this.gameEnded) return;
        Time.timeScale = 0f;
       
        int minutes = Mathf.FloorToInt(30f / 60);
        int seconds = Mathf.FloorToInt(30f % 60);
        string timeStamp = string.Format("{0:00}:{1:00}", minutes, seconds);
        int totalScore = ScoreManager.Instance.TotalScore;
        Debug.Log("Total Score:" + totalScore + "\n Total Time: " + timeStamp);
        this.gameEnded = false;

    }
}
