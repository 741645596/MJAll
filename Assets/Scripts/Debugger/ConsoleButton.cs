// ConsoleButton.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/6/13
using UnityEngine;

public class ConsoleButton
{
    private readonly Rect IconDefaultPosition;

    private readonly DebugWindow debugger;

    public ConsoleButton(DebugWindow debugger)
    {
        this.debugger = debugger;
        IconDefaultPosition = new Rect(0, 0, 80, 35);
    }

    public void Draw()
    {
        GUI.DragWindow(IconDefaultPosition);

        if (GUILayout.Button("Console", GUILayout.Width(80), GUILayout.Height(70)))
        {
            debugger.OpenConsoleWindow();
        }
    }
}
