using UnityEditor;
using UnityEngine;
using System.Collections;
using Aqua.Reflection;
using Core.Controller;

public class ActionWindowEditor : EditorWindow
{





    //private ActionListWindowEditor actionListWindow;


    [MenuItem("Action/Action Window")]
    public static void Init()
    {
        var window = GetWindow(typeof(ActionWindowEditor)) as ActionWindowEditor;
        //window.minSize = new Vector2(1100,640);
    }


    void Awake()
    {
        //if (EditorApplication.isPlaying)
        //{
            
        //}
    }


    void Update()
    {
        this.Repaint();
    }

    void OnGUI()
    {

        if (EditorApplication.isPlaying)
        {
            var unit = TestController.Instance.unit_1;
            if (unit != null)
            {
                var actionStatus = unit.GetProp("mActionStatus");
                EditorGUILayout.LabelField("Befor Key", actionStatus.GetProp("beforKey").ToString());
                EditorGUILayout.LabelField("Current Key", actionStatus.GetProp("_actionKey").ToString());
                var actionData = (ActionData)actionStatus.GetProp("_activeActionData");
                EditorGUILayout.LabelField("Action Name", actionData.Name);
                EditorGUILayout.LabelField("Action ID", actionData.AnimId);
            }
        }
    }




}
