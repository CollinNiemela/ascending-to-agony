using Godot;

public partial class Camera : Camera2D {
    private RigidBody2D player;

    public override void _Ready() {
        player = (RigidBody2D)GetTree().Root.GetNode(Global.GAME_SCENE).GetNode("Player");
    }

    public override void _Process(double delta) {
        if (IsInstanceValid(player)) {
            Vector2 cameraCenterPos = new Vector2(0, player.GlobalPosition.Y);
            float playerCameraOffset = (cameraCenterPos.X + player.GlobalPosition.X) / 2;
            Offset = new Vector2(Mathf.Round(playerCameraOffset), Mathf.Round(player.GlobalPosition.Y));
        }
    }
}

