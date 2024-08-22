using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScore : TextBase
{
    protected override void Update()
    {
        this.UpdateScore();
    }
    protected virtual void UpdateScore()
    {
        int score = ScoreManager.Instance.TotalScore;
        this.text.SetText("Score: " + score);
    }
}
