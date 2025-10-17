using Godot;

public partial class SpikeTrap : Area2D {
    private float maxY;
    private bool slam;
    private Line2D trapBase;
    private Vector2 originalPos;
    private int baseOffset;
    private bool goDown;
    private float lerpValue;
    private float lerpDuration;
    private int lerpPixelsPerSecond;

    public override void _Ready() {
        maxY = 0;
        slam = false;
        trapBase = GetNode<Line2D>("TrapBase");
        originalPos = GlobalPosition;
        baseOffset = -49;
        goDown = true;
        lerpDuration = 0f;
        lerpValue = 0f;
        lerpPixelsPerSecond = 600;
    }

    public override void _Process(double delta) {
        //moves down if the pressure plate is activated
        //then goes back up when it hits the ground
        if (slam) {
            if (goDown) {
                lerpValue = Mathf.Clamp((float)delta + lerpValue, 0, lerpDuration);
                GlobalPosition = new Vector2(GlobalPosition.X, Mathf.Lerp(originalPos.Y, maxY, lerpValue / lerpDuration));
                Vector2 localPos = trapBase.ToLocal(originalPos);
                trapBase.AddPoint(new Vector2(localPos.X, Mathf.Floor(localPos.Y + baseOffset)));
                trapBase.RemovePoint(1);
                if (lerpValue == lerpDuration) goDown = false;
            } else {
                lerpValue = Mathf.Clamp(lerpValue - (float)delta, 0, lerpDuration);
                GlobalPosition = new Vector2(GlobalPosition.X, Mathf.Lerp(originalPos.Y, maxY, lerpValue / lerpDuration));
                Vector2 localPos = trapBase.ToLocal(originalPos);
                trapBase.AddPoint(new Vector2(localPos.X, Mathf.Floor(localPos.Y + baseOffset)));
                trapBase.RemovePoint(1);
                if (lerpValue == 0) {
                    goDown = true;
                    slam = false;
                }
            }
        }
    }

    private void onSpikeTrapBodyEntered(Node body) {
        //if the player enters the traps collider then it ends the level
        if (body.IsInGroup("Player")) {
            Player player = (Player)body;
            if (player.getInvincibilityToggle()) return;
            Global global = GetNode<Global>("/root/Global");
            global.EmitSignal(Global.SignalName.ResetPlayer);
        }
    }

    public void setSlam(bool s) { slam = s; }

    public bool getSlam() { return slam; }

    public void setMaxY(float y) {
        //sets the max y that the trap can go to then sets the correct
        //lerp duration depending on the set lerp pixels per second
        maxY = y;
        lerpDuration = (maxY - originalPos.Y) / lerpPixelsPerSecond;
    }
}
