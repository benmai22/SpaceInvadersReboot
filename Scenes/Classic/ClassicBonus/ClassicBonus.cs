using Godot;
using System;

public class ClassicBonus : KinematicBody2D
{
    Vector2 Velocity;
    public override void _Ready()
    {
        Velocity = new Vector2(250, 0);
        if(GlobalPosition.x > 0) Velocity *= -1;
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        var c = MoveAndCollide(Velocity * delta);
        if(c != null)
        {
            if(c.Collider != null)
            {
                QueueFree();
            }
        }
    }
}
