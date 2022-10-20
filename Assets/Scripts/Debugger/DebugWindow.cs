// Debugger.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/6/12

using UnityEngine;

public class DebugWindow : MonoBehaviour
{
    private readonly FPSCounter fps = new FPSCounter(0.5f);
    private ConsoleWindow consoleWindow;
    private ConsoleButton consoleButton;
    private bool isShowConsoleWindow;
    private Rect dragRect;
    private Rect windowRect;

    private void Start()
    {
        //if (Debug.isDebugBuild == false)
        //{
        //    return;
        //}
        
        dragRect = new Rect(Screen.width - 100, Screen.height - 100, 100, 100);
        windowRect = new Rect(0, 0, Screen.width, Screen.height);

        consoleWindow = new ConsoleWindow(this);
        consoleButton = new ConsoleButton(this);

        DontDestroyOnLoad(gameObject);

        Application.logMessageReceived += consoleWindow.OnLogMessageReceived;
    }


    public void OpenConsoleWindow()
    {
        isShowConsoleWindow = true;
    }

    public void CloseConsoleWindow()
    {
        isShowConsoleWindow = false;
    }

    public void OnDestory()
    {
        //if (Debug.isDebugBuild == false)
        //{
        //    return;
        //}

        Application.logMessageReceived -= consoleWindow.OnLogMessageReceived;
    }

    public void Update()
    {
        //if (Debug.isDebugBuild == false)
        //{
        //    return;
        //}

        fps.Update(Time.deltaTime, Time.unscaledDeltaTime);
    }


    public void OnGUI()
    {
        //if (Debug.isDebugBuild == false)
        //{
        //    return;
        //}

        if (isShowConsoleWindow)
        {
            windowRect = GUI.Window(0, windowRect, (id) =>
            {
                if (consoleWindow != null)
                {
                    WLDebug.LogLevel = LogLevel.Debug;
                    consoleWindow.Draw();
                }
            }, "Console");
        }
        else
        {
            dragRect = GUI.Window(0, dragRect, (id) =>
            {
                if(consoleButton != null)
                {
                    consoleButton.Draw();
                }
            }, "FPS:" + fps.CurrentFps);
        }
    }
}
