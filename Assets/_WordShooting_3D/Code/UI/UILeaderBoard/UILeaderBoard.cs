using UnityEngine;
using TMPro;
using System.Linq;

public class UILeaderBoard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;


    private void Update()
    {
        string scoreboard = "";
        var sortedPlayerInfos = NetworkScore.Instance.PlayerInfos
           .Where(playerInfo => !string.IsNullOrEmpty(playerInfo.PlayerName.ToString())) 
           .OrderByDescending(playerInfo => playerInfo.PlayerScore);

        foreach (var playerInfo in sortedPlayerInfos)
        {
            scoreboard += $"{playerInfo.PlayerName}: {playerInfo.PlayerScore}\n";
        }

        scoreText.text = scoreboard;
    }
}
