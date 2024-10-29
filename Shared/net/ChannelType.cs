namespace Shared.net;

public enum ChannelType : byte
{
    Auth = 0,
    ThisPosition = 1,
    OtherPosition = 2,
    ThisAction = 3,
    OtherAction = 4,
    OtherPlayer = 5,


    Info = 10
}