namespace Game.Movement;

using Godot;
using Godot.Collections;
using Shared.net.structs;

public partial class OtherPlayerList : Node {
  [Export] private PackedScene? _otherPlayerScene;

  public Dictionary<string, OtherPlayerManager> PlayersList = new Dictionary<string, OtherPlayerManager>();

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
  }

  public void AddPlayer(string Id) {
    var player = _otherPlayerScene.Instantiate() as OtherPlayerManager;
    player.PlayerId = Id;
    player.PlayerName = Id;
    PlayersList.Add(player.PlayerId!, player);
    AddChild(player);
  }

  public void RemovePlayer(OtherPlayerManager manager) {
    PlayersList.Remove(manager.PlayerId!);
    manager.QueueFree();
  }

  public void RemovePlayer(string playerId) {
    PlayersList[playerId].QueueFree();
    PlayersList.Remove(playerId);
  }

  public void UpdatePosition(OtherPosition position) {
    if (PlayersList.TryGetValue(position.Id, out var manager)) {
      manager.AddPositionAndRotation(position.X, position.Y, position.Rotation);
    }
    else {
      AddPlayer(position.Id);
    }
  }
}
