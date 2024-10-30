using Godot;
using System;

public partial class Camera : Camera2D {
  [Export] public CharacterBody2D playerNode = null;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _PhysicsProcess(double delta) {
    if (playerNode != null) {
      Position = playerNode.Position;
    }
  }
}
