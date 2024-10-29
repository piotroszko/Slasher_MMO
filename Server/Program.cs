using Spectre.Console;

namespace Server;

internal class Program
{
    private static void Main()
    {
        var server = new Server();
        server.Start();

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

                server.StartLoop((netManager, positions) =>
                {
                    ctx.Refresh();
                    table.Rows.Clear();
                    foreach (var peer in netManager.ConnectedPeerList)
                    {
                        if (!positions.ContainsKey(peer.Id.ToString())) continue;
                        var player = positions[peer.Id.ToString()];
                        table.AddRow(peer.Ping.ToString(), peer.Id.ToString(), "Name-" + peer.Id, player.X.ToString(),
                            player.Y.ToString());
                    }

                    return null;
                });
            });

        server.Stop();
    }
}