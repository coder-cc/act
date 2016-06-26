using System;
using CCEditorGUI;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;


public class ListNodal : EditorGUIWidget
{


    private bool isfold;
    private bool isChangeName;
    private ActionListWindowEditor ownerWindow;

    private float flodWidth;

    public ListNodal(ActionListWindowEditor owner)
    {
        GUISource = owner;
        ownerWindow = owner;
        SetSize(280, 20);
    }
    

    public ActionListWindowEditor OwnerWindow
    {
        get
        {
            if (ownerWindow == null)
                ownerWindow = GUISource as ActionListWindowEditor;
            return ownerWindow;
        }
    }


    public override void OnDraw()
    {
        GUILayout.BeginArea(areaRect);
        bool fold = DrawHeader( isfold, GetSelectNodalLevel() == this);
        if (fold != isfold)
        {
            isfold = fold;
            ownerWindow.RefreshPosition();
        }
        GUILayout.EndArea();
        if (isfold)
            for (int i = 0; i < childList.Count; i++)
            {
                childList[i].OnDraw();
            }
        
    }


    public override void OnClick()
    {
    }



    public float SetOrderPosition(float w, float h)
    {
        SetPostion(w, h);

        if (isfold)
            for (int i = 0; i < childList.Count; i++)
            {
                h = ((ListNodal)childList[i]).SetOrderPosition(w + 15, h + Height);
            }

        return h;
    }


    public float GetLastChildPositionY()
    {
        if (childList.Count == 0)
        {
            return LocalPostion.y;
        }
        return ((ListNodal)childList[childList.Count - 1]).GetLastChildPositionY();
    }


    protected override void CalcPosition()
    {
        areaRect.x = LocalPostion.x;
        areaRect.y = LocalPostion.y;
    }

    public bool DrawHeader( bool fold, bool isSelect)
    {
        GUILayout.BeginHorizontal();
        GUI.changed = false;
        GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);




        string text = fold ? "\u25BC" + (char)0x200a : "\u25BA" + (char)0x200a;


        if (childList.Count == 0)
        {
            text = string.Empty;
        }

        if (!GUILayout.Toggle(true, text, "PreToolbar2",
            GUILayout.MaxWidth(10f)))
        {
            fold = !fold;
            isChangeName = false;
        }

        if (isChangeName)
        {
            name = EditorGUILayout.TextField(name, GUILayout.MinWidth(200));
        }
        else
        {
            if (GUILayout.Button(name, EditorStyles.label, GUILayout.MinWidth(200)) )
            {
                if (Event.current.button == 0)
                {
                    if (GetSelectNodalLevel() == this)
                        isChangeName = true;
                    SetSelectNodalLevel(this);
                }
                else if (Event.current.button == 1)
                {
                    ActionContextMenu.AddItem("Add Child", false, OnMenuCallback, null );
                    ActionContextMenu.Show();
                }
            }
        }

        GUI.contentColor = Color.white;
        GUILayout.EndHorizontal();

        GUI.backgroundColor = Color.white;
        return fold;
    }

    private void OnMenuCallback(object userData)
    {
        isfold = true;
        ownerWindow.CreateNodal(name + "(Child)", this);
        ActionContextMenu.Clear();
    }


    public void OnFocus(bool isFocus)
    {
        if (isFocus == false)
            isChangeName = false;
    }


    public ListNodal GetSelectNodalLevel()
    {
        return OwnerWindow.GetSelectNodalLevel();
    }


    private void SetSelectNodalLevel(ListNodal nodal)
    {
        OwnerWindow.SetSelectNodalLevel(nodal);
    }

}



public class ActionListWindowEditor : EditorGUIWindow
{


    private Rect mArea;
    private Rect mScrollArea;
    private ListNodal selectNodalLevel;
    private ListNodal rootNodal;

    private float StartHeight = 0f;
    private Vector2 scrollVector2 = Vector2.zero;


    private Vector2 minSize;


    private ManualScrollbar verticalScrollbar;
    private ManualScrollbar horizontalScrollbar;

    public ActionListWindowEditor()
    {
        minSize = new Vector2(305, 640);
        mArea = new Rect(0, 0, 305, 25);
        mScrollArea = new Rect(0,25,300,615);

        verticalScrollbar = new ManualScrollbar(mScrollArea, ManualScrollbar.Director.Right);
        verticalScrollbar.SetRect(new Rect(mScrollArea.x + mScrollArea.width - 15, 20, 20, mScrollArea.height + 5));
        horizontalScrollbar = new ManualScrollbar(mScrollArea, ManualScrollbar.Director.Bottom);
        horizontalScrollbar.SetRect(new Rect(0, (mScrollArea.y + mScrollArea.height) -15 , mScrollArea.width - 15, 20));

        for (int i = 0; i < 50; i++)
        {
            CreateNodal(i.ToString(), null);
        }
    }



    public void CreateNodal( string name , ListNodal parent)
    {
        ListNodal nodal = new ListNodal(this) {Name = name, Parent = parent};
        if (parent == null)
            mAllWidgets.Add(nodal);
        else
        {
            parent.AddChild(nodal);
        }
        RefreshPosition();
    }


    public void RefreshPosition()
    {
        if (mAllWidgets.Count == 0)
            return;

        float height = -mAllWidgets[0].Height;
        for (int i = 0; i < mAllWidgets.Count; i++)
        {
            height = ((ListNodal)mAllWidgets[i]).SetOrderPosition(0, height + mAllWidgets[0].Height);
        }


        mScrollArea.height = Mathf.Clamp(height, 615, int.MaxValue);
        verticalScrollbar.UpdateOwnerRect(mScrollArea.height + 20);

        mScrollArea.width = Mathf.Clamp(300, 300, int.MaxValue);
        horizontalScrollbar.UpdateOwnerRect(mScrollArea.width);

        //height = ((ListNodal) mAllWidgets[mAllWidgets.Count - 1]).GetLastChildPositionY();
        //mArea.height = Mathf.Clamp(height + 50, minSize.y, height + 50);
        //Debug.Log();
    }


    public ListNodal GetSelectNodalLevel()
    {
        return selectNodalLevel;
    }


    public void SetSelectNodalLevel(ListNodal nodal)
    {
        if (selectNodalLevel == nodal)
            return;

        if (selectNodalLevel != null)
        {
            selectNodalLevel.OnFocus(false);
        }

        selectNodalLevel = nodal;
    }


    public void Draw()
    {
        base.OnGUI();

       
        
        //GUILayout.BeginArea(mScrollArea);
        //GUILayout.BeginScrollView( new Bound())
        //EditorGUILayout.BeginScrollView()
        //scrollVector2 = EditorGUILayout.BeginScrollView(scrollVector2, true, true);
        //GUILayout.VerticalScrollbar(  )
        //DrawVerticalScrollbar();


        mScrollArea.y = verticalScrollbar.Value;
        mScrollArea.x = horizontalScrollbar.Value;
      
        GUILayout.BeginArea(mScrollArea);
        for (int i = 0; i < mAllWidgets.Count; i++)
        {
            mAllWidgets[i].OnDraw();
        }
        GUILayout.EndArea();

        verticalScrollbar.Draw();
        horizontalScrollbar.Draw();


        GUILayout.BeginArea(mArea);
        DrawTools();
        GUILayout.EndArea();



        //EditorGUILayout.EndScrollView();
        //GUILayout.EndArea();

        //GUILayout.EndArea();


        //EditorGUILayout.BeginVertical();

        
        //EditorGUILayout.EndVertical();
    }


    private static int testID;
    private void DrawTools()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("+", GUILayout.MinWidth(70f)))
        {
            CreateNodal("Node " + (++testID), null);
        }

        if (GUILayout.Button("Child", GUILayout.MinWidth(70f)))
        {
            if (mAllWidgets.Count == 0)
            {
                CreateNodal("Node " + (++testID), null);
            }
            else
            {
                CreateNodal("Node " + (++testID), (ListNodal)mAllWidgets[Random.Range(0, mAllWidgets.Count - 1)]);
            }
        }


        EditorGUILayout.EndHorizontal();
    }



    //float 
    private void DrawVerticalScrollbar()
    {
        GUI.VerticalScrollbar(new Rect(300, 25, 20, 600), 0, 100, 0, 100);
    }

}



public class ManualScrollbar
{

    public Rect mViewArea;
    public Rect mRect;
    public Director mType;

    private float mValue;
    private float mSize;

    private float mMax;
    private bool isVertical;




    public enum Director
    {
        Top,
        Bottom,
        Left,
        Right,
    }


    public ManualScrollbar( Rect view,  Director type)
    {
        mType = type;
        mViewArea = view;
        isVertical = (type == Director.Left || type == Director.Right);
        CalcPostion(type);
    }





    public float Value
    {
        get
        {
            //return mValue; 
            if (isVertical)
            {
                return (-(mValue * mMax)) + mViewArea.y;
            }
            else
            {
                return (-(mValue * mMax)) + mViewArea.x;
            }
            
        }
        set { mValue = value; }
    }

    public float Size
    {
        get { return mSize; }
        set { mSize = value; }
    }



    public void SetRect(Rect rt)
    {
        mRect = rt;
    }

    public void SetWidth( float w)
    {
        mRect.width = w;
    }

    public void SetHeight(float h)
    {
        mRect.height = h;
    }

    public void UpdateOwnerWidth(float w)
    {
        mViewArea.width = w;
    }


    public void UpdateOwnerHeright(float h)
    {
        mViewArea.height = h;
    }


    public void UpdateOwnerRect( float currentSize)
    {
        mMax = currentSize;
        if (isVertical)
        {
            if (currentSize <= mViewArea.height)
                mSize = 1;
            else
            {
                mSize = mViewArea.height / currentSize;
            }
        }
        else
        {
            if (currentSize <= mViewArea.width)
                mSize = 1;
            else
            {
                mSize = mViewArea.width / currentSize;
            }
        }
    }


    public void CalcPostion(Director t)
    {
        if (t == Director.Left)
        {
            mRect.width = 20;
            mRect.x = 0;
            mRect.y = 0;
            mRect.height = mViewArea.height;
        }
        else if (t == Director.Right)
        {
            mRect.width = 20;
            mRect.x = (mViewArea.x + mViewArea.width) - 20;
            mRect.y = 20;
            mRect.height = mViewArea.height - 20;
        }
        else if (t == Director.Bottom)
        {
            mRect.width = mViewArea.width;
            mRect.x = 0f;
            mRect.y = (mViewArea.y + mViewArea.height) - 40;
            mRect.height = 20;
        }
        else if (t == Director.Top)
        {
            mRect.width = mViewArea.width;
            mRect.x = 0f;
            mRect.y = 0f;
            mRect.height = 20;
        }
    }

    public void Draw()
    {
        if (mType == Director.Bottom || mType == Director.Top)
            mValue = GUI.HorizontalScrollbar(mRect, mValue, mSize, 0, 1);
        else
        {
            mValue = GUI.VerticalScrollbar(mRect, mValue, mSize, 0, 1);
        }
    }


}


//public class ActionListWindowEditor : IFoldNodal
//{

//    private FoldNodalLevel selectNodalLevel;
//    private BetterList<FoldNodalLevel> nodalBetterList;

//    private Bound mArea;

//    public ActionListWindowEditor()
//    {
//        mArea = new Bound(0, 0, 305, 640);
//        nodalBetterList = new BetterList<FoldNodalLevel>();
//    }


//    public BetterList<FoldNodalLevel> NodalBetterList
//    {
//        get { return nodalBetterList; }
//    }


//    public void Draw()
//    {
//        GUILayout.BeginArea(mArea);
//        DrawTools();
//        DrawTitle();
//        GUILayout.EndArea();
//    }


//    private void DrawTitle()
//    {
//        for (int i = 0; i < nodalBetterList.size; i++)
//        {
//            nodalBetterList[i].Draw();
//        }
//    }


//    public void AddNodal(string name, object data = null)
//    {
//        FoldNodalLevel nodal = new FoldNodalLevel(this, name, true, data);
//        nodalBetterList.Add(nodal);
//    }


//    private void DrawTools()
//    {
//        EditorGUILayout.BeginHorizontal();

//        if (GUILayout.Button("+", GUILayout.MinWidth(70f)))
//        {
//            AddNodal("Fold", null);
//        }

//        EditorGUILayout.EndHorizontal();
//    }


//    public FoldNodalLevel GetSelectNodalLevel()
//    {
//        return selectNodalLevel;
//    }

//    public void SetSelectNodalLevel(FoldNodalLevel nodal)
//    {
//        selectNodalLevel = nodal;
//    }


//    public void AddRootNodalLevel(FoldNodalLevel nodal)
//    {
//        nodalBetterList.Add(nodal);   
//    }


//    public void RemoveRootNodalLevel(FoldNodalLevel nodal)
//    {
//        nodalBetterList.Remove(nodal);
//    }
//}




public interface IFoldNodal
{

    FoldNodalLevel GetSelectNodalLevel();

    void SetSelectNodalLevel(FoldNodalLevel nodal);


    void AddRootNodalLevel(FoldNodalLevel nodal);


    void RemoveRootNodalLevel(FoldNodalLevel nodal);
}



public class FoldNodalLevel 
{

    private bool isFold;
    private bool isChangeName;

    private string name;
    private BetterList<FoldNodalLevel> childBetterList;
    private FoldNodalLevel parentNodalLevel;
    private object data;


    private IFoldNodal ownerFoldNodal;


    public bool IsFold
    {
        get { return isFold; }
        set { isFold = value; }
    }


    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public int Count
    {
        get { return childBetterList.size; }
    }


    public BetterList<FoldNodalLevel> ChildBetterList
    {
        get { return childBetterList; }
        set { childBetterList = value; }
    }

    public object Data
    {
        get { return data; }
        set { data = value; }
    }


    public FoldNodalLevel(IFoldNodal o, string name, bool defaultFold, object data = null)
    {
        this.ownerFoldNodal = o;
        this.name = name;
        this.isFold = defaultFold;
        childBetterList = new BetterList<FoldNodalLevel>();
    }


    public void Add(FoldNodalLevel nodal)
    {
        childBetterList.Add(nodal);
    }


    public void Remove(FoldNodalLevel nodal)
    {
        for (int i = 0; i < childBetterList.size; i++)
        {
            if (childBetterList[i] == nodal)
            {
                childBetterList.RemoveAt(i);
                break;
            }
        }
    }


    public bool Contains(FoldNodalLevel nodal)
    {
        return FindIndex(nodal) != -1;
    }


    private int FindIndex(FoldNodalLevel nodal)
    {
        for (int i = 0; i < childBetterList.size; i++)
        {
            if (childBetterList[i] == nodal)
            {
                return i;
            }
        }

        return -1;
    }


    private void SetParent(FoldNodalLevel parent)
    {
        //if (parent != null)
        //{
        //    parent.
        //}

        parentNodalLevel = parent;
        //if ()
    }


    public void Draw()
    {
        isFold = DrawHeader(name, isFold, ownerFoldNodal.GetSelectNodalLevel() == this);
    }



    public bool DrawHeader(string text, bool isfold, bool isSelect)
    {
        GUILayout.BeginHorizontal();
        GUI.changed = false;
        GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
       
        if (!GUILayout.Toggle(true, isfold ? "\u25BC" + (char) 0x200a : "\u25BA" + (char) 0x200a, "PreToolbar2",
                GUILayout.MaxWidth(10f)))
        {
            isfold = !isfold;
            isChangeName = false;
        }
       
        if (isChangeName)
        {
            name = EditorGUILayout.TextField(name, GUILayout.MinWidth(280));
        }
        else
        {
            if (GUILayout.Button(text, EditorStyles.label, GUILayout.MinWidth(280)))
            {
                if (ownerFoldNodal.GetSelectNodalLevel() == this)
                    isChangeName = true;
                ownerFoldNodal.SetSelectNodalLevel(this);
            }
        }

        GUI.contentColor = Color.white;
        GUILayout.EndHorizontal();

        GUI.backgroundColor = Color.white;
        return isfold;
    }


    //public void Add(FoldNodalLevel item)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void Clear()
    //{
    //    throw new System.NotImplementedException();
    //}

    //public bool Contains(FoldNodalLevel item)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void CopyTo(FoldNodalLevel[] array, int arrayIndex)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public int Count
    //{
    //    get { throw new System.NotImplementedException(); }
    //}

    //public bool IsReadOnly
    //{
    //    //get { throw new System.NotImplementedException(); }
    //}

    //public bool Remove(FoldNodalLevel item)
    //{
    //    //throw new System.NotImplementedException();
    //}

    //public IEnumerator GetEnumerator()
    //{
    //    //throw new System.NotImplementedException();
    //}

    //IEnumerator<FoldNodalLevel> IEnumerable<FoldNodalLevel>.GetEnumerator()
    //{
    //    //throw new System.NotImplementedException();
    //}
}