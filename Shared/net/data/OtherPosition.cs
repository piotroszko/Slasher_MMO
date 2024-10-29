using LiteNetLib.Utils;

namespace Shared.net.structs;

public class OtherPosition : INetSerializable
{
    public string Id;
    public float Rotation;
    public float X;
    public float Y;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(X);
        writer.Put(Y);
        writer.Put(Rotation);
    }

    public void Deserialize(NetDataReader reader)
    {
        Id = reader.GetString();
        X = reader.GetFloat();
        Y = reader.GetFloat();
        Rotation = reader.GetFloat();
    }
}