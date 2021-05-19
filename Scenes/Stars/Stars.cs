using System;
using Godot;

public class Stars : Sprite {

    [Export]
    Vector2 velocity = new Vector2 (0, 50);
    [Export]
    int initYPos = -200;
    [Export]
    int height = 30;
    private Vector2 viewSize = new Vector2 ();
    public override void _Ready () {
        SetProcess (true);
        viewSize = GetViewportRect ().Size;
    }

    public override void _Process (float delta) {
        Translate (velocity * delta);
        var pos = Position.y;
        var posStart = new Vector2 (Position.x, initYPos);
        var posEnd = viewSize.y + height;

        if (pos > posEnd)
            Position = posStart;

    }
}