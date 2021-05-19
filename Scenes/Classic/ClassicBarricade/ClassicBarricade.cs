using Godot;
using System;

[Tool]
public class ClassicBarricade : StaticBody2D
{
    [Export]
    Texture Texture;
    [Export]
    Texture TextureDamaged;
    [Export]
    Texture TextureDamaged2;
    int Health = 3;
    Sprite Sprite;

    public override void _Ready()
    {
        Sprite = GetNode<Sprite>("Sprite");
        if(!Engine.EditorHint)
        {
            Sprite.Texture = Texture;
        }
    }

    public override void _Process(float delta)
    {
        if(Engine.EditorHint)
        {
            Sprite.Texture = Texture;
        }
    }

    public void Damage()
    {
        Health--;
        if(Health <= 0) QueueFree();
        else
        {  
            Texture texture;
            switch(Health)
            {
                default:
                case 3:
                    texture = Texture; 
                    break;
                case 2:
                    texture = TextureDamaged; 
                    break;
                case 1:
                    texture = TextureDamaged2; 
                    break;
            }
            Sprite.Texture = texture;
        }
    }
}
