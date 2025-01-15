using Fusion;

public struct PlayerScoreInfo : INetworkStruct
{
    [Networked] public NetworkString<_32> PlayerName { get; set; }  
    [Networked] public int PlayerScore { get; set; }             
}
