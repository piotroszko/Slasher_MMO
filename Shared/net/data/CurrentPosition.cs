using LiteNetLib.Utils;

namespace Shared.net.structs;

public struct CurrentPosition : INetSerializable
{
    public float Rotation;
    public float X;
    public float Y;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(X);
        writer.Put(Y);
        writer.Put(Rotation);
    }

    public void Deserialize(NetDataReader reader)
    {
        X = reader.GetFloat();
        Y = reader.GetFloat();
        Rotation = reader.GetFloat();
    }
}

public class CurrentPositionPacket
{
    public CurrentPosition Position { get; set; }
}