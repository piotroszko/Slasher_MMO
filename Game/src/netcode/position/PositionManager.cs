using System;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;
using Shared.net;
using Shared.net.structs;

public partial class PositionManager : Node {
  private readonly NetSerializer _netSerializer = new();
  private NetManager _client;

  public Func<CurrentPosition, string> SendCurrentPosition;

  public void HandleOtherPositionData(
    NetPeer peer,
    NetPacketReader reader,
    DeliveryMethod deliveryMethod) {
    try {
      var objRead = _netSerializer.Deserialize<OtherPosition>(reader);
    }
    catch (Exception e) {
      GD.PrintErr("HandleOtherPositionData:", e);
    }
  }

  public void HandleCurrentPositionData(
    NetPeer peer,
    NetPacketReader reader,
    DeliveryMethod deliveryMethod) {
    try {
      var objRead = _netSerializer.Deserialize<CurrentPosition>(reader);
    }
    catch (Exception e) {
      GD.PrintErr("HandleCurrentPositionData:", e);
    }
  }

  public override void _Ready() {
    _client = GetParent<Network>().Client;

    SendCurrentPosition = position => {
      var objRead = _netSerializer.Serialize(position);
      _client.FirstPeer.Send(objRead, (byte)ChannelType.ThisPosition, DeliveryMethod.ReliableUnordered);
      return null;
    };
  }

  public override void _Process(double delta) {
  }
}
