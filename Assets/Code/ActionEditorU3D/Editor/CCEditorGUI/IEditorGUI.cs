using UnityEngine;
using System.Collections;

namespace CCEditorGUI
{

    public enum EventGUIType
    {
        Hover,
        Press,
        Click,
        Drag,
    }


    public interface IEditorGUIListen
    {
        void OnClick(EditorGUIWidget node);
        void OnHover(EditorGUIWidget node);
        void OnPress(EditorGUIWidget node, bool isPress);
        void OnDrag(EditorGUIWidget node, Vector2 delta);
    }

}