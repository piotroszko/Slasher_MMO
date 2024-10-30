namespace Game.NetCode.Sync;

using System;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;
using Shared.net;
using Shared.net.structs;

public partial class NetOtherPositionsManager : Node {
  public PackedScene? OtherPlayerScene;

  private readonly NetSerializer _netSerializer = new();
  private NetManager? _client;

  public Func<CurrentPosition, string>? SendCurrentPosition;

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
    if (_client is null) {
      GD.PrintErr("HandleOtherPositionsManager:_Ready");
      return;
    }

    SendCurrentPosition = position => {
      var objRead = _netSerializer.Serialize(position);
      _client.FirstPeer.Send(objRead, (byte)ChannelType.ThisPosition, DeliveryMethod.ReliableUnordered);
      return null!;
    };
  }

  public override void _Process(double delta) {
  }
}
