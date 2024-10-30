namespace Game.Movement;

using Godot;
using System;

public partial class Camera : Camera2D {
  [Export] private CharacterBody2D? _playerNode;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _PhysicsProcess(double delta) {
    if (_playerNode != null) {
      Position = _playerNode.Position;
    }
  }
}
