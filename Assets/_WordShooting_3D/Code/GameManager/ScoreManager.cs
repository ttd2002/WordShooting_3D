using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : SingletonAbstract<ScoreManager>
{
    protected int wordScore = 5;
    protected int bonusScore = 20;
    [SerializeField] protected int totalScore = 0;
    public int TotalScore => totalScore;

    public virtual void AddScore()
    {
        this.totalScore += this.wordScore;
    }

    public virtual void AddBonusScore()
    {
        this.totalScore += this.bonusScore;
    }
}
