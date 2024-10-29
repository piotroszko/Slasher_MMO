using System.Linq;
using Godot;
using LiteNetLib;
using Shared.net;

public partial class Network : Node {
  [ExportCategory("Auth")]
  // Auth properties
  [Export]
  private string _authAddress = "localhost";

  [Export] private int _authPort = 9050;
  [Export] private int _reconnectDelay = 1000;

  [ExportCategory("Socket")]
  //// Socket properties
  [Export]
  private string _socketAddress = "localhost";

  [Export] private string _socketPassword = "SomeConnectionKey";

  [Export] private int _socketPort = 8080;
  // public properties

  // private properties
  public NetManager Client;

  public override void _Ready() {
    var listener = new EventBasedNetListener();
    Client = new NetManager(listener);
    Client.ReconnectDelay = _reconnectDelay;
    Client.Start();
    Client.Connect(_socketAddress, _socketPort, _socketPassword);


    var childrenPositionManager = GetChildren().OfType<PositionManager>();
    PositionManager positionManager;
    if (childrenPositionManager.Any()) {
      positionManager = childrenPositionManager.First();
    }
    else {
      positionManager = new PositionManager();
      CallDeferred("add_child", positionManager);
    }

    listener.NetworkReceiveEvent += (peer, dataReader, channel, deliveryMethod) => {
      switch (channel) {
        case (byte)ChannelType.ThisPosition:
          positionManager.HandleCurrentPositionData(peer, dataReader, deliveryMethod);
          break;
        case (byte)ChannelType.OtherPosition:
          positionManager.HandleOtherPositionData(peer, dataReader, deliveryMethod);
          break;
      }

      GD.PrintRich("We got: ", dataReader.GetString(100), ", on channel:" + channel);
      dataReader.Recycle();
    };
  }

  public override void _Process(double delta) {
  }
}
