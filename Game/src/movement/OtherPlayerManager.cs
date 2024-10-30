namespace Game.Movement;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class OtherPlayerManager : CharacterBody2D {
  private long _timeOfLastUpdate;
  public double AverageDelta = 0.016;
  public string? PlayerId;
  public string? PlayerName;

  public List<Vector2>? PositionHistory;
  public List<float>? RotationHistory;

  public override void _Ready() {
    PositionHistory = new List<Vector2>();
    RotationHistory = new List<float>();

    var collision = new CollisionShape2D();
    AddChild(collision);
  }

  public void AddPositionAndRotation(float x, float y, float rotation) {
    PositionHistory ??= [];
    RotationHistory ??= [];

    PositionHistory.Add(new Vector2(x, y));
    RotationHistory.Add(rotation);

    if (_timeOfLastUpdate == default) {
      _timeOfLastUpdate = DateTime.Now.Ticks;
    }
    else {
      AverageDelta = ((double)DateTime.Now.Ticks - (double)_timeOfLastUpdate) / (double)TimeSpan.TicksPerSecond;
    }
  }

  public override void _PhysicsProcess(double delta) {
    if (!(PositionHistory?.Count > 0)) {
      return;
    }

    var p2 = PositionHistory.First();
    PositionHistory.RemoveAt(0);

    Velocity = new Vector2((float)((p2.X - Position.X) / AverageDelta), (float)((p2.Y - Position.Y) / AverageDelta));
  }
}
