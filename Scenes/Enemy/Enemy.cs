using System;
using Godot;

public class Enemy : KinematicBody2D {
	[Export]
	public int minSpeed = 100;
	[Export]
	public int maxSpeed = 400;
	[Export]
	public int enemyWidth = 25;
	[Export]
	public int enemyHeight = 25;
	[Export]
	public Vector2 bulletInterval = new Vector2 (4, 8);

	private int speed = 100;
	private int direction = 1;
	Vector2 viewSize = new Vector2 ();
	PackedScene bulletScene = (PackedScene) GD.Load ("res://Scenes/Bullet/EnemyBullet.tscn");

	private float currentBulletInterval = 0;
	private float bulletTime = 0;

	[Signal]
	public delegate void EnemyHitSignal ();

	public override void _Ready () {
		SetProcess (true);
		SetPhysicsProcess (true);
		viewSize = GetViewportRect ().Size;
		Random rnd = new Random ();
		direction = (rnd.Next (0, 2) == 1 ? 1 : -1);
		speed = rnd.Next (minSpeed, maxSpeed);
		GenerateNewBulletTime ();

		this.Connect (nameof (EnemyHitSignal), GetTree ().Root.GetNode ("World"), "OnEnemyHitSignal");
		// GD.Print ("Root : " + GetTree ().Root.GetChild (0).Name);

		// player = GetParent().GetNode<KinematicBody2D> ("Player");

	}

	public override void _Process (float delta) {
		viewSize = GetViewportRect ().Size;
		var currentPosition = Position;

		if (currentPosition.x <= (0 + enemyWidth))
			direction = 1;
		else if (currentPosition.x >= (viewSize.x - enemyWidth))
			direction = -1;

		currentPosition.x = Mathf.Clamp (currentPosition.x, 0 + enemyWidth, viewSize.x - enemyWidth);
		Position = currentPosition;

	}

	public override void _PhysicsProcess (float delta) {
		MoveAndCollide (new Vector2 (direction * speed * delta, 0));

		bulletTime += delta;
		if (bulletTime >= currentBulletInterval) {
			Godot.Collections.Dictionary hits =
				GetWorld2d ().DirectSpaceState.IntersectRay (new Vector2 (Position.x, 0), new Vector2 (Position.x, viewSize.y), null, 1);

			if (hits.Count > 0) {
				Godot.Object collider = (Godot.Object) hits["collider"];
				if (((Node) collider).Name == "Player") {
					bulletTime = 0;
					GenerateNewBulletTime ();
					FireBullet ();
				}
			}
		}
	}

	private void GenerateNewBulletTime () {
		currentBulletInterval = new Random ().Next ((int) bulletInterval.x, (int) bulletInterval.y);
	}

	private void FireBullet () {
		if (!GetTree ().Paused) {
			var bullet = (KinematicBody2D) bulletScene.Instance ();
			bullet.Position = new Vector2 (Position.x, Position.y + enemyHeight);
			bullet.Set ("direction", 1);
			GetTree ().Root.AddChild (bullet);
		}
	}

	public void EnemyHit () {
		if (GenerateEnemies.instance != null) {
			int enemyCount = GenerateEnemies.instance.RemoveEnemy (this);
			EmitSignal (nameof (EnemyHitSignal));
		}
	}
}
