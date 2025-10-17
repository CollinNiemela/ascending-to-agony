using Godot;

public partial class OnScreenCheck : VisibleOnScreenNotifier2D {

    public override void _Ready() {
        Global.setTreePause(GetParent(), false);
    }

    private void onScreenEntered() {
        Global.setTreePause(GetParent(), true);
    }

    private void onScreenExited() {
        Global.setTreePause(GetParent(), false);
    }
}
