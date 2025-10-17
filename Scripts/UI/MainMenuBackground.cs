using Godot;

public partial class MainMenuBackground : Control {
    private int speed;
    private int resetX;

    public override void _Ready() {
        speed = 50;
        resetX = -8832;
    }

    public override void _Process(double delta) {
        //X scrolling
        //loops the background so it moves forever
        GlobalPosition = new Vector2(Mathf.Clamp(GlobalPosition.X - (float)(delta * speed), resetX, 0), GlobalPosition.Y);
        if (GlobalPosition.X == resetX) {
            GlobalPosition = new Vector2(0, GlobalPosition.Y);
        }
    }
}
