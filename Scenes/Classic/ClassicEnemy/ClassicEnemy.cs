using Godot;
using System;

[Tool]
public class ClassicEnemy : KinematicBody2D
{
    [Export]
    int Points = 10;
    [Export]
    Texture Texture;
    [Export]
    PackedScene ProjectileScene;
    Position2D ProjectilePosition;
    RayCast2D RayCast;
    AudioStreamPlayer ShootSound;
    Sprite Sprite;

    public override void _Ready()
    {
        ProjectilePosition = GetNode<Position2D>("ProjectilePosition");
        RayCast = GetNode<RayCast2D>("ProjectileRay");
        ShootSound = GetNode<AudioStreamPlayer>("ShootSound");
        Sprite = GetNode<Sprite>("Sprite");
        Sprite.Texture = Texture;
    }

    public override void _Process(float delta)
    {
        if(Engine.EditorHint)
            Sprite.Texture = Texture;
    }

    public void Tick()
    {
        var rand = new Random();
        if (rand.Next(21) == 0)
        {
            if (!RayCast.IsColliding() && !(RayCast.GetCollider() is ClassicEnemy))
            {
                ShootSound.Seek(0);
                ShootSound.Play();
                var projectile = ProjectileScene.Instance() as ClassicProjectile;
                AddChild(projectile);
                projectile.SetAsToplevel(true);
                projectile.GlobalPosition = ProjectilePosition.GlobalPosition;
                projectile.Velocity = new Vector2(0, 500);
            }
        }
    }

    public void Destroy()
    {
        var explosion = GetNode<AudioStreamPlayer>("ExplosionSound");
        var particles = GetNode<CPUParticles2D>("ExplosionParticles");
        var p = GetNode("../..");

        RemoveChild(explosion);
        RemoveChild(particles);
        p.AddChild(explosion);
        p.AddChild(particles);
        explosion.Play();
        particles.Emitting = true;
        particles.SetAsToplevel(true);
        particles.GlobalPosition = GlobalPosition;

        QueueFree();
    }
}
