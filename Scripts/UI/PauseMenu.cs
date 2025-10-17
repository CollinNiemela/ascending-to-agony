using Godot;

public partial class PauseMenu : PanelContainer {
	//Exports
	[Export] private Button resumeButton;
	[Export] private Button mainMenuButton;
	[Export] private Button quitButton;

	//UI controls
	private const string MAIN_MENU_PATH = "res://Scenes/Menus/MainMenu.tscn";
	private AudioStreamPlayer menuHover;
	private AudioStreamPlayer menuClick;

	public override void _Ready() {
		//set process mode so this menu works when game is paused
		ProcessMode = ProcessModeEnum.WhenPaused;

		//get non-exported nodes
		menuHover = GetNode<AudioStreamPlayer>("MenuHovering");
		menuClick = GetNode<AudioStreamPlayer>("MenuClick");

		//connect signals
		ConnectSignals();
	}

	//Connect all UI signals using modern C# delegate syntax
	private void ConnectSignals() {
		if (resumeButton != null) {
			resumeButton.MouseEntered += onMouseEnteredButton;
			resumeButton.Pressed += onResumeButtonPressed;
		}

		if (mainMenuButton != null) {
			mainMenuButton.MouseEntered += onMouseEnteredButton;
			mainMenuButton.Pressed += onMainMenuButtonPressed;
		}

		if (quitButton != null) {
			quitButton.MouseEntered += onMouseEnteredButton;
			quitButton.Pressed += onQuitButtonPressed;
		}
	}

	//Button handlers
	private void onResumeButtonPressed() {
		menuClick.Play();
		//hide pause menu
		Visible = false;
		//unpause the game
		GetTree().Paused = false;
	}

	private void onMainMenuButtonPressed() {
		menuClick.Play();
		//unpause before changing scenes
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile(MAIN_MENU_PATH);
	}

	private void onQuitButtonPressed() {
		GetTree().Quit();
	}

	private void onMouseEnteredButton() {
		menuHover.Play();
	}
}
