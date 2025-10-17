using Godot;

public partial class BackgroundSaw : Sprite2D
{
    private int speed;

    public override void _Ready() { speed = 500; }

    public override void _Process(double delta)
    {
        RotationDegrees += (float)(delta * speed);
    }
}
