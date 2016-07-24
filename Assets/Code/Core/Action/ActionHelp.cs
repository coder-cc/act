using System.Collections.Generic;
using Aqua.Action.Event;
using CSTools;
using UnityEngine;
using Aqua.InputEvent;


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
        interrupt = new ActionInterrupt
        {
            ActionID = "N0030",
            ConditionType = ConditionType.Or,
            ConnectTime = 100,
            DetectionStartTime = 0,
            DetectionEndTime = 200,
            InterruptName = "Idle => 战斗跑步"
        };
        interrupt.Conditions.Add(new InputCondition(InputState.KeyPressing, GameInputType.Move));
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
        interrupt.Conditions.Add(new InputCondition(InputState.KeyDown, GameInputType.Attack));
        interrupt.Conditions.Add(new InputCondition(InputState.KeyRelease, GameInputType.Attack));
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

        interrupt = new ActionInterrupt
        {
            ActionID = "N0000",
            ConditionType = ConditionType.Or,
            ConnectTime = 100,
            DetectionStartTime = 0,
            DetectionEndTime = 200,
            InterruptName = "战斗跑步 => Idle"
        };
        interrupt.Conditions.Add(new InputCondition(InputState.KeyRelease, GameInputType.Move));
        action.InterruptList.Add(interrupt);


        interrupt = new ActionInterrupt()
        {
            ActionID = "W10010",
            ConditionType = ConditionType.Or,
            ConnectTime = 100,
            DetectionStartTime = 0,
            DetectionEndTime = 200,
            InterruptName = "战斗跑步 => 普攻1",
            ConnectMode = ActionInterruptConnectMode.Immediately
        };
        interrupt.Conditions.Add(new InputCondition(InputState.KeyDown, GameInputType.Attack));
        interrupt.Conditions.Add(new InputCondition(InputState.KeyRelease, GameInputType.Attack));
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
        action.EventArgses.Add(new EffectEventArgs()
        {
            ResourcePath = "Assets/LocalResources/Actor/Effect/fx_my_p_01.prefab",
            BindingType = EffectBindingType.BindingOnwer,
            Time = 20,
            LifeTime = 500,
            PositionY = 110,
            //LocalOffset
        });


        action.EventArgses.Add(new VelocityEventArgs()
        {
            Time = 20,
            VelocityZ = 550,
        });

        action.EventArgses.Add(new VelocityEventArgs()
        {
            Time = 70,
            VelocityZ = 0,
        });


        interrupt = new ActionInterrupt();
        interrupt.ActionID = "W10020";
        interrupt.ConditionType = ConditionType.Or;
        interrupt.ConnectMode = ActionInterruptConnectMode.WaitFinish;
        interrupt.DetectionStartTime = 60;
        interrupt.DetectionEndTime = 200;
        interrupt.ConnectTime = 100;
        interrupt.InterruptName = "普攻1 => 普攻2";
        //interrupt.Conditions.Add(new InputCondition(InputState.KeyRelease, GameInputType.Attack));
        interrupt.Conditions.Add(new InputCondition(InputState.KeyDown, GameInputType.Attack));
        action.InterruptList.Add(interrupt);


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


        //  普攻2

        action = new ActionData()
        {
            AnimId = "W10020",
            AnimTime = 200,
            BlendTime = 100,
            CanMove = false,
            DefaultLinkActionId = "W10025",
            MoveSpeed = 0,
            Name = "普攻2",
            PoseTime = 100,
        };
        data.ActionDatas.Add(action);
        action.AnimSlotList.Add(new AnimSlotData() { Animation = "m_z_p_02a", Weight = 1, UseDir = false, Start = 0, End = 100 });

        interrupt = new ActionInterrupt();
        interrupt.ActionID = "W10030";
        interrupt.ConditionType = ConditionType.Or;
        interrupt.ConnectMode = ActionInterruptConnectMode.WaitFinish;
        interrupt.DetectionStartTime = 60;
        interrupt.DetectionEndTime = 200;
        interrupt.ConnectTime = 100;
        interrupt.InterruptName = "普攻2 => 普攻3";
        //interrupt.Conditions.Add(new InputCondition(InputState.KeyRelease, GameInputType.Attack));
        interrupt.Conditions.Add(new InputCondition(InputState.KeyDown, GameInputType.Attack));
        action.InterruptList.Add(interrupt);

        action.EventArgses.Add(new EffectEventArgs()
        {
            ResourcePath = "Assets/LocalResources/Actor/Effect/fx_my_p_02.prefab",
            BindingType = EffectBindingType.BindingOnwer,
            Time = 50,
            LifeTime = 500,
            PositionY = 120,
            //LocalOffset
        });



        //  普攻2 收僵
        action = new ActionData()
        {
            AnimId = "W10025",
            AnimTime = 300,
            BlendTime = 100,
            CanMove = false,
            DefaultLinkActionId = "N0000",
            MoveSpeed = 0,
            Name = "普攻2[收僵]",
        };
        data.ActionDatas.Add(action);
        action.AnimSlotList.Add(new AnimSlotData() { Animation = "m_z_p_02b", Weight = 1, UseDir = false, Start = 0, End = 100 });


        //  普攻3

        action = new ActionData()
        {
            AnimId = "W10030",
            AnimTime = 600,
            BlendTime = 0,
            CanMove = false,
            DefaultLinkActionId = "W10035",
            MoveSpeed = 0,
            Name = "普攻3",
            PoseTime = 0,
        };
        data.ActionDatas.Add(action);
        action.AnimSlotList.Add(new AnimSlotData() { Animation = "m_z_p_03a", Weight = 1, UseDir = false, Start = 0, End = 100 });

        interrupt = new ActionInterrupt();
        interrupt.ActionID = "W10040";
        interrupt.ConditionType = ConditionType.Or;
        interrupt.ConnectMode = ActionInterruptConnectMode.WaitFinish;
        interrupt.DetectionStartTime = 60;
        interrupt.DetectionEndTime = 200;
        interrupt.ConnectTime = 100;
        interrupt.InterruptName = "普攻3 => 普攻4";
        //interrupt.Conditions.Add(new InputCondition(InputState.KeyRelease, GameInputType.Attack));
        interrupt.Conditions.Add(new InputCondition(InputState.KeyDown, GameInputType.Attack));
        action.InterruptList.Add(interrupt);

        action.EventArgses.Add(new EffectEventArgs()
        {
            ResourcePath = "Assets/LocalResources/Actor/Effect/fx_my_p_03.prefab",
            BindingType = EffectBindingType.BindingOnwer,
            Time = 30,
            LifeTime = 500,
            PositionY = 150,
            //LocalOffset
        });

        //  普攻3 收僵

        action = new ActionData()
        {
            AnimId = "W10035",
            AnimTime = 600,
            BlendTime = 0,
            CanMove = false,
            DefaultLinkActionId = "N0000",
            MoveSpeed = 0,
            Name = "普攻3 收僵",
            PoseTime = 0,
        };
        data.ActionDatas.Add(action);
        action.AnimSlotList.Add(new AnimSlotData() { Animation = "m_z_p_03b", Weight = 1, UseDir = false, Start = 0, End = 100 });



        //  普攻4 - 01

        action = new ActionData()
        {
            AnimId = "W10040",
            AnimTime = 500,
            BlendTime = 100,
            CanMove = false,
            DefaultLinkActionId = "W10043",
            MoveSpeed = 0,
            Name = "普攻4 - 01",
            PoseTime = 0,
        };
        data.ActionDatas.Add(action);
        action.AnimSlotList.Add(new AnimSlotData() { Animation = "m_z_p_04", Weight = 1, UseDir = false, Start = 0, End = 29 });

        action.EventArgses.Add(new EffectEventArgs()
        {
            ResourcePath = "Assets/LocalResources/Actor/Effect/fx_my_p_04.prefab",
            BindingType = EffectBindingType.BindingOnwer,
            Time = 20,
            LifeTime = 500,
            PositionY = 120,
            //LocalOffset
        });

        action.EventArgses.Add(new EffectEventArgs()
        {
            ResourcePath = "Assets/LocalResources/Actor/Effect/fx_my_p_04.prefab",
            BindingType = EffectBindingType.BindingOnwer,
            Time = 60,
            LifeTime = 500,
            PositionY = 140,
            //LocalOffset
        });


        //  普攻4 - 02

        action = new ActionData()
        {
            AnimId = "W10043",
            AnimTime = 200,
            BlendTime = 100,
            CanMove = false,
            DefaultLinkActionId = "W10046",
            MoveSpeed = 0,
            Name = "普攻4 - 02",
            PoseTime = 100,
        };
        data.ActionDatas.Add(action);
        action.AnimSlotList.Add(new AnimSlotData() { Animation = "m_z_p_04", Weight = 1, UseDir = false, Start = 29, End = 66 });


        action.EventArgses.Add(new EffectEventArgs()
        {
            ResourcePath = "Assets/LocalResources/Actor/Effect/fx_my_p_04b.prefab",
            BindingType = EffectBindingType.BindingOnwer,
            Time = 20,
            LifeTime = 500,
            PositionY = 100,
            //LocalOffset
        });



        //  普攻4 - 收僵

        action = new ActionData()
        {
            AnimId = "W10046",
            AnimTime = 400,
            BlendTime = 100,
            CanMove = false,
            DefaultLinkActionId = "N0000",
            MoveSpeed = 0,
            Name = "普攻4 - 02",
            PoseTime = 0,
        };
        data.ActionDatas.Add(action);
        action.AnimSlotList.Add(new AnimSlotData() { Animation = "m_z_p_04", Weight = 1, UseDir = false, Start = 66, End = 100 });



        _GroupDatas.Add(data);
    }


}
