using System;
using Fusion;

public class NetworkScore : SingletonNetworkAbstract<NetworkScore>
{
    [Networked, Capacity(4)]
    public NetworkArray<PlayerScoreInfo> PlayerInfos { get; }

    public void UpdatePlayerScore(int index, int currentScore)
    {
        if (index >= 0 && index < PlayerInfos.Length && Runner.IsServer)
        {
            PlayerInfos.Set(index, new PlayerScoreInfo
            {
                PlayerName = PlayerInfos[index].PlayerName,
                PlayerScore = currentScore += 5
            });
        }
    }
    public void UpdatePlayerBonusScore(int index, int currentScore)
    {
        if (index >= 0 && index < PlayerInfos.Length && Runner.IsServer)
        {
            PlayerInfos.Set(index, new PlayerScoreInfo
            {
                PlayerName = PlayerInfos[index].PlayerName,
                PlayerScore = currentScore += 20
            });
        }
    }
    public void UpdatePlayerName(int index, string newName)
    {
        if (index >= 0 && index < PlayerInfos.Length)
        {
            PlayerInfos.Set(index, new PlayerScoreInfo
            {
                PlayerName = newName,
                PlayerScore = PlayerInfos[index].PlayerScore
            });
        }
    }
}
