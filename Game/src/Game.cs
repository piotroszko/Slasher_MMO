namespace Game;

using Godot;
using LiteNetLib;

public partial class Game : Control {
  private NetManager client;

  public override void _Ready() {
    var listener = new EventBasedNetListener();
    client = new NetManager(listener);
    client.Start();
    client.Connect("localhost" /* host ip or name */, 9050 /* port */,
      "SomeConnectionKey" /* text key or NetDataWriter */);
    listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) => {
      GD.PrintRich("We got: ", dataReader.GetString(100 /* max length of string */), ", on channel:" + channel);
      dataReader.Recycle();
    };
  }

  public override void _PhysicsProcess(double delta) => client.PollEvents();
}
