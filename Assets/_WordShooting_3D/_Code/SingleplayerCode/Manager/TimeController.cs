using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : SingletonAbstract<TimeController>
{
    protected string gameDuration;
    public string GameDuration => gameDuration;
    [SerializeField] protected float remainingTime = 120f;
    public float RemainingTime => remainingTime;
    public bool isPause = false;

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
        UICompleteLevel.Instance.Open();
        MonsterSpawner.Instance.DespawnAll();
        this.gameEnded = false;

    }
    public virtual void PauseGame()
    {
        this.isPause = !this.isPause;
        if (this.isPause)
        {
            UICompleteLevel.Instance.Open();
            Time.timeScale = 0f;
        }
        else
        {
            UICompleteLevel.Instance.Close();
            Time.timeScale = 1f;
        }
    }
}
