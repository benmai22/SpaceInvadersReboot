using System;
using Godot;

public class Asteroid : KinematicBody2D {
    [Export]
    public int speed = 150;

    Vector2 viewSize = new Vector2 ();

    public override void _Ready () {
        SetPhysicsProcess (true);
        SetProcess (true);
        viewSize = GetViewportRect ().Size;
    }
    public override void _Process (float delta) {
        var currentPosition = Position;
        // Remove Asteroid once out of screen
        if (currentPosition.y >= viewSize.y) {
            QueueFree ();
        }

        // var currentCamera = GetViewport ().GetCamera ();

        Position = new Vector2 (new Random ().Next ((int) Position.x - 2, (int) Position.x + 2), Position.y);
    }
    public override void _PhysicsProcess (float delta) {
        var collidedObject = MoveAndCollide (new Vector2 (0, speed * delta));
        // Remove anything on hit
        if (collidedObject != null) {
            // if (((Node) collidedObject.Collider).Name == "Player") {
            //     QueueFree ();
            //     collidedObject.Collider.Call ("DestroyShip", Name);
            // } else {
            //     collidedObject.Collider.Free ();
            // }

            var colliderName = ((Node) collidedObject.Collider).Name;
            if (colliderName == "Player") {
                QueueFree ();
                collidedObject.Collider.Call ("DestroyShip", Name);
            } else if (!colliderName.Contains ("EnemyBullet") && !colliderName.Contains ("Bullet")) {
                collidedObject.Collider.Call ("EnemyHit");
                collidedObject.Collider.Free ();
            } else {
                collidedObject.Collider.Free ();
            }

        }
    }
}