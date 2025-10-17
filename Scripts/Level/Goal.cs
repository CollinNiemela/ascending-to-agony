using Godot;

public partial class Goal : Area2D {

    private void onFinishLineBodyEntered(Node body) {
        if (body.IsInGroup("Player")) {
            Global global = GetNode<Global>("/root/Global");
            global.enableWonMessage((Player)body);
        }
    }
}
