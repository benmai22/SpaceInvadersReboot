using System;
using Godot;

public class GameManager : Node2D {

	[Export]
	public Vector2 orbGenerationTime = new Vector2 (10, 20);

	[Export]
	public Vector2 asteroidGenerationTime = new Vector2 (15, 25);
	[Export]
	public Vector2 armorGenerationTime = new Vector2 (15, 25);

	[Export]
	public int initialEnemies = 10;
	[Export]
	public int bulletTimer = 2;
	[Export]
	public int enemyHitScore = 50;
	PackedScene enemyScene = (PackedScene) GD.Load ("res://Scenes/Enemy/Enemy.tscn");

	PackedScene orbScene = (PackedScene) GD.Load ("res://Scenes/Orb/Orb.tscn");
	PackedScene asteroidScene = (PackedScene) GD.Load ("res://Scenes/Asteroid/Asteroid.tscn");
	PackedScene armorScene = (PackedScene) GD.Load ("res://Scenes/Shields/Armor.tscn");
	Vector2 viewSize = new Vector2 ();

	private int currentOrbGenerationTime = 0;

	private float orbGenerationTimer = 0f;

	private int currentAsteroidGenerationTime = 0;

	private float asteroidGenerationTimer = 0f;
	private int currentArmorGenerationTime = 0;

	private float armorGenerationTimer = 0f;
	private bool enemiesGenerated = false;
	private int currentScore = 0;
	private RichTextLabel scoreText;
	private RichTextLabel fpsText;
	private string menuScene = "res://Scenes/MainMenu/MainMenu.tscn";

	public override void _Ready () {
		LoadGame ();
		SetProcess (true);
		GenerateNewOrbTime ();
		GenerateNewAsteroidTime ();
		GenerateNewArmorTime ();
		SetBulletTimer ();

		viewSize = GetViewportRect ().Size;
		scoreText = (RichTextLabel) GetNode ("ScoreText");
		scoreText.BbcodeText = "[right]" + currentScore.ToString () + "[/right]";
		fpsText = (RichTextLabel) GetNode ("FPSText");

	}

	public override void _Process (float delta) {

		// fpsText.Text = Performance.GetMonitor (Performance.Monitor.TimeFps).ToString ();

		if (!enemiesGenerated) {
			enemiesGenerated = true;
			GenerateNewEnemies (initialEnemies);
		}

		orbGenerationTimer += delta;
		asteroidGenerationTimer += delta;
		armorGenerationTimer += delta;

		if (orbGenerationTimer >= currentOrbGenerationTime) {
			orbGenerationTimer = 0f;
			GenerateNewOrbTime ();
			GenerateOrb ();
		}

		if (asteroidGenerationTimer >= currentAsteroidGenerationTime) {
			asteroidGenerationTimer = 0f;
			GenerateNewAsteroidTime ();
			GenerateAsteroid ();
		}

		if (armorGenerationTimer >= currentArmorGenerationTime) {
			armorGenerationTimer = 0f;
			GenerateNewArmorTime ();
			GenerateArmor ();
		}
	}

	private void GenerateNewEnemies (int count) {
		if (GenerateEnemies.instance != null) {
			var generatedEnemies = GenerateEnemies.instance;
			var enemies = generatedEnemies.GenerateNewEnemies (enemyScene, viewSize, count);
			foreach (var enemy in enemies) {
				GetTree ().Root.AddChild (enemy);
				// enemy.Connect ("EnemyHitSignal", this, nameof (OnEnemyHitSignal));
			}
		}
	}

	private void SetBulletTimer () {
		Timer aTimer = new Timer();
		aTimer.Autostart = true;
		aTimer.OneShot = false;
		aTimer.Connect("timeout", this, "FireBulletEvent");
	}

	private void FireBulletEvent () {
		if (GenerateEnemies.instance != null) {
			GenerateEnemies.instance.FireBullet ();
		}
	}

	private void GenerateNewOrbTime () {
		currentOrbGenerationTime = new Random ().Next ((int) orbGenerationTime.x, (int) orbGenerationTime.y);
	}

	private void GenerateOrb () {
		var orb = (KinematicBody2D) orbScene.Instance ();
		// Generate orb randomly on X axis leaving out the corners
		orb.Position = new Vector2 (new Random ().Next (Convert.ToInt32 (viewSize.x / 10), Convert.ToInt32 (viewSize.x / 1.2)), 0);
		GetTree ().Root.AddChild (orb);
	}

	private void GenerateNewAsteroidTime () {
		currentAsteroidGenerationTime = new Random ().Next ((int) asteroidGenerationTime.x, (int) asteroidGenerationTime.y);
	}
	private void GenerateAsteroid () {
		var asteroid = (KinematicBody2D) asteroidScene.Instance ();
		// Generate orb randomly on X axis leaving out the corners
		asteroid.Position = new Vector2 (new Random ().Next (Convert.ToInt32 (viewSize.x / 10), Convert.ToInt32 (viewSize.x / 1.2)), 0);
		GetTree ().Root.AddChild (asteroid);
	}

	private void GenerateNewArmorTime () {
		currentArmorGenerationTime = new Random ().Next ((int) armorGenerationTime.x, (int) armorGenerationTime.y);
	}
	private void GenerateArmor () {
		var armor = (KinematicBody2D) armorScene.Instance ();
		// Generate orb randomly on X axis leaving out the corners
		armor.Position = new Vector2 (new Random ().Next (Convert.ToInt32 (viewSize.x / 10), Convert.ToInt32 (viewSize.x / 1.2)), 0);
		GetTree ().Root.AddChild (armor);
	}

	public void OnEnemyHitSignal () {
		currentScore += enemyHitScore;
		scoreText.BbcodeText = "[right]" + currentScore.ToString () + "[/right]";
		GD.Print ("Enemy hit count : " + GenerateEnemies.instance.Count ());
		if (GenerateEnemies.instance != null && GenerateEnemies.instance.Count () <= 0) {
			OnGameOver (true);
		}
	}

	public void Cleanup () {
		if (GenerateEnemies.instance != null)
			GenerateEnemies.instance.RemoveAllEnemyInstance ();
		SaveGame ();
		GetTree ().Paused = false;
	}

	public void OnGameOver (bool won) {
		var gameOver = GetNode<Control>("Interface/GameOver");
		if(!gameOver.Visible) {
			var label = gameOver.GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/Label");
			label.Text = won ? "You saved planet earth from all the aliens!" : "The aliens destroyed your ship!";
			gameOver.Show();
		}
	}

	public void ReturnToMainMenu () {
		Cleanup();
		var resp = GetTree ().ChangeScene (menuScene);
	}
	
	public void RestartLevel () {
		Cleanup();
		GetTree ().ReloadCurrentScene();
	}

	public void SaveGame () {
		var saveGame = new File ();
		saveGame.Open ("user://score.save", (File.ModeFlags) (int) File.ModeFlags.Write);

		saveGame.StoreLine (JSON.Print (currentScore));

		saveGame.Close ();
	}

	public void LoadGame () {
		var saveGame = new File ();
		if (!saveGame.FileExists ("user://score.save")) {
			GD.Print ("No save file found! ");
			return; // Error!  We don't have a save to load.
		}
		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open ("user://score.save", (File.ModeFlags) (int) File.ModeFlags.Read);

		while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// Get the saved dictionary from the next line in the save file
			// var nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveGame.GetLine()).Result);
			currentScore = Int32.Parse (saveGame.GetLine ());

		}
		saveGame.Close ();
	}

}
