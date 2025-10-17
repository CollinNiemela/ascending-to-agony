using Godot;

public partial class Player : RigidBody2D {
    [Export]
    private PackedScene bulletScene;
    [Export]
    private int maxForceSpeed = 1500;
    [Export]
    private int maxFallSpeed = 5000;
    [Export]
    private float forceAmount = 23000f;
    [Export]
    private bool isInvincible = false;

    private Sprite2D weaponSprite;
    private RayCast2D floorRayCast;
    private GUI gui;
    private AudioStreamPlayer2D shootAudio;
    private AudioStreamPlayer2D deathAudio;
    private int maxBullets;
    private int bullets;
    private bool clicked;
    private bool shouldReset;
    private Vector2 originalPosition;

    public override void _Ready() {
        //sets players variables
        weaponSprite = GetNode<Sprite2D>("WeaponSprite");
        floorRayCast = GetNode<RayCast2D>("FloorRayCast");
        gui = GetParent().GetNode<GUI>("HighResLayer/GUI");
        shootAudio = GetNode<AudioStreamPlayer2D>("ShootAudio");
        deathAudio = GetNode<AudioStreamPlayer2D>("DeathAudio");
        maxBullets = 6;
        bullets = 6;
        clicked = false;
        shouldReset = false;
        originalPosition = GlobalPosition;

        //connect reset player signal to global singleton
        Global global = GetNode<Global>("/root/Global");
        global.ResetPlayer += resetPlayer;
    }

    public override void _ExitTree() {
        //disconnect from global signal when player is destroyed
        Global global = GetNode<Global>("/root/Global");
        global.ResetPlayer -= resetPlayer;
    }

    public override void _PhysicsProcess(double delta) {
        //sets weapon and raycast direction
        weaponSprite.LookAt(GetGlobalMousePosition());
        floorRayCast.RotationDegrees = -RotationDegrees;

        //If the player is not on the floor and velocity is 0 then give the player a little push
        if (!floorRayCast.IsColliding() && Mathf.Floor(Mathf.Abs(LinearVelocity.X) + Mathf.Abs(LinearVelocity.Y)) == 0) {
            ApplyCentralImpulse(new Vector2(1, 1));
        }

        //checks if the player is on the floor and not moving then regens bullets
        if (floorRayCast.IsColliding() && Mathf.Abs(LinearVelocity.X) + Mathf.Abs(LinearVelocity.Y) < 10 && bullets != maxBullets) {
            bullets = maxBullets;
            gui.changeBulletCount(bullets);
        }

        //shoot weapon if player has enough bullets
        if (bullets > 0 && Input.IsActionJustPressed("mouseClick")) {
            ApplyCentralImpulse(-1f * GlobalPosition.DirectionTo(GetGlobalMousePosition()) * forceAmount);
            clicked = true;
            shootBullet();
        }
    }

    //sets a max speed for the player
    public override void _IntegrateForces(PhysicsDirectBodyState2D state) {
        //handle reset request
        if (shouldReset) {
            state.Transform = new Transform2D(0f, originalPosition);
            state.LinearVelocity = Vector2.Zero;
            state.AngularVelocity = 0f;
            shouldReset = false;
        }

        //checks if the mouse has been clicked then reduces speed if too fast
        if (clicked) {
            if (state.LinearVelocity.Length() > maxForceSpeed) {
                state.LinearVelocity = state.LinearVelocity.Normalized() * maxForceSpeed;
            }
            clicked = false;
        }
        if (state.LinearVelocity.Length() > maxFallSpeed) {
            state.LinearVelocity = state.LinearVelocity.Normalized() * maxFallSpeed;
        }
    }

    private void shootBullet() {
        Area2D bullet = (Area2D)bulletScene.Instantiate();
        GetParent().AddChild(bullet);
        Marker2D bulletSpawn = (Marker2D)GetNode("WeaponSprite/Marker2D");
        bullet.GlobalTransform = bulletSpawn.GlobalTransform;
        bullets--;
        gui.changeBulletCount(bullets);
        shootAudio.Play();
    }

    public int getBulletCount() { return bullets; }

    public int getMaxBulletCount() { return maxBullets; }

    public void setMaxBulletCount(int max) { maxBullets = max; }

    public void setMaxSpeed(int speed) { maxForceSpeed = speed; }

    public void setMass(float mass) { Mass = mass; }

    public void setGravity(float gravity) { GravityScale = gravity; }

    public void setDamp(float damp) { AngularDamp = damp; }

    public void setForceAmount(float force) { forceAmount = force; }

    public void setPosition(float x, float y) { GlobalPosition = new Vector2(x, y); }

    public float getVelocity() { return Mathf.Abs(LinearVelocity.X) + Mathf.Abs(LinearVelocity.Y); }

    public void toggleInvincibility() { isInvincible = !isInvincible; }

    public bool getInvincibilityToggle() { return isInvincible; }

    public void playDeathSound() { deathAudio.Play(); }

    public void resetPlayer() {
        // Remove all bullets (player + turret) from the scene
        var sceneBullets = GetTree().GetNodesInGroup("Bullet");
        foreach (Node bullet in sceneBullets) {
            bullet.QueueFree();
        }
        Sleeping = false;
        shouldReset = true;
        bullets = maxBullets;
        gui.changeBulletCount(bullets);
        playDeathSound();
    }
}
