using Godot;
using System.Collections.Generic;

public partial class TerminalUI : Control {
    // Exports
    [Export] private int maxHistory = 100;
    [Export] private LineEdit prompt;
    [Export] private VBoxContainer rows;
    [Export] private Node terminal;
    [Export] private ScrollContainer scrollContainer;
    [Export] private FontFile terminalFont;

    // Cached data
    private List<string> pastCommands;
    private int currentIndex;
    private bool searched;
    private int currentHistory;
    private ScrollBar scrollBar;
    private double maxScrollValue;

    public override void _Ready() {
        //sets terminal uis variables
        pastCommands = new List<string>();
        currentIndex = 0;
        searched = false;
        currentHistory = 1;

        // Get scrollbar from scrollContainer
        scrollBar = scrollContainer.GetVScrollBar();
        maxScrollValue = scrollBar.MaxValue;

        // Connect signals using modern C# delegates
        prompt.TextSubmitted += onEnter;
        prompt.TextChanged += onChange;
        scrollBar.Changed += onScrollChange;
    }

    public override void _Process(double delta) {
        //checks if up is pressed and sets the current command to a past command
        if (Input.IsActionJustPressed("terminal_up") && pastCommands.Count != 0) {
            if (!searched) {
                searched = true;
                prompt.Text = pastCommands[currentIndex];
            } else {
                if (currentIndex == pastCommands.Count - 1) {
                    prompt.CaretColumn = prompt.Text.Length;
                    return;
                }
                if (currentIndex < pastCommands.Count - 1) {
                    currentIndex++;
                }
                prompt.Text = pastCommands[currentIndex];
            }
            scrollContainer.ScrollVertical = (int)maxScrollValue;
            prompt.CaretColumn = prompt.Text.Length;
        }

        //checks if down is pressed and sets the current command to a past command
        if (Input.IsActionJustPressed("terminal_down") && pastCommands.Count != 0 && searched) {
            if (currentIndex == 0) return;
            if (currentIndex >= 1) {
                currentIndex--;
            }
            prompt.Text = pastCommands[currentIndex];
            scrollContainer.ScrollVertical = (int)maxScrollValue;
            prompt.CaretColumn = prompt.Text.Length;
        }
    }

    private void onEnter(string text) {
        //enters the text into the backend and prints out the return value
        if (!pastCommands.Contains(text) && text != "") {
            pastCommands.Insert(0, text);
        } else if (pastCommands.Contains(text)) {
            pastCommands.Remove(text);
            pastCommands.Insert(0, text);
        }
        currentIndex = 0;
        searched = false;
        if (text.ToLower() == "clear") {
            deleteChildrenFromGroup(rows, "OutputLine");
            prompt.Clear();
            return;
        }
        Label outputLine = new Label();
        outputLine.AddThemeFontOverride("font", terminalFont);
        outputLine.AddToGroup("OutputLine");
        string[] output = (string[])terminal.Call("runCommand", text);
        outputLine.Text += " > " + text;
        foreach (string o in output) {
            outputLine.Text += "\n" + o;
        }
        outputLine.Text += "\n";
        rows.AddChild(outputLine);
        rows.MoveChild(outputLine, outputLine.GetIndex() - 1);
        currentHistory++;
        while (currentHistory >= maxHistory) {
            rows.RemoveChild(rows.GetChild(0));
            currentHistory--;
        }
        prompt.Clear();
    }

    private void onChange(string text) {
        if (scrollContainer.ScrollVertical != (int)maxScrollValue) {
            scrollContainer.ScrollVertical = (int)maxScrollValue;
        }
    }

    private void onScrollChange() {
        if (maxScrollValue != scrollBar.MaxValue) {
            maxScrollValue = scrollBar.MaxValue;
            scrollContainer.ScrollVertical = (int)maxScrollValue;
        }
    }

    //deletes all children in parent that are in the given group
    private void deleteChildrenFromGroup(Node parent, string group) {
        foreach (Node child in parent.GetChildren()) {
            if (child.GetGroups().Contains(group)) {
                parent.RemoveChild(child);
            }
        }
    }

    //takes focus away from another control node
    public void grabFocus() {
        prompt.GrabFocus();
    }
}