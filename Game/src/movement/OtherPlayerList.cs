using Godot;
using Godot.Collections;
using Shared.net.structs;

public partial class OtherPlayerList : Node {
  [Export] PackedScene OtherPlayerScene;

  public Dictionary<string, OtherPlayerManager> PlayersList = new Dictionary<string, OtherPlayerManager>();

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
  }

  public void AddPlayer(OtherPlayerManager manager) {
    PlayersList.Add(manager.Id, manager);
    AddChild(manager);
  }

  public void RemovePlayer(OtherPlayerManager manager) {
    PlayersList.Remove(manager.Id);
    manager.QueueFree();
  }

  public void RemovePlayer(string playerId) {
    PlayersList[playerId].QueueFree();
    PlayersList?.Remove(playerId);
  }

  public void UpdatePosition(OtherPosition position) {
    if (PlayersList.TryGetValue(position.Id, out var manager)) {
      manager.AddPositionAndRotation(position.X, position.Y, position.Rotation);
    }
  }
}
