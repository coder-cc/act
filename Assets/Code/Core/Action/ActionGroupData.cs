using System.Collections.Generic;
using UnityEngine;
using System.Collections;

//[ser]
//[ser]
[SerializeField]
public class ActionGroupData
{

    private int _GroupNum;
    public int GroupNum
    {
        get { return _GroupNum; }
        set { _GroupNum = value; }
    }


    private string _StartupAction = "";
    public string StartupAction
    {
        get { return _StartupAction; }
        set { _StartupAction = value; }
    }


    private int _RoleID;
    public int RoleId
    {
        get { return _RoleID; }
        set { _RoleID = value; }
    }


    private List<ActionData> _ActionDatas = new List<ActionData>();
    public List<ActionData> ActionDatas
    {
        get { return _ActionDatas; }
        set { _ActionDatas = value; }
    }

    
}


[SerializeField]
public class ActionData
{

    private string _AnimID;
    /// <summary>
    /// 动画ID
    /// </summary>
    public string AnimId
    {
        get { return _AnimID; }
        set { _AnimID = value; }
    }

    private int _AnimTime;
    /// <summary>
    /// 动画时长 (毫秒)
    /// </summary>
    public int AnimTime
    {
        get { return _AnimTime; }
        set { _AnimTime = value; }
    }


    private int _PoseTime;
    /// <summary>
    /// pose 持续时间
    /// </summary>
    public int PoseTime
    {
        get { return _PoseTime; }
        set { _PoseTime = value; }
    }

    private string _DefaultLinkActionID;
    /// <summary>
    /// 缺省动作连接
    /// </summary>
    public string DefaultLinkActionId
    {
        get { return _DefaultLinkActionID; }
        set { _DefaultLinkActionID = value; }
    }

    private int _MoveSpeed = 0;
    /// <summary>
    /// 移动速度
    /// </summary>
    public int MoveSpeed
    {
        get { return _MoveSpeed; }
        set { _MoveSpeed = value; }
    }


    private int _BlendTime = 0;
    /// <summary>
    /// 动画混合时间
    /// </summary>
    public int BlendTime
    {
        get { return _BlendTime; }
        set { _BlendTime = value; }
    }


    private string _Name = string.Empty;
    /// <summary>
    /// 动画名称
    /// </summary>
    public string Name
    {
        get { return _Name; }
        set { _Name = value; }
    }


    private bool _CanMove = false;
    /// <summary>
    /// 是否可以移动
    /// </summary>
    public bool CanMove
    {
        get { return _CanMove; }
        set { _CanMove = value; }
    }

   

    public List<AnimSlotData> AnimSlotList
    {
        get { return _AnimSlotList; }
    }
    private readonly List<AnimSlotData> _AnimSlotList = new List<AnimSlotData>(); 


    public List<ActionInterrupt> InterruptList
    {
        get { return _InterruptList; }
    }
    private readonly List<ActionInterrupt>  _InterruptList = new List<ActionInterrupt>();

  
}


public class AnimSlotData
{
    private string _Animation;
    /// <summary>
    /// 动画名称
    /// </summary>
    public string Animation
    {
        get { return _Animation; }
        set { _Animation = value; }
    }

  
    private int _Start;
    /// <summary>
    /// 动画开始时间(百分比)
    /// </summary>
    public int Start
    {
        get { return _Start; }
        set { _Start = value; }
    }


    private int _End;
    /// <summary>
    /// 动画结束时间(百分比)
    /// </summary>
    public int End
    {
        get { return _End; }
        set { _End = value; }
    }

 
    private int _Weight = default(int);
    /// <summary>
    /// 权重
    /// </summary>
    public int Weight
    {
        get { return _Weight; }
        set { _Weight = value; }
    }

   
    private int _Dir = default (int);
    /// <summary>
    /// 方向
    /// </summary>
    public int Dir
    {
        get { return _Dir; }
        set { _Dir = value; }
    }

 
    private bool _UseDir = default(bool);
    /// <summary>
    /// 是否使用方向
    /// </summary>
    public bool UseDir
    {
        get { return _UseDir; }
        set { _UseDir = value; }
    }


    
}