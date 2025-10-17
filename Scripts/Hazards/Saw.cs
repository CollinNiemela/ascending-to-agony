using Godot;

public partial class Saw : Area2D {
    private int speed;

    public override void _Ready() {
        speed = 500;
    }

    public override void _Process(double delta) {
        RotationDegrees += (float)(delta * speed);
    }

    private void onSawBodyEntered(Node body) {
        if (body.IsInGroup("Player")) {
            Player player = (Player)body;
            if (player.getInvincibilityToggle()) return;
            Global global = GetNode<Global>("/root/Global");
            global.EmitSignal(Global.SignalName.ResetPlayer);
        }
    }
}
