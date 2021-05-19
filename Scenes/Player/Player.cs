using System;
using Godot;

public class Player : KinematicBody2D {

	[Export]
	public int speed = 100;
	[Export]
	public int shipWidth = 10;
	[Export]
	public int shipHeight = 30;

	[Export]
	public float bulletCooldownTime = 0.5f;

	[Export]
	public float burstBulletCooldownTime = 0.1f;
	[Export]
	public int hitDamage = 20;

	[Export]
	public float flashSeconds = 0.5f;

	[Export]
	public int currentHealth = 100;

	[Export]
	public int currentLives = 0;

	PackedScene bulletScene = (PackedScene) GD.Load ("res://Scenes/Bullet/Bullet.tscn");
	PackedScene burstBulletScene = (PackedScene) GD.Load ("res://Scenes/Bullet/BurstBullet.tscn");

	private Vector2 viewSize = new Vector2 ();

	private float bulletCooldownTimer = 0;
	private float currentBulletCooldownTime = 0;
	private bool canFire = true;
	private bool shouldFlashShip = false;
	private bool isShipFlashing = false;
	private bool isArmorActive = false;
	private Sprite shipSprite = null;
	private RichTextLabel healthText = null;
	private RichTextLabel livesText = null;
	private int burstBulletMode = 0;
	private bool isBurstToggled = false;

	private AudioStreamPlayer audio;

	[Signal]
	public delegate void GameOverSignal ();

	public override void _Ready () {
		SetProcess (true);
		SetPhysicsProcess (true);
		LoadCurrentLives ();
		LoadCurrentHealth ();
		LoadBurstBullet ();

		viewSize = GetViewportRect ().Size;

		currentBulletCooldownTime = bulletCooldownTime;
		shipSprite = (Sprite) GetNode ("Sprite");
		healthText = (RichTextLabel) GetNode ("HealthText");
		livesText = (RichTextLabel) GetNode ("LivesText");
		healthText.Text = currentHealth + "%";
		livesText.Text = currentLives.ToString ();

		audio = (AudioStreamPlayer) GetNode ("Audio");        
	}

	public override void _Process (float delta) {
		if (!isBurstToggled) {
			if (Input.IsActionJustPressed ("fire")) {
				FireBullet ();
			}
		} else {
			if (Input.IsActionPressed ("fire")) {
				FireBullet ();
			}
		}

		var currentPosition = Position;
		currentPosition.x = Mathf.Clamp (currentPosition.x, 0 + shipWidth, viewSize.x - shipWidth);
		Position = currentPosition;

		if (!canFire) {
			bulletCooldownTimer += delta;
			if (bulletCooldownTimer >= bulletCooldownTime) {
				bulletCooldownTimer = 0;
				bulletCooldownTime = currentBulletCooldownTime;
				canFire = true;
				isShipFlashing = false;
			} else if (shouldFlashShip) {
				shouldFlashShip = false;
				if (!isShipFlashing) {
					isShipFlashing = true;
					FlashShip ();
				}
			}
		}

		if (isArmorActive && !isShipFlashing) {
			shipSprite.SelfModulate = new Color (0, 1, 0);
		} else if (!isArmorActive && !isShipFlashing) {
			shipSprite.SelfModulate = new Color (1, 1, 1);
		}
	}

	async private void FlashShip () {
		bool changeColor = false;
		while (isShipFlashing) {
			await ToSignal (GetTree ().CreateTimer (flashSeconds), "timeout");
			shipSprite.SelfModulate = changeColor ? new Color (1, 0, 0) : new Color (1, 1, 1);
			changeColor = !changeColor;
		}
	}

	public override void _PhysicsProcess (float delta) {
		if (Input.IsActionPressed ("ui_left") || Input.IsActionPressed ("move_left"))
			MoveAndCollide (new Vector2 ((-speed * delta), 0));
		else if (Input.IsActionPressed ("ui_right") || Input.IsActionPressed ("move_right"))
			MoveAndCollide (new Vector2 ((speed * delta), 0));
	}

	private void FireBullet () {
		if (canFire) {
			canFire = false;
			if (!isBurstToggled) {
				var bullet = (KinematicBody2D) bulletScene.Instance ();
				bullet.Position = new Vector2 (Position.x, Position.y - shipHeight);
				GetTree ().Root.AddChild (bullet);
			} else {
				var bullet = (KinematicBody2D) burstBulletScene.Instance ();
				bullet.Position = new Vector2 (Position.x-40, Position.y - shipHeight);
				GetTree ().Root.AddChild (bullet);
			}
			audio.Play();
		}
	}

	private void StopBulletForTime (float newTime) {
		bulletCooldownTime += newTime;
		canFire = false;
		shouldFlashShip = true;
	}

	private void AddArmour (float time) {
		isArmorActive = true;
		DisableArmorAfterTime (time);
	}

	async private void DisableArmorAfterTime (float time) {
		await ToSignal (GetTree ().CreateTimer (time), "timeout");
		isArmorActive = false;
	}

	private void DestroyShip (string colliderName) {
		if (!isArmorActive) {

			if (colliderName == "Asteroid")
				currentHealth = 0;
			else
				currentHealth -= hitDamage;

			if (currentHealth <= 0) {
				if (currentLives > 0) {
					currentLives -= 1;
					currentHealth = 100;
					SaveHealth ();
					SaveLives ();
				} else {
					EmitSignal (nameof (GameOverSignal), false);
					QueueFree ();
				}
			}

			livesText.Text = currentLives.ToString ();
			healthText.Text = currentHealth + "%";

		}
	}

	private void LoadCurrentLives () {
		var saveGame = new File ();
		if (!saveGame.FileExists ("user://lives.save")) {
			GD.Print ("No Lives save file found! ");
			return; // Error!  We don't have a save to load.
		}
		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open ("user://lives.save", (File.ModeFlags) (int) File.ModeFlags.Read);

		while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// Get the saved dictionary from the next line in the save file
			// var nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveGame.GetLine()).Result);
			currentLives = currentLives + (Int16.Parse (saveGame.GetLine ()));

		}
		saveGame.Close ();
	}

	private void LoadCurrentHealth () {
		var saveGame = new File ();
		if (!saveGame.FileExists ("user://health.save")) {
			GD.Print ("No Health save file found! ");
			return; // Error!  We don't have a save to load.
		}
		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open ("user://health.save", (File.ModeFlags) (int) File.ModeFlags.Read);

		while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// Get the saved dictionary from the next line in the save file
			// var nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveGame.GetLine()).Result);
			currentHealth = (Int16.Parse (saveGame.GetLine ()));
			currentHealth = Mathf.Max (currentHealth, 100);
		}
		saveGame.Close ();
	}

	private void LoadBurstBullet () {
		var saveGame = new File ();
		if (!saveGame.FileExists ("user://burst.save")) {
			GD.Print ("No Burst save file found! ");
			return; // Error!  We don't have a save to load.
		}
		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open ("user://burst.save", (File.ModeFlags) (int) File.ModeFlags.Read);

		while (saveGame.GetPosition () < saveGame.GetLen ()) {
			// Get the saved dictionary from the next line in the save file
			// var nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveGame.GetLine()).Result);
			burstBulletMode = (Int16.Parse (saveGame.GetLine ()));
		}
		saveGame.Close ();
	}
	public void SaveLives () {
		var saveGame = new File ();

		saveGame.Open ("user://lives.save", (File.ModeFlags) (int) File.ModeFlags.Write);

		saveGame.StoreLine (JSON.Print (Mathf.Max ((currentLives - 1), 0)));

		saveGame.Close ();
	}

	public void SaveHealth () {
		var saveGame = new File ();

		saveGame.Open ("user://health.save", (File.ModeFlags) (int) File.ModeFlags.Write);

		saveGame.StoreLine (JSON.Print (currentHealth));

		saveGame.Close ();
	}

	public override void _UnhandledInput (InputEvent @event) {
		if (@event is InputEventKey eventKey) {
			if (@event.IsActionPressed ("number2")) {
				if (burstBulletMode == 1) {
					isBurstToggled = true;
					currentBulletCooldownTime = burstBulletCooldownTime;
				} else {

				}
			} else if (@event.IsActionPressed ("number1")) {
				isBurstToggled = false;
				currentBulletCooldownTime = bulletCooldownTime;
			}
		}
	}
}
