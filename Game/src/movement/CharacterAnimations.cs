using Godot;
using System;

public partial class CharacterAnimations : AnimatedSprite2D {
  [Export] String idleAnimation = "idle";

  [Export] String runAnimation = "run";

  [Export] String jumpStartAnimation = "jump_start";
  [Export] String jumpEndAnimation = "jump_end";
  [Export] String jumpAnimation = "jump";

  CharacterBody2D characterBody;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    characterBody = GetParent() as CharacterBody2D;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    var IsOnFloor = characterBody.IsOnFloor();
    var hasSpeed = characterBody.Velocity.Abs().X > 90.0f;
    var isIdleAnim = Animation == idleAnimation;
    var isJumpStartAnim = Animation == jumpStartAnimation;
    var isRunAnim = Animation == runAnimation;

    if (Input.IsActionJustPressed("jump")) {
      if (isIdleAnim || isRunAnim) { Stop(); }

      Play(jumpStartAnimation);
    }

    if (IsOnFloor) {
      if ((Input.IsActionPressed("move_left") || Input.IsActionPressed("move_right")) && hasSpeed) {
        if (isIdleAnim || isJumpStartAnim) { Stop(); }

        Play(runAnimation);
      }
      else if (!hasSpeed) {
        if (isRunAnim || isJumpStartAnim) { Stop(); }

        Play(idleAnimation);
      }
    }

    var direction = characterBody.Velocity.X < 0 ? 1 : -1;
    if (direction == 1) {
      FlipH = true;
    }
    else {
      FlipH = false;
    }
  }
}
