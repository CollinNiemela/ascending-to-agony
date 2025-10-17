using Godot;
using System.Collections.Generic;

public partial class Terminal : Node {
    private CommandHolder commands;

    public override void _Ready() {
        //adds the commands to the command holder
        commands = new CommandHolder();
        commands.insert("setopacity", "Sets the terminals opacity to the given percent.", (setOpacityDel)setOpacity);
        commands.insert("clear", "Clears the past command output.", null);
        commands.insert("setmaxspeed", "Sets the players max speed to the given value.", (setMaxSpeedDel)setMaxSpeed);
        commands.insert("setmass", "Sets the players mass to the given value.", (setMassDel)setMass);
        commands.insert("setgravity", "Sets the players gravity scale to the given value.", (setGravityDel)setGravity);
        commands.insert("setfriction", "Sets the players friction to the given value.", (setFrictionDel)setFriction);
        commands.insert("setforce", "Sets the amount of force a bullet propels the player.", (setForceDel)setForce);
        commands.insert("setposition", "Sets the players global position to the given coordinates.", (setPositionDel)setPosition);
        commands.insert("setmaxbullets", "Sets the players maximum bullet count to the given number.", (setMaxBulletsDel)setMaxBullets);
        commands.insert("togglegui", "Toggles the users GUI.", (toggleGUIDel)toggleGUI);
        commands.insert("toggledebug", "Toggles the debug menu.", (toggleDebugDel)toggleDebug);
        commands.insert("toggleinv", "Toggles the players invincibility boolean.", (toggleInvincibilityDel)toggleInvincibility);
        commands.insert("resetplayer", "Resets the player to its original spawn position.", (resetPlayerDel)resetPlayer);
    }

    public string[] runCommand(string command) {
        //runs the command and returns the returned values
        KeyValuePair<string[], object> pair = commands.run(command);
        if (pair.Key.Length > 0) {
            return pair.Key;
        } else {
            string[] output = { (string)pair.Value };
            return output;
        }
    }

    //sets the players max speed to the given value if above 0
    private delegate string setMaxSpeedDel(int a);
    private string setMaxSpeed(int maxSpeed) {
        if (maxSpeed < 0) return "Players max speed cannot be less than 0";
        Player player = (Player)GetTree().Root.GetNode(Global.GAME_SCENE).GetNode("Player");
        player.setMaxSpeed(maxSpeed);
        return $"Set players max speed to {maxSpeed}";
    }

    //sets the players mass to the given value if above 0
    private delegate string setMassDel(float a);
    private string setMass(float mass) {
        if (mass < 0) return "Players mass cannot be less than 0";
        Player player = (Player)GetTree().Root.GetNode(Global.GAME_SCENE).GetNode("Player");
        player.setMass(mass);
        return $"Set players mass to {mass}";
    }

    //sets the players gravity to the given value if above 0
    private delegate string setGravityDel(float a);
    private string setGravity(float gravity) {
        if (gravity < 0) return "Players gravity cannot be less than 0";
        Player player = (Player)GetTree().Root.GetNode(Global.GAME_SCENE).GetNode("Player");
        player.setGravity(gravity);
        return $"Set players gravity to {gravity}";
    }

    //sets the players friction to the given value if above 0
    private delegate string setFrictionDel(float a);
    private string setFriction(float friction) {
        if (friction < 0) return "Players friction cannot be less than 0";
        Player player = (Player)GetTree().Root.GetNode(Global.GAME_SCENE).GetNode("Player");
        player.setDamp(friction);
        return $"Set players friction to {friction}";
    }

    //Toggles the player GUI menu
    private delegate string toggleGUIDel();
    private string toggleGUI() {
        Node2D bulletContainer = (Node2D)GetParent().GetParent().GetNode("GUI/BulletGUI");
        bulletContainer.Visible = !bulletContainer.Visible;
        return bulletContainer.Visible ? "Toggled GUI on" : "Toggled GUI off";
    }

    //Toggles the players debug menu
    private delegate string toggleDebugDel();
    private string toggleDebug() {
        Control debugControl = (Control)GetParent().GetParent().GetNode("GUI/DebugControl");
        debugControl.Visible = !debugControl.Visible;
        return debugControl.Visible ? "Toggled debug menu on" : "Toggled debug menu off";
    }

    //sets the players opacity to the given value if above 24
    private delegate string setOpacityDel(int a);
    private string setOpacity(int percent) {
        if (percent > 100) percent = 100;
        if (percent < 25) percent = 25;
        Control terminalUI = (Control)GetParent();
        Color a = terminalUI.Modulate;
        a.A = ((float)percent) / 100;
        terminalUI.Modulate = a;
        return $"Set the terminals opacity to {percent}%";
    }

    //sets the force a bullet propels the player if above 0
    private delegate string setForceDel(float a);
    private string setForce(float forceAmount) {
        if (forceAmount < 0) return "Players force amount cannot be less than 0";
        Player player = (Player)GetTree().Root.GetNode(Global.GAME_SCENE).GetNode("Player");
        player.setForceAmount(forceAmount);
        return $"Set players force amount to {forceAmount}";
    }

    //sets the players global position to the given position
    private delegate string setPositionDel(float a, float b);
    private string setPosition(float x, float y) {
        if (x > 100000 || x < -100000) return "X needs to be in between -100,000 and 100,000";
        if (y > 100000 || y < -100000) return "Y needs to be in between -100,000 and 100,000";
        Player player = (Player)GetTree().Root.GetNode(Global.GAME_SCENE).GetNode("Player");
        player.setPosition(x, y);
        return $"Set players global position to ({x}, {y})";
    }

    //sets the players max bullet count to the given amount
    private delegate string setMaxBulletsDel(int a);
    private string setMaxBullets(int max) {
        if (max < 0) return "Players maximum bullet count cannot be negative";
        Player player = (Player)GetTree().Root.GetNode(Global.GAME_SCENE).GetNode("Player");
        player.setMaxBulletCount(max);
        return $"Set player maximum bullet count to {max}";
    }

    //toggles player invincibility
    private delegate string toggleInvincibilityDel();
    private string toggleInvincibility() {
        Player player = (Player)GetTree().Root.GetNode(Global.GAME_SCENE).GetNode("Player");
        player.toggleInvincibility();
        return player.getInvincibilityToggle() ? "Toggled invincibility on" : "Toggled invincibility off";
    }

    //resets players position
    private delegate string resetPlayerDel();
    private string resetPlayer() {
        Global global = GetNode<Global>("/root/Global");
        global.resetGame();
        return "Reset player's position";
    }
}
