namespace Game.NetCode;

using Godot;
using LiteNetLib;
using Shared.net;
using Sync;

public partial class Network : Node {
  [ExportCategory("Auth")] [Export] private string _authAddress = "localhost";
  [Export] private int _authPort = 9050;


  [ExportCategory("Socket")] [Export] private string _socketAddress = "localhost";
  [Export] private string _socketPassword = "SomeConnectionKey";
  [Export] private int _socketPort = 8080;
  [Export] private int _reconnectDelay = 1000;


  public NetManager? Client;
  public EventBasedNetListener? Listener;

  private NetOtherPositionsManager? _netOtherPositionsManager;


  public override void _Ready() {
    Listener = new EventBasedNetListener();
    Client = new NetManager(Listener) { ReconnectDelay = _reconnectDelay };
    Client.Start();
    Client.Connect(_socketAddress, _socketPort, _socketPassword);


    _netOtherPositionsManager = new NetOtherPositionsManager();
    CallDeferred("add_child", _netOtherPositionsManager);

    AddHandlers();
  }

  public override void _PhysicsProcess(double delta) => Client?.PollEvents();

  public override void _Process(double delta) {
  }

  private void AddHandlers() =>
    Listener!.NetworkReceiveEvent += (peer, dataReader, channel, deliveryMethod) => {
      switch (channel) {
        case (byte)ChannelType.ThisPosition:
          _netOtherPositionsManager!.HandleCurrentPositionData(peer, dataReader, deliveryMethod);
          break;
        case (byte)ChannelType.OtherPosition:
          _netOtherPositionsManager!.HandleOtherPositionData(peer, dataReader, deliveryMethod);
          break;
      }

      GD.PrintRich("We got: ", dataReader.GetString(100), ", on channel:" + channel);
      dataReader.Recycle();
    };
}
