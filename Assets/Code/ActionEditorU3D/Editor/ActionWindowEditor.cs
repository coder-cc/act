using UnityEditor;
using UnityEngine;
using System.Collections;

public class ActionWindowEditor : EditorWindow
{





    private ActionListWindowEditor actionListWindow;


    [MenuItem("Action/Action Window")]
    public static void Init()
    {
        var window = GetWindow(typeof(ActionWindowEditor)) as ActionWindowEditor;
        window.minSize = new Vector2(1100,640);
    }


    void Awake()
    {
        actionListWindow = new ActionListWindowEditor();
    }


    void OnGUI()
    {
        actionListWindow.Draw();
    }




}
