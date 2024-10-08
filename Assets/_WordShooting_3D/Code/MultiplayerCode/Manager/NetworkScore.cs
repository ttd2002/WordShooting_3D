using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkScore : NetworkBehaviour
{
    protected int wordScore = 5;
    protected int bonusScore = 20;
    [Networked]
    [OnChangedRender(nameof(OnScoreChanged))]
    public int totalScore { get; set; } = 0;

    [SerializeField] private TextMeshProUGUI playerScore;
    private void OnScoreChanged()
    {
        playerScore.text = totalScore.ToString();
    }

    public virtual void AddScore()
    {
        this.totalScore += this.wordScore;
        playerScore.text = totalScore.ToString();
    }
    public virtual void AddBonusScore()
    {
        this.totalScore += this.bonusScore;
        playerScore.text = totalScore.ToString();
    }

}
