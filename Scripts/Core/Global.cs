using Godot;

//Global singleton for persistent user settings and game state management
public partial class Global : Node {
    public const string GAME_SCENE = "MainScene";
    public const string GAME_SCENE_FILE_PATH = "res://Scenes/Level/MainScene.tscn";
    private const string USER_SAVE = "user://UserData.save";
    private Godot.Collections.Dictionary<string, Variant> userData;

    [Signal]
    public delegate void ResetPlayerEventHandler();

    public Global() {
        userData = new Godot.Collections.Dictionary<string, Variant>();
        loadData();
    }

    //loads the users data from save file or creates default values
    public void loadData() {
        if (!FileAccess.FileExists(USER_SAVE)) {
            //setting default save values if save is not found
            addData("MasterVolume", 0d);
            addData("MusicVolume", 0d);
            addData("SoundEffectsVolume", 0d);
            addData("ScreenSize", 0);
            addData("WindowMode", 0);
            addData("MaximumFPS", 0);
            addData("EnableTerminal", false);
            addData("EnableDebugMenu", false);
            saveData();
        }
        FileAccess file = FileAccess.Open(USER_SAVE, FileAccess.ModeFlags.Read);
        userData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)file.GetVar());
        file.Close();
    }

    //saves the users data to disk
    public void saveData() {
        FileAccess file = FileAccess.Open(USER_SAVE, FileAccess.ModeFlags.Write);
        file.StoreVar(userData);
        file.Close();
    }

    //adds or updates a setting value and saves immediately
    public void addData(string name, Variant data) {
        userData.Remove(name);
        userData.Add(name, data);
        saveData();
    }

    //resets all settings to default values
    public void resetData() {
        if (FileAccess.FileExists(USER_SAVE)) {
            addData("MasterVolume", 0d);
            addData("MusicVolume", 0d);
            addData("SoundEffectsVolume", 0d);
            addData("ScreenSize", 0);
            addData("WindowMode", 0);
            addData("MaximumFPS", 0);
            addData("EnableTerminal", false);
            addData("EnableDebugMenu", false);
            saveData();
        }
    }

    //returns the entire user data dictionary
    public Godot.Collections.Dictionary<string, Variant> getUserData() { return userData; }

    //gets a specific setting value by key name, returns default if not found
    public Variant getData(string name) { return userData.ContainsKey(name) ? userData[name] : default; }

    //displays the win screen when player reaches the goal
    public void enableWonMessage(Player player) {
        EndMenu endContainer = GetTree().Root.GetNode(GAME_SCENE).GetNode<EndMenu>("HighResLayer/GUI/EndContainer");
        GetTree().Root.GetNode(GAME_SCENE).GetNode<AudioStreamPlayer>("GameWonSound").Play();
        GetTree().Paused = true;
        endContainer.Visible = true;
    }

    //resets the game state and player position
    public void resetGame() {
        GetTree().Paused = false;

        GetTree().Root.GetNode(GAME_SCENE).GetNode<Player>("Player").resetPlayer();
        GetTree().Root.GetNode(GAME_SCENE).GetNode<EndMenu>("HighResLayer/GUI/EndContainer").Visible = false;
    }

    //pauses all the nodes in the given node tree
    public static void toggleTreePause(Node node) {
        setTreePause(node, !node.IsProcessing());
    }

    //sets the pause mode of the given node
    public static void setNodePause(Node node, bool pause) {
        node.SetProcess(pause);
        node.SetPhysicsProcess(pause);
        node.SetProcessInput(pause);
        node.SetProcessInternal(pause);
        node.SetProcessUnhandledInput(pause);
        node.SetProcessUnhandledKeyInput(pause);
    }

    //sets the pause mode of the entire given nodes tree
    public static void setTreePause(Node node, bool pause) {
        setNodePause(node, pause);
        foreach (Node child in node.GetChildren()) {
            setNodePause(child, pause);
        }
    }
}
