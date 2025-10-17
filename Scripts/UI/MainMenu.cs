using Godot;

public partial class MainMenu : Control {
	//Container exports
	[Export] private CenterContainer main;
	[Export] private SettingsMenu settings;
	[Export] private CenterContainer help;
	[Export] private CenterContainer credits;

	//Button exports
	[Export] private Button startButton;
	[Export] private Button settingsButton;
	[Export] private Button helpButton;
	[Export] private Button creditsButton;
	[Export] private Button quitButton;
	[Export] private Button helpBackButton;
	[Export] private Button creditsBackButton;

	//Audio exports
	[Export] private AudioStreamPlayer menuHover;
	[Export] private AudioStreamPlayer menuClick;

	//UI controls
	private ScrollContainer settingsScroll;

	public override void _Ready() {
		GetUIControls();
		ConnectSignals();
	}

	//Get non-exported UI controls
	private void GetUIControls() {
		if (settings != null) {
			settingsScroll = settings.GetNode<ScrollContainer>("ScrollContainer");
		}
	}

	//Connect all UI signals using modern C# delegate syntax
	private void ConnectSignals() {
		//main menu buttons
		if (startButton != null) {
			startButton.MouseEntered += onMouseEnteredButton;
			startButton.Pressed += onStartPressed;
		}

		if (settingsButton != null) {
			settingsButton.MouseEntered += onMouseEnteredButton;
			settingsButton.Pressed += onSettingsPressed;
		}

		if (helpButton != null) {
			helpButton.MouseEntered += onMouseEnteredButton;
			helpButton.Pressed += onHelpPressed;
		}

		if (creditsButton != null) {
			creditsButton.MouseEntered += onMouseEnteredButton;
			creditsButton.Pressed += onCreditsPressed;
		}

		if (quitButton != null) {
			quitButton.MouseEntered += onMouseEnteredButton;
			quitButton.Pressed += onQuitPressed;
		}

		//back buttons
		if (helpBackButton != null) {
			helpBackButton.MouseEntered += onMouseEnteredButton;
			helpBackButton.Pressed += onBackPressed;
		}

		if (creditsBackButton != null) {
			creditsBackButton.MouseEntered += onMouseEnteredButton;
			creditsBackButton.Pressed += onBackPressed;
		}

		if (settings != null) {
			settings.BackPressed += onBackPressed;
		}
	}

	//transitions to the given scene
	private void transitionTo(string sceneName) {
		menuClick.Play();
		main.Visible = false;
		settings.Visible = false;
		help.Visible = false;
		credits.Visible = false;
		switch (sceneName) {
			case "Main":
				main.Visible = true;
				break;
			case "Settings":
				settings.Visible = true;
				break;
			case "Help":
				help.Visible = true;
				break;
			case "Credits":
				credits.Visible = true;
				break;
		}
	}

	private void onBackPressed() { transitionTo("Main"); }

	private void onQuitPressed() { GetTree().Quit(); }

	//go to beginning scene when start is pressed
	private void onStartPressed() {
		menuClick.Play();
		GetTree().ChangeSceneToFile(Global.GAME_SCENE_FILE_PATH);
	}

	private void onSettingsPressed() {
		if (settingsScroll.ScrollVertical != 0) settingsScroll.ScrollVertical = 0;
		transitionTo("Settings");
	}

	private void onHelpPressed() { transitionTo("Help"); }

	private void onCreditsPressed() { transitionTo("Credits"); }

	private void onMouseEnteredButton() { menuHover.Play(); }
}
