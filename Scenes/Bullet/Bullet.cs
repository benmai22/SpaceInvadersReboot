using System;
using Godot;

public class Bullet : KinematicBody2D {
	[Export]
	public int speed = 500;
	[Export]
	public int maxNewEnemySpawn = 3;

	PackedScene enemyScene = (PackedScene) GD.Load ("res://Scenes/Enemy/Enemy.tscn");
	Vector2 viewSize = new Vector2 ();

	int direction = -1;

	public override void _Ready () {
		SetProcess (true);
		SetPhysicsProcess (true);
		viewSize = GetViewportRect ().Size;        
	}

	public override void _Process (float delta) {
		var currentPosition = Position;
		// Remove bullet once out of screen
		if (currentPosition.y <= 0) {
			QueueFree ();
		}
	}
	public override void _PhysicsProcess (float delta) {
		var collidedObject = MoveAndCollide (new Vector2 (0, direction * speed * delta));
		// Remove bullet and enemy on hit
		if (collidedObject != null) {

			var colliderName = ((Node) collidedObject.Collider).Name;
			if (colliderName == "Player")
				collidedObject.Collider.Call ("DestroyShip", Name);
			else if (!colliderName.Contains ("EnemyBullet") && !colliderName.Contains ("Bullet")) {                
				collidedObject.Collider.Call ("EnemyHit");
				collidedObject.Collider.Free ();
				GenerateNewEnemies ();
			}

			QueueFree ();
		}
	}

	private void GenerateNewEnemies () {
		if (GenerateEnemies.instance != null) {
			Random rnd = new Random ();
			var generatedEnemies = GenerateEnemies.instance;
			var enemies = generatedEnemies.GenerateNewEnemies (enemyScene, viewSize, rnd.Next (0, (maxNewEnemySpawn + 1)));
			foreach (var enemy in enemies) {
				GetTree ().Root.AddChild (enemy);
			}
		}
	}

}
