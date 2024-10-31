using LiteNetLib;
using LiteNetLib.Utils;
using Shared.net;
using Shared.net.structs;

namespace Server.handlers;

public class PositionsHandler
{
    private readonly NetSerializer _netSerializer;
    public Dictionary<string, OtherPosition> Players = new();

    public PositionsHandler()
    {
        _netSerializer = new NetSerializer();
        _netSerializer.RegisterNestedType<CurrentPosition>();
        _netSerializer.RegisterNestedType<OtherPosition>();
    }

    public void HandleUpdatePosition(
        NetPeer peer,
        NetPacketReader reader,
        byte channel,
        DeliveryMethod deliveryMethod)
    {
        try
        {
            var objRead = _netSerializer.Deserialize<CurrentPositionPacket>(reader).Position;
            if (Players.ContainsKey(peer.Id.ToString()))
            {
                var otherPosition = Players[peer.Id.ToString()];
                otherPosition.X = objRead.X;
                otherPosition.Y = objRead.Y;
                otherPosition.Rotation = objRead.Rotation;
                Players[peer.Id.ToString()] = otherPosition;
            }

            Console.WriteLine($"Player {peer.Id.ToString()} is at {objRead.X}, {objRead.Y}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while reading position: {e}");
        }
    }

    public OtherPosition AddNewPlayer(string id)
    {
        var player = new OtherPosition();
        player.Id = id;
        Players.Add(id, player);
        return player;
    }

    public void SendNewPlayer(List<NetPeer> peers, OtherPosition newPlayer)
    {
        foreach (var peer in peers)
        {
            if (peer.Id.ToString() == newPlayer.Id) continue;
            NetDataWriter netDataWriter = new();
            var packet = new OtherPositionPacket()
            {
                Position = newPlayer,
            };
            _netSerializer.Serialize(netDataWriter, packet);
            peer.Send(netDataWriter, (byte)ChannelType.OtherPosition, DeliveryMethod.ReliableUnordered);
        }
    }

    public void SendPlayersPositions(List<NetPeer> peers)
    {
        NetDataWriter netDataWriter = new();
        foreach (var peer in peers)
        {
            var toSend = GetAllPositionsExceptId(peer.Id.ToString());
            foreach (var position in toSend)
            {
                var packet = new OtherPositionPacket()
                {
                    Position = position,
                };
                _netSerializer.Serialize(netDataWriter, packet);
                peer.Send(netDataWriter, (byte)ChannelType.OtherPosition, DeliveryMethod.ReliableUnordered);
                netDataWriter.Reset();
            }
        }
    }

    public void RemovePlayer(string id)
    {
        Players.Remove(id);
    }

    public List<OtherPosition> GetAllPositionsExceptId(string id)
    {
        var playersToReturn = new List<OtherPosition>();
        foreach (var player in Players)
        {
            if (player.Value.Id == id) continue;
            playersToReturn.Add(player.Value);
        }

        return playersToReturn;
    }

    public OtherPosition GetPlayer(string id)
    {
        return Players[id];
    }
}