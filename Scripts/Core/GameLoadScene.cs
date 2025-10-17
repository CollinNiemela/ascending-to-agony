using Godot;

public partial class GameLoadScene : Node {
	private const string FIRST_SCENE = "res://Scenes/Menus/MainMenu.tscn";
	private Global global;

	public override void _Ready() {
		//gets the global node that has the data
		global = GetNode<Global>("/root/Global");

		//setting the video settings correctly
		setFps();
		setWindowMode();
		setScreenSize();
		setSound();

		//goes to the next scene (deferred to avoid node tree conflicts)
		GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, FIRST_SCENE);
	}

	private void setFps() {
		int[] maxFpsNumbers = { 480, 240, 144, 60 };
		try {
			Engine.MaxFps = maxFpsNumbers[global.getData("MaximumFPS").AsInt32()];
		} catch {
			Engine.MaxFps = maxFpsNumbers[0];
		}
	}

	private void setWindowMode() {
		string[] windowModes = { "Fullscreen", "Bordered", "Borderless" };
		string windowMode = windowModes[0];
		try {
			windowMode = windowModes[global.getData("WindowMode").AsInt32()];
		} catch { }
		switch (windowMode) {
			case "Fullscreen":
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				break;
			case "Bordered":
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
				break;
			case "Borderless":
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
				break;
		}
	}

	private void setScreenSize() {
		string[] screenSizes = { "1920x1080", "1664x936", "1280x720", "1024x576" };
		string screenSize = screenSizes[0];
		try {
			screenSize = screenSizes[global.getData("ScreenSize").AsInt32()];
		} catch { }
		string[] screenSizeSplit = screenSize.Split("x");
		Vector2I size = new Vector2I(int.Parse(screenSizeSplit[0]), int.Parse(screenSizeSplit[1]));
		DisplayServer.WindowSetSize(size);
		int screen = DisplayServer.WindowGetCurrentScreen();
		Vector2I screenSize2 = DisplayServer.ScreenGetSize(screen);
		Vector2I screenPos = DisplayServer.ScreenGetPosition(screen);
		DisplayServer.WindowSetPosition(screenPos + (screenSize2 - size) / 2);
	}

	private void setSound() {
		try {
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), global.getData("MasterVolume").AsSingle());
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), global.getData("MusicVolume").AsSingle());
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), global.getData("SoundEffectsVolume").AsSingle());
		} catch {
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), 0f);
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), 0f);
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), 0f);
		}
	}
}
