using Godot;

public partial class HighResLayer : CanvasLayer {
	private Global global;
	private TerminalUI terminalUI;
	private PauseMenu pauseMenu;

	public override void _Ready() {
		terminalUI = GetNode<TerminalUI>("TerminalUI");
		pauseMenu = GetNode<PauseMenu>("GUI/PauseContainer");
		global = GetNode<Global>("/root/Global");

		//ensure terminal and pause menu start hidden
		terminalUI.Visible = false;
		Global.setTreePause(terminalUI, false);
		pauseMenu.Visible = false;
		ProcessMode = ProcessModeEnum.Always;
	}

	public override void _Process(double delta) {
		//pauses and hides the terminal if the shortcut is entered
		if ((bool)global.getData("EnableTerminal") && Input.IsActionJustPressed("pause_terminal")) {
			terminalUI.Visible = !terminalUI.Visible;
			Global.toggleTreePause(terminalUI);
			if (terminalUI.Visible == true) {
				terminalUI.grabFocus();
			}
		}

		//toggle pause menu with ESC key (only if terminal is not open)
		if (Input.IsActionJustPressed("ui_pause") && !terminalUI.Visible) {
			pauseMenu.Visible = !pauseMenu.Visible;
			//use Godot 4's built-in pause system - pauses all Pausable nodes
			GetTree().Paused = pauseMenu.Visible;
		}
	}
}
