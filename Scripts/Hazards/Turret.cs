using Godot;
using System;

//Automated turret that detects and shoots at the player within range
public partial class Turret : Area2D {
    [Export] private float xDistance = 256f; //horizontal detection range from turret center
    [Export] private float yDistance = 256f; //vertical detection range (downward only)
    [Export] private PackedScene bulletScene; //bullet scene to instantiate when shooting
    [Export] private float rotationSpeed = 100f; //degrees per second rotation speed
    [Export] private float waitTimeBeforeShoot = 0.3f; //delay before shooting after aiming at player
    [Export] private float shootCooldown = 1.0f; //cooldown between shots
    [Export] private float respawnDelay = 3.0f; //time turret stays destroyed before respawning
    [Export] private float aimingTolerance = 1f; //angle tolerance for considering turret "aimed" at target (degrees)

    private Timer destroyedTimer;
    private Timer shootTimer;
    private Timer waitTimer;
    private Sprite2D weaponSprite;
    private RayCast2D playerRayCast;
    private AudioStreamPlayer2D shootAudio;
    private Marker2D bulletSpawn;
    private Player player;

    //State tracking
    private State currentState;
    private bool bulletShot;
    private const float IDLE_ROTATION_DEGREES = 90f;

    //Turret states
    private enum State {
        Idle,           //waiting for player, rotating to idle position
        FollowPlayer,   //tracking and shooting at player
        Destroyed       //temporarily disabled
    }

    public override void _Ready() {
        GetNodeReferences();
        InitializeTimers();
        ConnectSignals();
        InitializeState();
    }

    public override void _Process(double delta) {
        switch (currentState) {
            case State.Idle:
                ProcessIdleState(delta);
                break;

            case State.FollowPlayer:
                ProcessFollowPlayerState(delta);
                break;

            case State.Destroyed:
                //inactive - waiting for respawn
                break;
        }
    }

    //Gets all node references
    private void GetNodeReferences() {
        destroyedTimer = GetNode<Timer>("DestroyedTimer");
        shootTimer = GetNode<Timer>("ShootTimer");
        waitTimer = GetNode<Timer>("WaitTimer");
        weaponSprite = GetNode<Sprite2D>("WeaponSprite");
        playerRayCast = GetNode<RayCast2D>("PlayerRayCast");
        shootAudio = GetNode<AudioStreamPlayer2D>("ShootAudio");
        bulletSpawn = GetNode<Marker2D>("WeaponSprite/Marker2D");

        //finds the players node
        try {
            player = GetTree().Root.GetNode(Global.GAME_SCENE)?.GetNode<Player>("Player");
            if (player == null) {
                GD.PrintErr("Turret: Could not find Player node");
            }
        } catch (Exception e) {
            GD.PrintErr($"Turret: Failed to get player reference - {e.Message}");
        }
    }

    //Configure timer settings from exports
    private void InitializeTimers() {
        if (waitTimer != null)
            waitTimer.WaitTime = waitTimeBeforeShoot;

        if (shootTimer != null)
            shootTimer.WaitTime = shootCooldown;

        if (destroyedTimer != null)
            destroyedTimer.WaitTime = respawnDelay;
    }

    //Connect timer signals
    private void ConnectSignals() {
        if (shootTimer != null)
            shootTimer.Timeout += OnShootTimerTimeout;

        if (waitTimer != null)
            waitTimer.Timeout += OnWaitTimerTimeout;

        if (destroyedTimer != null)
            destroyedTimer.Timeout += OnDestroyedTimerTimeout;
    }

    //Set initial state
    private void InitializeState() {
        currentState = State.Idle;
        bulletShot = false;
    }

    //Sets idle state to return to center and check for player
    private void ProcessIdleState(double delta) {
        //check if player is in range and visible
        if (IsPlayerInRangeAndVisible()) {
            currentState = State.FollowPlayer;
            return;
        }

        //return weapon to idle position (facing down)
        RotateTowardsAngle(IDLE_ROTATION_DEGREES, delta);
    }

    //Process follow player state to track, aim, and shoot
    private void ProcessFollowPlayerState(double delta) {
        //check if player is still in range and visible
        if (!IsPlayerInRangeAndVisible()) {
            currentState = State.Idle;
            return;
        }

        //aim at player and shoot when locked on
        if (!bulletShot) {
            bool isAimed = AimAtPlayer(delta);

            if (isAimed) {
                //start wait timer before shooting
                waitTimer?.Start();
                bulletShot = true;
            }
        }
    }

    //Check if player is in detection range and visible (no obstructions)
    private bool IsPlayerInRangeAndVisible() {
        if (player == null) return false;

        return IsPlayerInRange() && IsPlayerVisible();
    }

    //Check if player is within the turret's detection rectangle
    private bool IsPlayerInRange() {
        if (player == null) return false;

        Vector2 playerPos = player.GlobalPosition;
        Vector2 turretPos = GlobalPosition;

        //player must be below turret
        if (playerPos.Y <= turretPos.Y) return false;

        //check horizontal range
        if (playerPos.X < turretPos.X - xDistance || playerPos.X > turretPos.X + xDistance)
            return false;

        //check vertical range
        if (playerPos.Y - turretPos.Y > yDistance)
            return false;

        return true;
    }

    //Check if player is visible (no obstacles blocking line of sight)
    private bool IsPlayerVisible() {
        if (player == null || playerRayCast == null) return false;

        playerRayCast.TargetPosition = playerRayCast.ToLocal(player.GlobalPosition);
        playerRayCast.ForceRaycastUpdate(); //update raycast immediately

        //if nothing was hit, player is visible
        if (!playerRayCast.IsColliding())
            return true;

        //check if we hit the player directly (not a wall/obstacle)
        GodotObject collider = playerRayCast.GetCollider();
        return collider == player;
    }

    //Aim weapon at player, returns true when locked on target
    private bool AimAtPlayer(double delta) {
        if (player == null || weaponSprite == null) return false;

        //calculate angle to player
        float targetAngle = CalculateAngleToPlayer();
        float currentAngle = weaponSprite.RotationDegrees;
        float angleDifference = Mathf.Abs(targetAngle - currentAngle);

        //check if already aimed
        if (angleDifference <= aimingTolerance) {
            return true; //locked on target
        }

        //rotate towards target
        RotateTowardsAngle(targetAngle, delta);
        return false; //still aiming
    }

    //Calculate angle from weapon to player in degrees
    private float CalculateAngleToPlayer() {
        if (player == null || weaponSprite == null) return IDLE_ROTATION_DEGREES;

        float angleRadians = weaponSprite.GlobalPosition.AngleToPoint(player.GlobalPosition);
        return Mathf.RadToDeg(angleRadians);
    }

    //Rotate weapon sprite towards target angle at configured rotation speed
    private void RotateTowardsAngle(float targetAngle, double delta) {
        if (weaponSprite == null) return;

        float currentAngle = weaponSprite.RotationDegrees;

        //already at target angle (with small tolerance)
        if (Mathf.Abs(targetAngle - currentAngle) < 0.1f) {
            weaponSprite.RotationDegrees = targetAngle;
            return;
        }

        //determine rotation direction and apply rotation
        float rotationStep = rotationSpeed * (float)delta;
        float direction = targetAngle > currentAngle ? 1f : -1f;
        weaponSprite.RotationDegrees += direction * rotationStep;

        //snap to target if close enough
        if (Mathf.Abs(targetAngle - weaponSprite.RotationDegrees) < 0.5f) {
            weaponSprite.RotationDegrees = targetAngle;
        }
    }

    //Instantiate and fire a bullet
    private void ShootBullet() {
        if (bulletScene == null || bulletSpawn == null) {
            GD.PrintErr("Turret: Cannot shoot - bulletScene or bulletSpawn is null");
            return;
        }

        //instantiate bullet
        Area2D bullet = (Area2D)bulletScene.Instantiate();
        GetParent().AddChild(bullet);
        bullet.GlobalTransform = bulletSpawn.GlobalTransform;

        //start cooldown and play audio
        shootTimer?.Start();
        shootAudio?.Play();
    }

    //Called when shoot cooldown expires - allow next shot
    private void OnShootTimerTimeout() {
        bulletShot = false;
    }

    //Called when aim wait time expires - fire bullet
    private void OnWaitTimerTimeout() {
        ShootBullet();
    }

    //Called when destroyed timer expires - respawn turret
    private void OnDestroyedTimerTimeout() {
        currentState = State.Idle;

        if (weaponSprite != null) {
            weaponSprite.Visible = true;
            weaponSprite.RotationDegrees = IDLE_ROTATION_DEGREES;
        }
    }

    //Temporarily destroy the turret (hides sprite, disables shooting)
    public void DestroyTurret() {
        currentState = State.Destroyed;

        if (weaponSprite != null)
            weaponSprite.Visible = false;

        destroyedTimer?.Start();
    }
}
