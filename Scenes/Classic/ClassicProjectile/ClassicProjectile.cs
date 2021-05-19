using Godot;
using System;

public class ClassicProjectile : KinematicBody2D
{
    public Vector2 Velocity;
    public ClassicPlayer Player;

    public override void _PhysicsProcess(float delta)
    {
        var c = MoveAndCollide(Velocity*delta);
        if(c != null)
        {
            if(c.Collider != null)
            {
                if(c.Collider is ClassicPlayer p)
                {
                    p.DamageAsync();
                }
                else if(c.Collider is ClassicEnemy e)
                {
                    e.Destroy();
                    if(Player != null)
                    {
                        Player.Score += 10;
                    }
                }
                else if(c.Collider is ClassicBarricade b)
                {
                    b.Damage();
                }
                else if(c.Collider is ClassicBonus bo)
                {
                    bo.QueueFree();
                    if(Player != null)
                    {
                        var rand = new Random();
                        Player.Score += rand.Next(1,4) * 50;
                    }
                }
                if(Player != null) Player.Projectile = null;
                QueueFree();
            }
        }
    }
}
