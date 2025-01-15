using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextLeaderBoard : TextBase
{
    protected override void OnEnable()
    {
        this.UpdateLeaderBoard();
    }
    protected virtual void UpdateLeaderBoard()
    {
        string scoreboard = "Leader board\n";
        var sortedPlayerInfos = NetworkScore.Instance.PlayerInfos
           .Where(playerInfo => !string.IsNullOrEmpty(playerInfo.PlayerName.ToString())) 
           .OrderByDescending(playerInfo => playerInfo.PlayerScore);

        foreach (var playerInfo in sortedPlayerInfos)
        {
            scoreboard += $"{playerInfo.PlayerName}: {playerInfo.PlayerScore}\n";
        }

        this.text.SetText(scoreboard);
        
    }
}
