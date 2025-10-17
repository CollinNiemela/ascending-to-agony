using Godot;

public partial class PlayerBullet : Area2D
{
    private int speed = 4000;

    public override void _PhysicsProcess(double delta)
    {
        Position += Transform.X * speed * (float)delta;
    }

    private void onBulletBodyEntered(Node body)
    {
        QueueFree();
    }

    private void onBulletAreaEntered(Node area)
    {
        if (area.IsInGroup("Turret"))
        {
            Turret turret = (Turret)area;
            turret.DestroyTurret();
        }
        QueueFree();
    }

    public void setSpeed(int bulletSpeed)
    {
        speed = bulletSpeed;
    }
}
