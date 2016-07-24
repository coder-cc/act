using UnityEngine;
using UnityEditor;
using Aqua.InputEvent;


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
            DrawState(InputManager.Instance.InputStatesBase);
        }
    }



    void DrawState(InputStateBase[] statesBase)
    {
        for (int i = 0; i < statesBase.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

            //GUILayout.Label(string.Format("Type : {0} State : {1} PressedTime : {2:F2} ReleasedTime : {3:F2}", statesBase[i].InputType, statesBase[i].State, statesBase[i].PressedTime, statesBase[i].ReleasedTime));
            GUILayout.Label(string.Format("IsPress : {0} IsDown : {1} IsUp {2}", statesBase[i].IsPress,
                statesBase[i].IsDown, statesBase[i].IsUp));

            EditorGUILayout.EndHorizontal();
        }
    }





}
