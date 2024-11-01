namespace Game.NetCode.Sync;

using Godot;
using LiteNetLib;
using LiteNetLib.Utils;
using Shared.net;
using Shared.net.structs;

public partial class SendPosition : Node {
  private NetSerializer _netSerializer = new NetSerializer();
  private NetManager? _client;
  public CharacterBody2D _playerScene = null!;

  public override void _PhysicsProcess(double delta) {
    var pos = new CurrentPosition();
    pos.X = _playerScene.Position.X;
    pos.Y = _playerScene.Position.Y;
    pos.Rotation = _playerScene.Rotation;

    SendCurrentPosition(pos);
  }

  private void SendCurrentPosition(CurrentPosition position) {
    if (_client == null) {
      GetClient();
      _netSerializer.RegisterNestedType<CurrentPosition>();
      _netSerializer.RegisterNestedType<OtherPosition>();
      if (_client == null) {
        GD.PrintErr("SendCurrentPosition: client is null");
        return;
      }
    }

    NetDataWriter netDataWriter = new();
    var packet = new SlasherPacket() { CurrentPosition = position, };
    _netSerializer.Serialize(netDataWriter, packet);

    _client.FirstPeer.Send(netDataWriter, (byte)ChannelType.ThisPosition, DeliveryMethod.ReliableOrdered);
  }


  private void GetClient() => _client = GetParent<Network>().Client;
}
