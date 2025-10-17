using Godot;
using System;

public partial class GUI : Control {
	//Exports
	[Export] private Label fpsCounter;
	[Export] private Label velocityAmount;
	[Export] private Label playerPosition;
	[Export] private Control debugControl;

	//Data
	private Player player;
	private Global global;

	public override void _Ready() {
		player = (Player)GetTree().Root.GetNode(Global.GAME_SCENE).GetNode("Player");
		global = GetNode<Global>("/root/Global");
		debugControl.Visible = (bool)global.getData("EnableDebugMenu");
	}

	public override void _Process(double delta) {
		if (IsInstanceValid(player)) {
			if (debugControl.Visible) {
				velocityAmount.Text = Math.Floor(player.getVelocity()).ToString();
				playerPosition.Text = $"{Math.Floor(player.GlobalPosition.X)}, {Math.Floor(player.GlobalPosition.Y)}";
				fpsCounter.Text = Engine.GetFramesPerSecond().ToString();
			}
		}
	}

	//Updates bullet count display
	public void changeBulletCount(int count) {
		if (count == player.getMaxBulletCount()) {
			for (int i = 1; i < player.getMaxBulletCount() + 1; i++) {
				Sprite2D bullet = GetNode<Sprite2D>("BulletGUI/Bullet" + i);
				bullet.SelfModulate = new Color(1, 1f, 1f, 1f);
			}
		} else {
			Sprite2D bullet = GetNode<Sprite2D>("BulletGUI/Bullet" + (count + 1));
			bullet.SelfModulate = new Color(1, 1f, 1f, 0.25f);
		}
	}
}
