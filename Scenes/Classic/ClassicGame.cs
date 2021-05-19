using Godot;
using System;

public class ClassicGame : Node2D
{
    [Export]
    PackedScene BonusScene;
    Label ScoreLabel;
    Label LivesLabel;
    ClassicPlayer Player;
    Timer MoveTimer;
    Timer BonusTimer;
    Node2D Enemies;
    int EnemyCount;
    int Level = 1;
    AudioStreamPlayer StepSound;
    Control GameOver;
    Label GameOverLabel;
    Label GameOverScoreLabel;
    public override void _Ready()
    {
        ScoreLabel = GetNode<Label>("HUD/Score");
        LivesLabel = GetNode<Label>("HUD/Lives");
        Player = GetNode<ClassicPlayer>("ClassicPlayer");
        Player.Game = this;
        MoveTimer = GetNode<Timer>("MoveTimer");
        BonusTimer = GetNode<Timer>("BonusTimer");
        Enemies = GetNode<Node2D>("Enemies");
        StepSound = GetNode<AudioStreamPlayer>("StepSound");
        EnemyCount = Enemies.GetChildren().Count;

        GameOver = GetNode<Control>("HUD/GameOver");
        GameOverLabel = GetNode<Label>("HUD/GameOver/Box/Text");
        GameOverScoreLabel = GetNode<Label>("HUD/GameOver/Box/Score");
    }

    public override void _Process(float delta)
    {
        ScoreLabel.Text = "Score: " + Player.Score;
        LivesLabel.Text = "Lives: " + Player.Lives;
    }

    bool MoveRight = true;
    void Move()
    {
        var children = Enemies.GetChildren();
        if (children.Count != 0)
        {
            StepSound.Seek(0);
            StepSound.Play();
            foreach (var c in children)
            {
                if (c is ClassicEnemy e)
                    e.Tick();
            }

            var stepSize = 8;
            if (MoveRight)
            {
                var max = GetMaxPosition(children);
                var target = 512 - max - 32;
                if (Enemies.Position.x >= (target))
                {
                    MoveRight = !MoveRight;
                    Enemies.Position = new Vector2(Enemies.Position.x, Enemies.Position.y + 32);
                    var h = GetMaxHeight(children);
                    if(h >= 224)
                    {
                        Lose();
                        return;
                    }
                }
            }
            else
            {
                var min = GetMinPosition(children);
                var target = -512 - min + 32;
                if (Enemies.Position.x <= (target))
                {
                    MoveRight = !MoveRight;
                    Enemies.Position = new Vector2(Enemies.Position.x, Enemies.Position.y + 32);
                    var h = GetMaxHeight(children);
                    if(h >= 224)
                    {
                        Lose();
                        return;
                    }
                }
            }

            if (MoveRight) Enemies.Position = new Vector2(Enemies.Position.x + stepSize, Enemies.Position.y);
            else Enemies.Position = new Vector2(Enemies.Position.x - stepSize, Enemies.Position.y);

            var time = ((float)Enemies.GetChildren().Count / EnemyCount) * 0.8f * (0.85f / Level);
            MoveTimer.Start(time);
        }
        else
        {
            Win();
        }
    }

    void Bonus()
    {
        var bonus = BonusScene.Instance() as ClassicBonus;
        AddChild(bonus);
        var rand = new Random();
        bonus.Position = new Vector2(rand.Next(0,2) == 1 ? -468 : -468, -460);
        BonusTimer.Start(rand.Next(10,26));
    }

    float GetMinPosition(Godot.Collections.Array children)
    {
        var min = 0f;
        foreach (ClassicEnemy c in children)
        {
            if (c.Position.x < min)
                min = c.Position.x;
        }
        return min;
    }
    float GetMaxPosition(Godot.Collections.Array children)
    {
        var max = 0f;
        foreach (ClassicEnemy c in children)
        {
            if (c.Position.x > max)
                max = c.Position.x;
        }
        return max;
    }

    float GetMaxHeight(Godot.Collections.Array children)
    {
        var max = 0f;
        foreach (ClassicEnemy c in children)
        {
            if (c.GlobalPosition.y > max)
                max = c.GlobalPosition.y;
        }
        return max;
    }

    public void Lose()
    {
        MoveTimer.Stop();
        Player.QueueFree();
        GameOver.Visible = true;
        GameOverLabel.Text = "You lost!";
        GameOverScoreLabel.Text = ScoreLabel.Text;
    }
    public void Win()
    {
        MoveTimer.Stop();
        GameOver.Visible = true;
        GameOverLabel.Text = "You won!";
        GameOverScoreLabel.Text = ScoreLabel.Text;
    }
    void Reload()
    {
        GetTree().ReloadCurrentScene();
    }
    void MainMenu()
    {
        GetTree().ChangeScene("res://Scenes/MainMenu/MainMenu.tscn");
    }
}