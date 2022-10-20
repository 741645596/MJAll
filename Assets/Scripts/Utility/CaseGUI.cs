using System.Collections.Generic;
using ILRuntime.CLR.TypeSystem;
using Unity.Core;
using UnityEngine;

public class CaseGUI : MonoBehaviour
{
    public static CaseGUI Instance
    {
        get
        {
            if (_instance == null && !_isQuitting)
            {
                Application.quitting += OnQuitting;
                _instance = FindObjectOfType<CaseGUI>();
                if (_instance == null)
                {
                    _instance = new GameObject("CaseGUI").AddComponent<CaseGUI>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            return _instance;
        }
    }

    private static bool _isQuitting = false;
    private static CaseGUI _instance;

    public List<string> caseNames;
    public BaseCaseStage caseStage;
    public string stageName;

    private void OnGUI()
    {
        if (caseNames == null || caseStage == null || stageName == null)
        {
            return;
        }

        float width = 250 * display.srceenScaleFactor;
        float height = 55 * display.srceenScaleFactor;
        GUI.skin.button.fontSize = Mathf.FloorToInt(25 * display.srceenScaleFactor);

        GUILayout.BeginArea(new Rect(Screen.width - width, 0, width, Screen.height));
        caseNames.ForEach(n =>
        {
            string name = n.Substring(4);
            if (GUILayout.Button(name, GUILayout.Width(width), GUILayout.Height(height)))
            {
                IType itype = AppDomainManager.AppDomain.LoadedTypes[stageName];
                var type = itype.ReflectionType;
                type.GetMethod(n).Invoke(caseStage, null);
            }
        });
        GUILayout.EndArea();
    }

    private static void OnQuitting()
    {
        _isQuitting = true;
        _instance = null;
        Application.quitting -= OnQuitting;
    }
}
