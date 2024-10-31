namespace Game.NetCode;

using System;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;
using Shared.net;
using Sync;

public partial class Network : Node {
  [ExportCategory("Auth")] [Export] private string _authAddress = "localhost";
  [Export] private int _authPort = 9050;


  [ExportCategory("Socket")] [Export] private string _socketAddress = "localhost";
  [Export] private string _socketPassword = "SomeConnectionKey";
  [Export] private int _socketPort = 8080;
  [Export] private int _reconnectDelay = 1000;

  [ExportCategory("Player")] [Export] private CharacterBody2D _playerScene;


  public NetManager? Client;
  public EventBasedNetListener? Listener;

  private NetOtherPositionsManager? _netOtherPositionsManager;
  private SendPosition? _sendPosition;

  private bool _isInitialized;


  public override void _Ready() {
    Listener = new EventBasedNetListener();
    Client = new NetManager(Listener) { ReconnectDelay = _reconnectDelay, ChannelsCount = 10 };
    Client.Start();
    Client.Connect("localhost", 9050, "SomeConnectionKey");
  }

  public override void _PhysicsProcess(double delta) => Client?.PollEvents();

  public override void _Process(double delta) {
    if (_isInitialized) {
      return;
    }

    _netOtherPositionsManager = new NetOtherPositionsManager();
    AddChild(_netOtherPositionsManager);

    _sendPosition = new SendPosition();
    AddChild(_sendPosition);
    _sendPosition._playerScene = _playerScene;

    AddHandlers();
    _isInitialized = true;
    GD.Print("Network Initialized");
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
