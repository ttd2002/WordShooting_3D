using System;
using System.Collections.Generic;

[Serializable]
public class PlayerScore
{
    public string playerName;
    public string playerScore;
    public PlayerScore(string playerName, string playerScore)
    {
        this.playerName = playerName;
        this.playerScore = playerScore;
    }
    public string GetPlayerName()
    {
        return this.playerName;
    }
    public string GetPlayerScore()
    {
        return this.playerScore;
    }
}
