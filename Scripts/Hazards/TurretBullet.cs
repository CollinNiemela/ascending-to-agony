using Godot;

public partial class TurretBullet : Area2D {
    private int speed = 2500;

    public override void _PhysicsProcess(double delta) {
        Position += Transform.X * speed * (float)delta;
    }

    private void onBulletBodyEntered(Node body) {
        if (body.IsInGroup("Player")) {
            Player player = (Player)body;
            if (player.getInvincibilityToggle()) return;
            Global global = GetNode<Global>("/root/Global");
            global.EmitSignal(Global.SignalName.ResetPlayer);
        }
        QueueFree();
    }

    public void setSpeed(int bulletSpeed) {
        speed = bulletSpeed;
    }
}
