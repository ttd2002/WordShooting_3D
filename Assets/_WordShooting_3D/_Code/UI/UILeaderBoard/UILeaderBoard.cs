using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections;

public class UILeaderBoard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private bool isNetworkScoreReady;

    private void Start()
    {
        StartCoroutine(CheckNetworkScoreReady());
    }

    private IEnumerator CheckNetworkScoreReady()
    {
        while (NetworkScore.Instance == null || !NetworkScore.Instance.Object || !NetworkScore.Instance.Object.IsValid)
        {
            yield return null; // Chờ đến khi NetworkScore sẵn sàng
        }

        isNetworkScoreReady = true;
    }

    private void Update()
    {
        if (!isNetworkScoreReady) return; // Chỉ tiếp tục nếu NetworkScore đã sẵn sàng

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
