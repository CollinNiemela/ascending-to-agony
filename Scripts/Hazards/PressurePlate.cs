using Godot;

public partial class PressurePlate : Area2D {
    [Export]
    private NodePath spikeTrapPath;
    private SpikeTrap spikeTrap;

    private AnimatedSprite2D plateSprite;
    private AudioStreamPlayer2D pressedSound;

    public override void _Ready() {
        spikeTrap = GetNode<SpikeTrap>(spikeTrapPath);
        spikeTrap.setMaxY(GlobalPosition.Y);
        plateSprite = GetNode<AnimatedSprite2D>("PlateSprite");
        pressedSound = GetNode<AudioStreamPlayer2D>("PressedSound");
    }

    private void onPressurePlateBodyEntered(Node body) {
        //if the player enters the plates collider it enables the
        //spike trap and sets the animation to the Pressed state
        if (body.IsInGroup("Player")) {
            pressedSound.Play();
            plateSprite.Animation = "Pressed";
            if (spikeTrap.getSlam() != true) spikeTrap.setSlam(true);
        }
    }

    private void onPressurePlateBodyExited(Node body) {
        //if the player leaves the plates collider it resets the animation
        if (body.IsInGroup("Player")) {
            plateSprite.Animation = "Unpressed";
        }
    }
}
