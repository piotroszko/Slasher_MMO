using LiteNetLib;
using Microsoft.Extensions.Configuration;
using Server.handlers;
using Shared.net;
using Shared.net.structs;

namespace Server;

public class Server
{
    private readonly IConfigurationRoot _configuration;
    private readonly EventBasedNetListener _listener;
    private readonly NetManager _netManager;
    private readonly PositionsHandler _positionsHandler;
    private bool _isStarted;
    public int Port;
    public readonly string ConnectionKey;

    public Server(int port = 9050, string connectionKey = "SomeConnectionKey")
    {
        Port = port;
        ConnectionKey = connectionKey;
        
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var netConfig = _configuration.GetSection("Net");
        _listener = new EventBasedNetListener();
        _netManager = new NetManager(_listener);

        // Handlers
        _positionsHandler = new PositionsHandler();


        if (netConfig["LatencySimulation"] == "True")
        {
            _netManager.SimulateLatency = true;

            // default
            _netManager.SimulationMinLatency = 40;
            _netManager.SimulationMaxLatency = 60;

            if (int.TryParse(netConfig["LatencyMin"], out var latencyMin))
                _netManager.SimulationMinLatency = latencyMin;
            if (int.TryParse(netConfig["LatencyMax"], out var latencyMax))
                _netManager.SimulationMaxLatency = latencyMax;
        }

        _netManager.ChannelsCount = 10;
    }

    public void Start()
    {
        _netManager.Start(Port);
        _isStarted = true;
        _addHandlers();
    }

    private void _addHandlers()
    {
        if (!_isStarted) return;
        _listener.ConnectionRequestEvent += request =>
        {
            if (_netManager.ConnectedPeersCount < 10 /* max connections */)
                request.AcceptIfKey("SomeConnectionKey");
            else
                request.Reject();
        };
        _listener.PeerConnectedEvent += peer =>
        {
            var newPlayer = _positionsHandler.AddNewPlayer(peer.Id.ToString());
            _positionsHandler.SendNewPlayer(_netManager.ConnectedPeerList, newPlayer);
        };
        _listener.PeerDisconnectedEvent += (peer, disconnectinfo) =>
        {
            _positionsHandler.RemovePlayer(peer.Id.ToString());
        };
        _listener.NetworkReceiveEvent += (peer, reader, channel, method) =>
        {
            switch (channel)
            {
                case (byte)ChannelType.ThisPosition:
                {
                    _positionsHandler.HandleUpdatePosition(peer, reader, channel, method);
                    break;
                }
            }
        };
    }

    public void StartLoop(params Func<NetManager, Dictionary<string, OtherPosition>, string>[] extensions)
    {
        while (!Console.KeyAvailable)
        {
            _netManager.PollEvents();
            if (_netManager.ConnectedPeersCount > 1)
                _positionsHandler.SendPlayersPositions(_netManager.ConnectedPeerList);
            Thread.Sleep(15);
            foreach (var extensionFunc in extensions) extensionFunc(_netManager, _positionsHandler.Players);
        }
    }

    public void Stop()
    {
        _netManager.Stop();
    }
}