using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte KEY_TYPED = 3;

    public char typedChar;
    public NetworkButtons buttons;

}