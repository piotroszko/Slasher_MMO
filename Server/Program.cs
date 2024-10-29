using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Shared.net;
using Spectre.Console;

namespace Server;

internal class Program
{
    private static void Main()
    {
        var config = new ConfigurationBuilder().AddJsonFile(
                "appsettings.json")
            .Build();
        var netConfig = config.GetSection("Net");
        var listener = new EventBasedNetListener();
        var server = new NetManager(listener);

        if (netConfig["LatencySimulation"] == "True")
        {
            server.SimulateLatency = true;

            // default
            server.SimulationMinLatency = 40;
            server.SimulationMaxLatency = 60;

            if (int.TryParse(netConfig["LatencyMin"], out var latencyMin)) server.SimulationMinLatency = latencyMin;
            if (int.TryParse(netConfig["LatencyMax"], out var latencyMax)) server.SimulationMaxLatency = latencyMax;
        }

        server.ChannelsCount = 10;


        server.Start(9050 /* port */);

        listener.ConnectionRequestEvent += request =>
        {
            if (server.ConnectedPeersCount < 10 /* max connections */)
                request.AcceptIfKey("SomeConnectionKey");
            else
                request.Reject();
        };
        listener.PeerConnectedEvent += peer =>
        {
            var writer = new NetDataWriter(); // Create writer class
            writer.Put("Hello client!"); // Put some string
            peer.Send(writer, (byte)ChannelType.Auth, DeliveryMethod.ReliableUnordered); // Send with reliability
        };
        listener.PeerDisconnectedEvent += (peer, disconnectinfo) => { };

        var table = new Table().Centered();
        AnsiConsole.Live(table).AutoClear(false).Start(
            ctx =>
            {
                table.Title = new TableTitle("Server");
                table.Caption = new TableTitle("port=9050 | ip=localhost");
                table.AddColumn("Ping");
                table.AddColumn("Id");
                table.AddColumn("Name");
                table.AddColumn("X");
                table.AddColumn("Y");
                table.AddColumn("World");

                while (!Console.KeyAvailable)
                {
                    server.PollEvents();
                    ctx.Refresh();
                    Thread.Sleep(15);
                    table.Rows.Clear();
                    foreach (var peer in server.ConnectedPeerList)
                        table.AddRow(peer.Ping.ToString(), peer.Id.ToString());
                }
            });

        server.Stop();
    }
}