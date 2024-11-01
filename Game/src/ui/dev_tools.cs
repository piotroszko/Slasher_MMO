using Godot;
using System;

public partial class dev_tools : Control {
  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    if (Visible) Visible = false;
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
    if (Input.IsActionJustPressed("devtools")) {
      Visible = !Visible;
    }
  }
}
