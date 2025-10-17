using Godot;
using System;

public partial class SettingsMenu : Control {
	[Signal]
	public delegate void BackPressedEventHandler();

	//Slider exports
	[Export] private HSlider masterVolumeSlider;
	[Export] private HSlider musicVolumeSlider;
	[Export] private HSlider soundEffectsSlider;

	//OptionButton exports
	[Export] private OptionButton screenSizeButton;
	[Export] private OptionButton windowModeButton;
	[Export] private OptionButton maxFpsButton;

	//CheckBox exports
	[Export] private CheckBox terminalBox;
	[Export] private CheckBox debugBox;

	//Button exports
	[Export] private Button backButton;

	//Data
	private Global global;
	private int masterBusIndex = -1;
	private int musicBusIndex = -1;
	private int sfxBusIndex = -1;

	//Constants
	private const string MASTER_BUS_NAME = "Master";
	private const string MUSIC_BUS_NAME = "Music";
	private const string SFX_BUS_NAME = "SFX";

	private const string WINDOW_MODE_FULLSCREEN = "Fullscreen";
	private const string WINDOW_MODE_BORDERED = "Bordered";
	private const string WINDOW_MODE_BORDERLESS = "Borderless";

	public override void _Ready() {
		global = GetNode<Global>("/root/Global");

		InitializeAudioBuses();
		ConnectSignals();
		LoadAndApplySettings();
	}

	//Get audio bus indices for performance (avoid repeated lookups)
	private void InitializeAudioBuses() {
		masterBusIndex = AudioServer.GetBusIndex(MASTER_BUS_NAME);
		musicBusIndex = AudioServer.GetBusIndex(MUSIC_BUS_NAME);
		sfxBusIndex = AudioServer.GetBusIndex(SFX_BUS_NAME);

		//warn if buses don't exist
		if (masterBusIndex == -1) GD.PrintErr($"Audio bus '{MASTER_BUS_NAME}' not found!");
		if (musicBusIndex == -1) GD.PrintErr($"Audio bus '{MUSIC_BUS_NAME}' not found!");
		if (sfxBusIndex == -1) GD.PrintErr($"Audio bus '{SFX_BUS_NAME}' not found!");
	}

	//Connect all UI signals using modern C# delegate syntax
	private void ConnectSignals() {
		//sound sliders - HSlider.ValueChanged passes double
		if (masterVolumeSlider != null)
			masterVolumeSlider.ValueChanged += (value) => onMasterVolumeSliderChanged((float)value);

		if (musicVolumeSlider != null)
			musicVolumeSlider.ValueChanged += (value) => onMusicVolumeSliderChanged((float)value);

		if (soundEffectsSlider != null)
			soundEffectsSlider.ValueChanged += (value) => onSoundEffectsVolumeSliderChanged((float)value);

		//option buttons - OptionButton.ItemSelected passes long (index)
		if (screenSizeButton != null)
			screenSizeButton.ItemSelected += (index) => onScreenSizeButtonItemSelected((int)index);

		if (windowModeButton != null)
			windowModeButton.ItemSelected += (index) => onWindowModeButtonItemSelected((int)index);

		if (maxFpsButton != null)
			maxFpsButton.ItemSelected += (index) => onMaxFpsButtonItemSelected((int)index);

		//checkboxes - CheckBox.Toggled passes bool
		if (terminalBox != null)
			terminalBox.Toggled += onTerminalBoxToggled;

		if (debugBox != null)
			debugBox.Toggled += onDebugBoxToggled;

		//back button - Button.Pressed passes no parameters
		if (backButton != null)
			backButton.Pressed += OnBackButtonPressed;
	}

	//Load settings from Global and apply them to the UI
	private void LoadAndApplySettings() {
		try {
			var userData = global.getUserData();

			//apply sound settings
			if (masterVolumeSlider != null)
				masterVolumeSlider.Value = userData["MasterVolume"].AsDouble();
			if (musicVolumeSlider != null)
				musicVolumeSlider.Value = userData["MusicVolume"].AsDouble();
			if (soundEffectsSlider != null)
				soundEffectsSlider.Value = userData["SoundEffectsVolume"].AsDouble();

			//apply video settings
			if (screenSizeButton != null)
				screenSizeButton.Selected = userData["ScreenSize"].AsInt32();
			if (windowModeButton != null)
				windowModeButton.Selected = userData["WindowMode"].AsInt32();
			if (maxFpsButton != null)
				maxFpsButton.Selected = userData["MaximumFPS"].AsInt32();

			//apply debug settings
			if (terminalBox != null)
				terminalBox.ButtonPressed = userData["EnableTerminal"].AsBool();
			if (debugBox != null)
				debugBox.ButtonPressed = userData["EnableDebugMenu"].AsBool();
		} catch (Exception e) {
			GD.PrintErr($"SettingsMenu: Failed to load settings, resetting to defaults - {e.Message}");
			global.resetData();

			//retry loading after reset
			try {
				LoadAndApplySettings();
			} catch (Exception retryException) {
				GD.PrintErr($"SettingsMenu: Failed to load settings even after reset - {retryException.Message}");
			}
		}
	}

	//Sound settings event handlers
	private void onMasterVolumeSliderChanged(float value) {
		global.addData("MasterVolume", (double)value);
		if (masterBusIndex >= 0)
			AudioServer.SetBusVolumeDb(masterBusIndex, value);
	}

	private void onMusicVolumeSliderChanged(float value) {
		global.addData("MusicVolume", (double)value);
		if (musicBusIndex >= 0)
			AudioServer.SetBusVolumeDb(musicBusIndex, value);
	}

	private void onSoundEffectsVolumeSliderChanged(float value) {
		global.addData("SoundEffectsVolume", (double)value);
		if (sfxBusIndex >= 0)
			AudioServer.SetBusVolumeDb(sfxBusIndex, value);
	}

	//Video settings event handlers
	private void onMaxFpsButtonItemSelected(int index) {
		global.addData("MaximumFPS", index);
		ApplyMaxFps();
	}

	private void onWindowModeButtonItemSelected(int index) {
		global.addData("WindowMode", index);
		ApplyWindowMode();
	}

	private void onScreenSizeButtonItemSelected(int index) {
		global.addData("ScreenSize", index);
		ApplyScreenSize();
	}

	//Debug settings event handlers
	private void onTerminalBoxToggled(bool pressed) {
		global.addData("EnableTerminal", pressed);
	}

	private void onDebugBoxToggled(bool pressed) {
		global.addData("EnableDebugMenu", pressed);
	}

	//Apply the maximum FPS setting
	private void ApplyMaxFps() {
		if (maxFpsButton == null) return;

		try {
			Engine.MaxFps = int.Parse(maxFpsButton.Text);
		} catch (Exception e) {
			GD.PrintErr($"SettingsMenu: Failed to parse MaxFps from button text '{maxFpsButton.Text}' - {e.Message}");
		}
	}

	//Apply the window mode setting
	private void ApplyWindowMode() {
		if (windowModeButton == null) return;

		switch (windowModeButton.Text) {
			case WINDOW_MODE_FULLSCREEN:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				break;

			case WINDOW_MODE_BORDERED:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
				CenterWindow();
				ApplyScreenSize();
				break;

			case WINDOW_MODE_BORDERLESS:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
				ApplyScreenSize();
				break;

			default:
				GD.PrintErr($"SettingsMenu: Unknown window mode '{windowModeButton.Text}'");
				break;
		}
	}

	//Apply the screen size setting
	private void ApplyScreenSize() {
		if (screenSizeButton == null) return;

		try {
			string[] sizeSplit = screenSizeButton.Text.Split('x');
			if (sizeSplit.Length != 2) {
				GD.PrintErr($"SettingsMenu: Invalid screen size format '{screenSizeButton.Text}'");
				return;
			}

			Vector2I size = new Vector2I(int.Parse(sizeSplit[0]), int.Parse(sizeSplit[1]));
			DisplayServer.WindowSetSize(size);
			CenterWindow();
		} catch (Exception e) {
			GD.PrintErr($"SettingsMenu: Failed to apply screen size '{screenSizeButton.Text}' - {e.Message}");
		}
	}

	//Center the window on the current screen
	private void CenterWindow() {
		int screen = DisplayServer.WindowGetCurrentScreen();
		Vector2I screenSize = DisplayServer.ScreenGetSize(screen);
		Vector2I screenPos = DisplayServer.ScreenGetPosition(screen);
		Vector2I windowSize = DisplayServer.WindowGetSize();
		DisplayServer.WindowSetPosition(screenPos + (screenSize - windowSize) / 2);
	}

	//Handle back button press - emit signal for parent to handle
	private void OnBackButtonPressed() {
		EmitSignal(SignalName.BackPressed);
	}
}
