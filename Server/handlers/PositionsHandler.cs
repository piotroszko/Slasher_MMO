using LiteNetLib;
using LiteNetLib.Utils;
using Shared.net;
using Shared.net.structs;

namespace Server.handlers;

public class PositionsHandler
{
    private readonly NetSerializer _netSerializer = new();
    public Dictionary<string, OtherPosition> Players = new();

    public void HandleUpdatePosition(
        NetPeer peer,
        NetPacketReader reader,
        byte channel,
        DeliveryMethod deliveryMethod)
    {
        try
        {
            var objRead = _netSerializer.Deserialize<CurrentPosition>(reader);
            if (Players.ContainsKey(peer.Id.ToString()))
            {
                Players[peer.Id.ToString()].X = objRead.X;
                Players[peer.Id.ToString()].Y = objRead.Y;
                Players[peer.Id.ToString()].Rotation = objRead.Rotation;
            }
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
            var rawPacket = _netSerializer.Serialize(newPlayer);
            peer.Send(rawPacket, (byte)ChannelType.OtherPosition, DeliveryMethod.ReliableUnordered);
        }
    }

    public void SendPlayersPositions(List<NetPeer> peers)
    {
        foreach (var peer in peers)
        {
            var toSend = GetAllPositionsExceptId(peer.Id.ToString());
            foreach (var packet in toSend)
            {
                var rawPacket = _netSerializer.Serialize(packet);
                peer.Send(rawPacket, (byte)ChannelType.OtherPosition, DeliveryMethod.ReliableUnordered);
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