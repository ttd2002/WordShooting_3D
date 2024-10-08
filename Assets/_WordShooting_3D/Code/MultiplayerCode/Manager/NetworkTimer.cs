using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkTimer : NetworkBehaviour
{
    [Networked]
    [OnChangedRender(nameof(OnCountdownTimeChanged))]
    private float countdownTime { get; set; } = 120f;
    [Networked]
    [OnChangedRender(nameof(OnPauseStatusChanged))]
    private bool isPaused { get; set; }

    [SerializeField]
    private TextMeshProUGUI timerText;

    private void Start()
    {
        if (Runner.IsServer)
        {
            Runner.StartCoroutine(StartCountdown());
        }
    }

    private void Update()
    {
        if (!Runner.IsServer) return;
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
        Debug.Log("Game pause status changed: " + isPaused);
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
        isPaused = true;
        Debug.Log("Countdown finished! Game is paused.");
        Time.timeScale = 0f;
    }
}
