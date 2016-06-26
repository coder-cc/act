using Core.Manager;
using UnityEngine;
using UnityEditor;
using System.Collections;





public class InputEditor : EditorWindow
{

    private bool isCanShow;


    [MenuItem("Action/Input")]
    static void OpenWnd()
    {
        GetWindow<InputEditor>();
    }


    private void Update()
    {
        if (Application.isPlaying && isCanShow == false)
        {
            isCanShow = true;
            //BindData();
        }

        if (Application.isPlaying == false && isCanShow)
        {
            isCanShow = false;
            //Clear();
        }

        if (isCanShow)
            Repaint();
    }


    void OnGUI()
    {
        if (isCanShow)
        {
            DrawState(InputManager.Instance.InputKeyStates);
        }
    }



    void DrawState(InputKeyState[] states)
    {
        for (int i = 0; i < states.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(string.Format("Type : {0} State : {1} PressedTime : {2:F2} ReleasedTime : {3:F2}", states[i].InputType, states[i].State, states[i].PressedTime, states[i].ReleasedTime));

            EditorGUILayout.EndHorizontal();
        }
    }





}
