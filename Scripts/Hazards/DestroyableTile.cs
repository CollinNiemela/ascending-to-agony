using Godot;

public partial class DestroyableTile : StaticBody2D {
    [Export(PropertyHint.Enum, "Single Tile, Left Tile, Middle Tile, Right Tile")]
    private int TileType = 0;

    private Timer waitTimer;
    private Timer resetTimer;
    private CollisionShape2D collision;
    private bool playerInside;
    private bool timerExecuted;
    private Sprite2D sprite;

    public override void _Ready() {
        sprite = GetNode<Sprite2D>("Sprite2D");
        sprite.Frame = TileType;
        waitTimer = GetNode<Timer>("WaitTimer");
        resetTimer = GetNode<Timer>("ResetTimer");
        collision = GetNode<CollisionShape2D>("CollisionShape2D");
        playerInside = false;
        timerExecuted = false;
    }

    private void onArea2DBodyEntered(Node body) {
        if (body.IsInGroup("Player")) {
            waitTimer.Start();
            playerInside = true;
        }
    }

    private void onArea2DBodyExited(Node body) {
        if (body.IsInGroup("Player")) {
            playerInside = false;
            if (timerExecuted) {
                sprite.Frame = TileType;
                Visible = true;
                collision.SetDeferred("disabled", false);
                timerExecuted = false;
            } else {
                resetTimer.Start();
            }
        }
    }

    private void onWaitTimerTimeout() {
        if (sprite.Frame == (TileType + 4)) {
            //half broken
            if (playerInside) {
                Visible = false;
                collision.SetDeferred("disabled", true);
            }
        } else if (Visible == false) {
            //fully broken
            if (!playerInside) {
                sprite.Frame = TileType;
                Visible = true;
                collision.SetDeferred("disabled", false);
            } else {
                timerExecuted = true;
            }
        } else {
            //not broken
            if (playerInside) {
                sprite.Frame = TileType + 4;
                waitTimer.Start();
            }
        }
    }

    private void onResetTimerTimeout() {
        if (!playerInside) {
            sprite.Frame = TileType;
        }
    }
}
