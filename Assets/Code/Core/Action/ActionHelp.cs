using System.Collections.Generic;
using CSTools;
using UnityEngine;
using System.Collections;



[SerializeField]
public class ActionHelp : Singleton<ActionHelp>
{
    private List<ActionGroupData> _GroupDatas;
    public List<ActionGroupData> GroupDatas
    {
        get { return _GroupDatas; }
        private set { _GroupDatas = value; }
    }


    public void Load()
    {
        _GroupDatas = new List<ActionGroupData>();
        Test_Action_0();
    }


    public ActionGroupData GetActionGroupData(int roleid, int groupnum)
    {
        if (_GroupDatas == null)
        {
            return null;
        }

        for (int i = 0; i < _GroupDatas.Count; i++)
        {
            if (_GroupDatas[i].RoleId == roleid && _GroupDatas[i].GroupNum == groupnum)
                return _GroupDatas[i];
        }

        return null;
    }


    static public int GetActionIndex(ActionGroupData groupData, string id)
    {
        if (groupData == null)
            return -1;

        for (int i = 0; i < groupData.ActionDatas.Count; i++)
        {
            if (groupData.ActionDatas[i].AnimId == id)
                return i;
        }

        return -1;
    }

    static public int GetActionTotalTime(ActionData data)
    {
        return data.AnimTime + data.PoseTime;
    }


    void Test_Action_0()
    {
        ActionGroupData data = new ActionGroupData
        {
            GroupNum = 0,
            RoleId = 1003,
            StartupAction = "N0000",
            ActionDatas = new List<ActionData>()
        };

        ActionInterrupt interrupt;

        //  站立
        ActionData action = null;
        action = new ActionData
        {
            AnimId = "N0000",
            AnimTime = 900,
            BlendTime = 100,
            CanMove = false,
            DefaultLinkActionId = "N0000",
            MoveSpeed = 0,
            Name = "Idle"
        };
        data.ActionDatas.Add(action);
        action.AnimSlotList.Add(new AnimSlotData() { Animation = "m_z_stand_f", Weight = 1, UseDir = false, Start = 0, End = 100});
        // Interrupt
        interrupt = new ActionInterrupt();
        interrupt.ActionID = "N0030";
        interrupt.ConditionType = ConditionType.Or;
        interrupt.ConnectTime = 100;
        interrupt.DetectionStartTime = 0;
        interrupt.DetectionEndTime = 200;
        interrupt.InterruptName = "Idle => 战斗跑步";
        interrupt.Conditions.Add(new InputCondition(GameInputState.KeyPressing, GameInputType.Move));
        action.InterruptList.Add(interrupt);


        interrupt = new ActionInterrupt()
        {
            ActionID = "W10010",
            ConditionType = ConditionType.Or,
            ConnectTime = 100,
            DetectionStartTime = 0,
            DetectionEndTime = 200,
            InterruptName = "Idle => 普攻1",
            ConnectMode = ActionInterruptConnectMode.Immediately
        };
        interrupt.Conditions.Add(new InputCondition(GameInputState.KeyDown, GameInputType.Attack));
        action.InterruptList.Add(interrupt);


        //  跑步
        action = new ActionData
        {
            AnimId = "N0030",
            AnimTime = 600,
            BlendTime = 100,
            CanMove = true,
            DefaultLinkActionId = "N0030",
            MoveSpeed = 500,
            Name = "战斗跑步"
        };
        data.ActionDatas.Add(action);
        action.AnimSlotList.Add(new AnimSlotData() { Animation = "m_z_runfront_01", Weight = 1, UseDir = false, Start = 0, End = 100 });

        interrupt = new ActionInterrupt();
        interrupt.ActionID = "N0000";
        interrupt.ConditionType = ConditionType.Or;
        interrupt.ConnectTime = 100;
        interrupt.DetectionStartTime = 0;
        interrupt.DetectionEndTime = 200;
        interrupt.InterruptName = "Idle => 战斗跑步";
        interrupt.Conditions.Add(new InputCondition(GameInputState.KeyRelease, GameInputType.Move));
        action.InterruptList.Add(interrupt);


        //  普攻1 
        action = new ActionData()
        {
            AnimId = "W10010",
            AnimTime = 200,
            BlendTime = 100,
            CanMove = false,
            DefaultLinkActionId = "W10015",
            MoveSpeed = 0,
            Name = "普攻1",
            PoseTime = 50,
        };
        data.ActionDatas.Add(action);
        action.AnimSlotList.Add(new AnimSlotData() { Animation = "m_z_p_01a", Weight = 1, UseDir = false, Start = 18, End = 100 });


        //  普攻1 收僵
        action = new ActionData()
        {
            AnimId = "W10015",
            AnimTime = 300,
            BlendTime = 100,
            CanMove = false,
            DefaultLinkActionId = "N0000",
            MoveSpeed = 0,
            Name = "普攻1[收僵]",
        };
        data.ActionDatas.Add(action);
        action.AnimSlotList.Add(new AnimSlotData() { Animation = "m_z_p_01b", Weight = 1, UseDir = false, Start = 0, End = 100 });

        _GroupDatas.Add(data);
    }


}
