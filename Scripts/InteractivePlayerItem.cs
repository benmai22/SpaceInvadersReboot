using System;
using Godot;

public class InteractivePlayerItem : KinematicBody2D {

    [Export]
    public int speed = 150;

    [Export]
    public string methodName = "";

    [Export]
    public float paramTime = 5;

    Vector2 viewSize = new Vector2 ();

    public override void _Ready () {
        SetPhysicsProcess (true);
        SetProcess (true);
        viewSize = GetViewportRect ().Size;
    }
    public override void _Process (float delta) {
        var currentPosition = Position;
        // Remove Orb once out of screen
        if (currentPosition.y >= viewSize.y) {
            QueueFree ();
        }
    }
    public override void _PhysicsProcess (float delta) {
        var collidedObject = MoveAndCollide (new Vector2 (0, speed * delta));
        // Remove Orb and Player on hit
        if (collidedObject != null) {
            //   GD.Print("Collided with : "+collidedObject.Collider.Free);            
            collidedObject.Collider.Call (methodName,paramTime); 
            QueueFree ();
        }
    }

}