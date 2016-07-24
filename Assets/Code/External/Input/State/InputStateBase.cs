using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Aqua.InputEvent
{

    /// <summary>
    /// 按键状态
    /// </summary>

    public class InputStateBase
    {

        /// <summary>
        /// 当前状态(帧更新)
        /// </summary>
        public GameInputType InputType { get; protected set; }


        /// <summary>
        /// 当前按键按下与上一次按下的间隔时间
        /// </summary>
        public float DownRangeTime { get; protected set; }

        /// <summary>
        /// 按键按下累计的时间
        /// </summary>
        public float PressedTime { get; protected set; }

        /// <summary>
        /// 距离最后一次按键抬起的累计时间
        /// </summary>
        public float ReleasedTime { get; protected set; }


        public bool IsPress, IsDown, IsUp;


        /// <summary>
        /// 0 release 
        /// 1 click
        /// 2 double click
        /// </summary>
        public InputState State { get; protected set; }



        /// <summary>
        /// 侦测状态
        /// </summary>
        /// <param name="mngInputManager"></param>
        /// <param name="deltaTime"></param>

        public virtual void InterceptState(InputManager mngInputManager, float deltaTime)
        {
            
        }


        /// <summary>
        /// 侦测KeyCode状态
        /// </summary>
        /// <param name="code"></param>
        /// <param name="deltaTime"></param>

        protected void InterceptKeyCodeState(KeyCode code, float deltaTime)
        {

            IsDown = Input.GetKeyDown(code);
            IsPress = Input.GetKey(code);
            IsUp = Input.GetKeyUp(code);


            //if (IsDown )
            //{
            //    if (State)
            //    State = InputState.KeyPressing;
            //}

            //if(!IsIgnore)
            //{
            //    IsDown = Input.GetKeyDown(code);
            //    IsPress = Input.GetKey(code);
            //    IsUp = Input.GetKeyUp(code);
            //}

            //if (IsDown)
            //{
            //    if (State != InputState.KeyDoubleClick && !DownRangeTime.Equals(0f) && Time.realtimeSinceStartup - DownRangeTime <= 0.5f)
            //    {
            //        DownRangeTime = 0f;
            //        State = InputState.KeyDoubleClick;
            //        //Debug.Log("Double Click");
            //        return;
            //    }

            //    if (State != InputState.KeyDown)
            //    {
            //        State = InputState.KeyDown;
            //        PressedTime = 0f;
            //        DownRangeTime = Time.realtimeSinceStartup;
            //    }
            //    ReleasedTime = 0f;
            //}
            //else if (IsPress)
            //{
            //    State = InputState.KeyPressing;
            //    PressedTime += deltaTime;
            //}
            //else if (IsUp)
            //{
            //    if (State == InputState.KeyDown ||
            //        State == InputState.KeyPressing)
            //    {
            //        ReleasedTime = 0f;
            //        State = InputState.KeyRelease;
            //        //Debug.Log("Up");
            //    }
            //    PressedTime = 0f;
            //}
            //else
            //{
            //    State = InputState.None;
            //    ReleasedTime += deltaTime;
            //}

            //IsUp = false;
            //IsDown = false;
            //if (!IsPress)
            //    IsIgnore = false;
        }


        /// <summary>
        /// 外部触发状态改变
        /// </summary>
        /// <param name="press">当前的按键是否是按下状态</param>

        public virtual void OnKeyCodePress(bool press)
        {
            //if (press)
            //{
            //    IsUp = false;
            //    IsDown = true;
            //    IsPress = true;
            //    //IsIgnore = true;
            //}
            //else
            //{
            //    IsUp = true;
            //    IsPress = false;
            //}
        }


        /// <summary>
        /// 根据类型创建KeyState实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        
        public static InputStateBase CreateStateByType(GameInputType type, KeyCode code)
        {
            switch (type)
            {
                case GameInputType.Jump:
                case GameInputType.Attack:
                case GameInputType.SkillAttack:
                    return new InputKeyState(code) { InputType = type };
                case GameInputType.Move:
                    return new InputMoveState() { InputType = GameInputType.Move };
            }

            throw new System.Exception(string.Format("Can't Find Type [{0}]", type.ToString()));
        }

    }


}