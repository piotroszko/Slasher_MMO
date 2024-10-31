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

  public void HandleOtherPositionData(
    NetPeer peer,
    NetPacketReader reader,
    DeliveryMethod deliveryMethod) {
    try {
      var objRead = _netSerializer.Deserialize<OtherPositionPacket>(reader).Position;
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
      var objRead = _netSerializer.Deserialize<CurrentPositionPacket>(reader).Position;
    }
    catch (Exception e) {
      GD.PrintErr("HandleCurrentPositionData:", e);
    }
  }

  public override void _Ready() {
    _netSerializer.RegisterNestedType<OtherPosition>();
    _netSerializer.RegisterNestedType<CurrentPosition>();
  }

  public override void _Process(double delta) {
  }
}
