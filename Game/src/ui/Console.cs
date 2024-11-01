using Godot;
using System.Collections.Generic;

public partial class Console : Panel {
  [Export] RichTextLabel consoleLabel = null;
  [Export] TextEdit consoleTextEdit = null;
  List<string> lines = new List<string>();

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
  }

  public void AddNewLine(string line) {
    lines.Add(line);
  }

  public static void PrintConsole(string text) {
  }
}
