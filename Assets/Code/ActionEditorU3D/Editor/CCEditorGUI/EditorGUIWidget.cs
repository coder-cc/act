using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Networking.NetworkSystem;


namespace CCEditorGUI
{

    [Serializable]
    public abstract class EditorGUIWidget
    {

        protected string name;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            set { name = value; }
            get { return name; }
        }


        protected Rect areaRect;
        public Rect AreaRect
        {
            get { return areaRect; }
            set { areaRect = value; }
        }

        protected bool isTrigger;

        /// <summary>
        /// 是否响应事件
        /// </summary>
        public bool IsTrigger
        {
            get { return isTrigger; }
            set { isTrigger = value; }
        }


        protected bool isAutoDepth;

        /// <summary>
        /// 是否自动深度排序
        /// </summary>
        public bool IsAutoDepth
        {
            get { return isAutoDepth; }
            set { isAutoDepth = value; }
        }


        public float Height
        {
            get { return areaRect.height; }
        }


        public float Width
        {
            get { return areaRect.width; }
        }


        /// <summary>
        /// 所属Window
        /// </summary>
        public EditorGUIWindow GUISource { get; set; }


        /// <summary>
        /// 字节点列表
        /// </summary>

        protected List<EditorGUIWidget> childList = new List<EditorGUIWidget>();


        protected EditorGUIWidget parent;

        /// <summary>
        /// 设置父节点
        /// </summary>
        public EditorGUIWidget Parent
        {
            get { return parent; }
            set
            {
                SetParent(value);
            }
        }


        private Vector3 localPostion;
        public Vector3 LocalPostion { get { return localPostion; } private set { localPostion = value; } }

        private Vector3 worldPostion;
        public Vector3 WorldPostion { get { return worldPostion; } private set { worldPostion = value; } }


        virtual public void OnClick() { }
        virtual public void OnHover() { }
        virtual public void OnPress(bool isPress) { }
        virtual public void OnDrag(Vector2 delta) { }
        public abstract void OnDraw();


        public EditorGUIWidget()
        {
            isTrigger = true;
        }


        public void SetSize(float w, float h)
        {
            areaRect.width = w;
            areaRect.height = h;
        }

        public void SetPostion(float x, float y)
        {
            localPostion.x = x;
            localPostion.y = y;
            CalcPosition();
        }

        public void Move(float x, float y)
        {
            localPostion.x += x;
            localPostion.y += y;
            CalcPosition();
        }

        virtual new public string ToString()
        {
            return name;
        }


        //protected Vector2 Postion()
        //{
        //    return new Vector2(areaRect.x, areaRect.y);
        //}


        virtual protected void CalcPosition()
        {
            var tempParent = parent;
            worldPostion.x = localPostion.x;
            worldPostion.y = localPostion.y;
            while (tempParent != null)
            {
                worldPostion.x += tempParent.localPostion.x;
                worldPostion.y += tempParent.localPostion.y;
                tempParent = tempParent.Parent;
            }

            areaRect.x = worldPostion.x;
            areaRect.y = worldPostion.y;


            for (int i = 0; i < childList.Count; i++)
            {
                childList[i].CalcPosition();
            }

        }

        public void SetSiblingIndex(int index)
        {
            if (GUISource != null)
            {
                var all = GUISource.AllWidget;
                index = Mathf.Clamp(index, 0, all.Count - 1);
                all.Remove(this);
                all.Insert(index, this);
            }
        }


        public void SetAsFirstSibling()
        {
            if (GUISource != null)
                SetSiblingIndex(GUISource.AllWidget.Count - 1);
        }


        public void SetAsLastSibling()
        {
            SetSiblingIndex(0);
        }


        public int GetSiblingIndex()
        {
            return GUISource == null ? -1 : GUISource.GetIndexByWidget(this);
        }


        public void RemoveChild(EditorGUIWidget widget)
        {
            childList.Remove(widget);
        }

        public void AddChild(EditorGUIWidget widget)
        {
            if (childList.Contains(widget) == false)
                childList.Add(widget);
        }

        protected void SetParent(EditorGUIWidget widget)
        {
            if (parent == widget || this == widget)
                return;

            if (parent != null)
            {
                parent.RemoveChild(this);
            }

            if (widget != null)
            {
                widget.AddChild(this);
                parent = widget;
            }
            else
            {
                localPostion = worldPostion;
                return;
            }

            float x = 0, y = 0;
            var tempParent = parent;
            while (tempParent != null)
            {
                x += tempParent.localPostion.x;
                y += tempParent.localPostion.y;
                tempParent = tempParent.Parent;
            }

            localPostion.x = worldPostion.x - x;
            localPostion.y = worldPostion.y - y;
            RefreshArea();
        }

        protected void RefreshArea()
        {
            areaRect.x = worldPostion.x;
            areaRect.y = worldPostion.y;
        }


        protected float GetWholeHeight()
        {
            float result = 0f;

            for (int i = 0; i < childList.Count; i++)
            {
                result += childList[i].GetWholeHeight();
            }

            result += Height;
            return result;
        }


        protected float GetWholeWidth()
        {
            float result = 0;
            EditorGUIWidget temp = parent;
            while (temp != null)
            {
                result += 5f;
                temp = temp.parent;
            }

            return result;
        }
    }



}