using System;
using System.Collections.Generic;
[Serializable]
public class MultiGameHistory
{
    public List<PlayerScore> leaderBoard = new List<PlayerScore>();
    public DateTime timestamp;  

    public string GetLeaderBoard()
    {
        string leaderBoardToString = "";
        foreach (PlayerScore playerInfo in leaderBoard)
        {
            leaderBoardToString += $"{playerInfo.GetPlayerName()}: {playerInfo.GetPlayerScore()}\n";
        }

        return leaderBoardToString;
    }
    
}
