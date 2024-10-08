using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTotalScore : TextBase
{
    protected override void OnEnable()
    {
        this.UpdateScore();
    }
    protected virtual void UpdateScore()
    {
        int score = ScoreManager.Instance != null ? ScoreManager.Instance.TotalScore : 0;
        this.text.SetText("Total score: " + score);
    }
}
