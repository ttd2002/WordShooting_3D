using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkTimer : SingletonNetworkAbstract<NetworkTimer>
{
    [Networked]
    [OnChangedRender(nameof(OnCountdownTimeChanged))]
    private float countdownTime { get; set; } = 15;
    [Networked]
    [OnChangedRender(nameof(OnPauseStatusChanged))]
    private bool isPaused { get; set; }
    [SerializeField] private TextMeshProUGUI timerText;
    private bool isFinished = false;
    protected override void Start()
    {
        if (Runner.IsServer)
        {
            Runner.StartCoroutine(StartCountdown());
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer || isFinished) return;
        countdownTime -= Runner.DeltaTime;

        if (countdownTime <= 0f)
        {
            countdownTime = 0f;
            OnCountdownFinished();
        }
    }

    private void OnCountdownTimeChanged()
    {
        UpdateTimerUI();
    }
    private void OnPauseStatusChanged()
    {
        Time.timeScale = isPaused ? 0f : 1f;
    }
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(countdownTime / 60);
        int seconds = Mathf.FloorToInt(countdownTime % 60);
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    private IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            yield return null;
        }
        OnCountdownFinished();
    }

    private void OnCountdownFinished()
    {
        if (isFinished) return;
        isFinished = true;
        isPaused = true;
        Time.timeScale = 0f;

        RPC_HandleCountdownFinished();

    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_HandleCountdownFinished()
    {
        SaveGameHistory();
        UICompleteLevel.Instance.Open();
    }
    private void SaveGameHistory()
    {
        MultiGameHistory multiGameHistory = new MultiGameHistory();
        var sortedPlayerInfos = NetworkScore.Instance.PlayerInfos
           .Where(playerInfo => !string.IsNullOrEmpty(playerInfo.PlayerName.ToString())) 
           .OrderByDescending(playerInfo => playerInfo.PlayerScore);

        foreach (var playerInfo in sortedPlayerInfos)
        {
            if (!string.IsNullOrEmpty(playerInfo.PlayerName.ToString()))
            {
                PlayerScore playerScore = new PlayerScore(playerInfo.PlayerName.ToString(), playerInfo.PlayerScore.ToString());
                multiGameHistory.leaderBoard.Add(playerScore);
            }
        }
        FirebaseManager.Instance.SaveMultiGameHistory(multiGameHistory);
    }
    
    public bool GetIsPaused()
    {
        return isPaused;
    }

}
