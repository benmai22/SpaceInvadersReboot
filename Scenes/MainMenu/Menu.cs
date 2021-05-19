using System;
using Godot;

public class Menu : Button {
	string gameScene = "res://Scenes/Game.tscn";
	string classicGameScene = "res://Scenes/Classic/ClassicGame.tscn";

	public void NewGameBtnPressed () {        
		GetTree ().ChangeScene (gameScene);
	}

	public void ClassicGameBtnPressed () {        
		GetTree ().ChangeScene (classicGameScene);
	}
	public void QuitBtnPressed () {
		GetTree ().Quit ();
	}
}
