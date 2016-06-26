using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CCEditorGUI
{

    public class EditorGUIWindow 
    {
        private EditorGUIWidget pointEnter;
        private EditorGUIWidget pointPress;
        private EditorGUIWidget lastPointPress;
        private EditorGUIWidget pointDrag;

        private Event nowEvent;


        protected List<EditorGUIWidget> mAllWidgets = new List<EditorGUIWidget>();
        public List<EditorGUIWidget> AllWidget
        {
            get { return mAllWidgets; }
        }


        private List<IEditorGUIListen> mListens = new List<IEditorGUIListen>();


        public void AddListen(IEditorGUIListen listen)
        {
            if (!mListens.Contains(listen))
            {
                mListens.Add(listen);
            }
        }


        public void RemoveListen(IEditorGUIListen listen)
        {
            mListens.Remove(listen);
        }


        public void AddGUIWidget(EditorGUIWidget widget)
        {
            widget.GUISource = this;
            mAllWidgets.Add(widget);
        }


        public void RemoveGUIWidget(EditorGUIWidget widget)
        {
            widget.GUISource = null;
            mAllWidgets.Remove(widget);
        }


        protected virtual void Awake()
        {
            mAllWidgets = new List<EditorGUIWidget>();
        }


        public void OnGUI()
        {
            nowEvent = Event.current;

            if (mAllWidgets.Count == 0)
                return;

            TryGetMouseStayWidget(nowEvent.mousePosition, out pointEnter);

            if (nowEvent.type == EventType.Ignore)
            {
                pointPress = null;
                pointDrag = null;
            }


            if (nowEvent.isMouse)
            {
                if (nowEvent.type == EventType.MouseDown)
                {

                    //  只响应一个button,其余过滤.
                    if (pointPress != null)
                        return;

                    lastPointPress = null;
                    pointPress = pointEnter;

                    if (pointPress != null)
                        Notify(pointPress, EventGUIType.Press, true);
                }

                else if (nowEvent.type == EventType.MouseUp)
                {
                    if (pointPress != null)
                    {
                        if (pointPress == pointEnter && pointPress != pointDrag)
                        {
#if GUI_WINDOW_LOG
                            Debug.Log("Click!");
#endif
                            Notify(pointPress, EventGUIType.Click, true);
                        }
                        else
                        {
                            Notify(pointPress, EventGUIType.Press, false);
                        }
                        lastPointPress = pointPress;
                    }
                    pointPress = null;
                    pointDrag = null;
                }

                else if (nowEvent.type == EventType.MouseDrag)
                {
                    if (pointDrag == null)
                        pointDrag = pointPress;
#if GUI_WINDOW_LOG
                    Debug.Log("drag1");
#endif
                    if (pointDrag != null)
                    {
#if GUI_WINDOW_LOG
                        Debug.Log("drag2");
#endif
                        Notify(pointDrag, EventGUIType.Drag, null);
                    }
                }



            }

#if GUI_WINDOW_LOG
            string debugInfo = string.Format("pointEnter {0} | pointPress {1} \n lastPointPress {2} | pointDrag {3}",
                pointEnter != null ? pointEnter.ToString() : string.Empty,
               pointPress != null ? pointPress.ToString() : string.Empty,
               lastPointPress != null ? lastPointPress.ToString() : string.Empty,
               pointDrag != null ? pointDrag.ToString() : string.Empty);

            GUILayout.Label(debugInfo);
#endif

        }


        public int GetIndexByWidget(EditorGUIWidget w)
        {
            for (int i = 0; i < AllWidget.Count; i++)
            {
                if (AllWidget[i] == w)
                    return i;
            }
            return -1;
        }


        public void Notify(EditorGUIWidget widget, EventGUIType guiType, object data)
        {
            if (widget == null)
            {
                Debug.Log(" Widget == null guitype " + guiType.ToString());
            }


            if (widget.IsAutoDepth)
            {
                widget.SetAsFirstSibling();
            }

            switch (guiType)
            {
                case EventGUIType.Hover:
                    widget.OnHover();
                    for (int i = 0; i < mListens.Count; i++)
                    {
                        mListens[i].OnHover(widget);
                    }
                    Debug.Log("Hover");
                    break;
                case EventGUIType.Press:
                    widget.OnPress((bool)data);
                    for (int i = 0; i < mListens.Count; i++)
                    {
                        mListens[i].OnPress(widget, (bool)data);
                    }
                    Debug.Log("Press " + data.ToString());
                    break;
                case EventGUIType.Click:
                    widget.OnClick();

                    for (int i = 0; i < mListens.Count; i++)
                    {
                        mListens[i].OnClick(widget);
                    }
                    Debug.Log("Click ");
                    break;
                case EventGUIType.Drag:
                    widget.OnDrag(nowEvent.delta);
                    for (int i = 0; i < mListens.Count; i++)
                    {
                        mListens[i].OnDrag(widget, nowEvent.delta);
                    }
                    Debug.Log("Drag ");
                    break;
            }
        }


        private bool TryGetMouseStayWidget(Vector2 point, out EditorGUIWidget widget)
        {
            widget = null;

            for (int i = mAllWidgets.Count - 1; i >= 0; i--)
            {
                if (mAllWidgets[i].IsTrigger == false)
                    continue;

                if (mAllWidgets[i].AreaRect.Contains(point))
                {
                    widget = mAllWidgets[i];
                    return true;
                }
            }
            return false;
        }
    }

}