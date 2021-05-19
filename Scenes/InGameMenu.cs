using System;
using Godot;

public class InGameMenu : Button {

	private bool isPaused = false;

	[Signal]
	public delegate void QuitGameSignal ();

	public void QuitBtnPressed () {        
		EmitSignal (nameof (QuitGameSignal));
	}

	public override void _UnhandledInput (InputEvent @event) {
		if (@event is InputEventKey eventKey)
			if (eventKey.Pressed && eventKey.Scancode == (int) KeyList.Escape) {
				// GD.Print ("Called for : " + isPaused);
				if (isPaused) {
					GetTree ().Paused = false;
					isPaused = false;
					((Button) GetNode ("QuitBtn")).Visible = false;
				} else {
					GetTree ().Paused = true;
					isPaused = true;
					((Button) GetNode ("QuitBtn")).Visible = true;
				}
			}
	}
}
