using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class OtherPlayerManager : CharacterBody2D {
  private long _timeOfLastUpdate;
  public double AverageDelta = 0.016;
  public string Id;
  public string Name;

  public List<Vector2> PositionHistory;
  public List<float> RotationHistory;

  public override void _Ready() {
    PositionHistory = new List<Vector2>();
    RotationHistory = new List<float>();

    var collision = new CollisionShape2D();
    AddChild(collision);
  }

  public void AddPositionAndRotation(float x, float y, float rotation) {
    PositionHistory.Add(new Vector2(x, y));
    RotationHistory.Add(rotation);

    if (_timeOfLastUpdate == default) {
      _timeOfLastUpdate = DateTime.Now.Ticks;
    }
    else {
      AverageDelta = (DateTime.Now.Ticks - _timeOfLastUpdate) / TimeSpan.TicksPerSecond;
    }
  }

  public override void _PhysicsProcess(double delta) {
    if (PositionHistory.Count > 0) {
      var p2 = PositionHistory.First();
      PositionHistory.RemoveAt(0);

      Velocity = new Vector2((float)((p2.X - Position.X) / AverageDelta), (float)((p2.Y - Position.Y) / AverageDelta));
    }
  }
}
