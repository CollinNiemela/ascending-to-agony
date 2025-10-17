using Godot;

public partial class EndMenu : PanelContainer {
    //Exports
    [Export] private Button playAgainButton;
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
        if (playAgainButton != null) {
            playAgainButton.MouseEntered += onMouseEnteredButton;
            playAgainButton.Pressed += onPlayAgainButtonPressed;
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

    private void onPlayAgainButtonPressed() {
        menuClick.Play();
        Global global = GetNode<Global>("/root/Global");
        global.resetGame();
    }

    private void onMainMenuButtonPressed() {
        menuClick.Play();
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile(MAIN_MENU_PATH);
    }

    private void onQuitButtonPressed() { GetTree().Quit(); }

    private void onMouseEnteredButton() {
        menuHover.Play();
    }
}
