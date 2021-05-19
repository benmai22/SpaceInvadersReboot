using Godot;
using System;

public class ClassicPlayer : KinematicBody2D
{
    float Speed = 300;
    [Export]
    PackedScene ProjectileScene;
    Position2D ProjectilePosition;
    AudioStreamPlayer ShootSound;
    public ClassicProjectile Projectile;
    public int Lives = 3;
    bool Dead;
    public int Score;
    Sprite Sprite;
    CPUParticles2D ExplosionParticles;
    AudioStreamPlayer ExplosionSound;
    public ClassicGame Game;
    public override void _Ready()
    {
        ProjectilePosition = GetNode<Position2D>("Position2D");
        ShootSound = GetNode<AudioStreamPlayer>("ShootSound");
        Sprite = GetNode<Sprite>("Sprite");
        ExplosionParticles = GetNode<CPUParticles2D>("ExplosionParticles");
        ExplosionSound = GetNode<AudioStreamPlayer>("ExplosionSound");
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!Dead)
        {
            var movment = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
            MoveAndCollide(new Vector2(movment * delta * Speed, 0));

            if (Input.IsActionJustPressed("fire") && Projectile == null)
            {
                ShootSound.Seek(0);
                ShootSound.Play();
                var projectile = ProjectileScene.Instance() as ClassicProjectile;
                AddChild(projectile);
                projectile.SetAsToplevel(true);
                projectile.GlobalPosition = ProjectilePosition.GlobalPosition;
                projectile.Velocity = new Vector2(0, -1000);
                projectile.Player = this;
                Projectile = projectile;
            }
        }
    }

    public async System.Threading.Tasks.Task DamageAsync()
    {
        if (Lives <= 0)
        {
            Dead = true;
            Sprite.Visible = false;
            SetCollisionLayerBit(1, false);
            Game.Lose();
        }
        else
        {
            Sprite.Visible = false;
            SetCollisionLayerBit(0, false);
            Dead = true;
            ExplosionParticles.Emitting = true;
            ExplosionSound.Seek(0);
            ExplosionSound.Play();
            Lives--;
            await ToSignal(GetTree().CreateTimer(2), "timeout");
            Position = new Vector2(0, Position.y);
            Sprite.Visible = true;
            SetCollisionLayerBit(0, true);
            Dead = false;
        }
    }
}
