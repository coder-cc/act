using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Core.Manager
{

    /// <summary>
    /// 按键状态
    /// </summary>

    public class InputKeyState
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


        private bool IsPress, IsDown, IsUp, IsIgnore;

        /// <summary>
        /// 0 release 
        /// 1 click
        /// 2 double click
        /// </summary>
        public GameInputState State { get; protected set; }



        /// <summary>
        /// 侦测状态
        /// </summary>
        /// <param name="mngInputManager"></param>
        /// <param name="deltaTime"></param>

        virtual public void InterceptState(InputManager mngInputManager, float deltaTime) { }


        /// <summary>
        /// 侦测KeyCode状态
        /// </summary>
        /// <param name="code"></param>
        /// <param name="deltaTime"></param>

        protected void InterceptKeyCodeState(KeyCode code, float deltaTime)
        {
            if(!IsIgnore)
            {
                IsDown = Input.GetKeyDown(code);
                IsPress = Input.GetKey(code);
                IsUp = Input.GetKeyUp(code);
            }

            if (IsDown)
            {
                if (!DownRangeTime.Equals(0f) && Time.realtimeSinceStartup - DownRangeTime <= 0.5f)
                {
                    DownRangeTime = 0f;
                    State = GameInputState.KeyDoubleClick;
                    Debug.Log("Double Click");
                    return;
                }

                if (State != GameInputState.KeyDown)
                {
                    State = GameInputState.KeyDown;
                    PressedTime = 0f;
                    DownRangeTime = Time.realtimeSinceStartup;
                }
                ReleasedTime = 0f;
            }
            else if (IsPress)
            {
                State = GameInputState.KeyPressing;
                PressedTime += deltaTime;
            }
            else if (IsUp)
            {
                if (State == GameInputState.KeyDown ||
                    State == GameInputState.KeyPressing)
                {
                    ReleasedTime = 0f;
                    State = GameInputState.KeyRelease;
                    Debug.Log("Up");
                }
                PressedTime = 0f;
            }
            else
            {
                State = GameInputState.None;
                ReleasedTime += deltaTime;
            }

            IsUp = false;
            IsDown = false;
            if (!IsPress)
                IsIgnore = false;
        }
     

        /// <summary>
        /// 外部触发状态改变
        /// </summary>
        /// <param name="press">当前的按键是否是按下状态</param>

        virtual public void OnKeyCodePress(bool press)
        {
            if (press)
            {
                IsUp = false;
                IsDown = true;
                IsPress = true;
                IsIgnore = true;
            }
            else
            {
                IsUp = true;
                IsPress = false;
            }
        }


        /// <summary>
        /// 根据类型创建KeyState实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        
        public static InputKeyState CreateStateByType(GameInputType type, KeyCode code)
        {
            switch (type)
            {
                case GameInputType.Jump:
                case GameInputType.Attack:
                case GameInputType.SkillAttack:
                    return new InputKeyCommonCodeState(code) { InputType = type };
                case GameInputType.Move:
                    return new InputKeyMoveState() { InputType = GameInputType.Move };
            }

            throw new System.Exception(string.Format("Can't Find Type [{0}]", type.ToString()));
        }

    }


    /// <summary>
    /// 移动输入状态
    /// </summary>

    public class InputKeyMoveState : InputKeyState
    {
        public override void InterceptState(InputManager mngInputManager, float deltaTime)
        {
            if (mngInputManager.WhetherToMove())
            {
                if (State == GameInputState.KeyPressing)
                {
                    PressedTime += deltaTime;
                }
                else
                {
                    State = GameInputState.KeyPressing;
                    ReleasedTime = 0f;
                }
            }
            else
            {
                if (State == GameInputState.KeyPressing)
                {
                    State = GameInputState.KeyRelease;
                    PressedTime = 0f;
                }
                else
                {
                    ReleasedTime += deltaTime;
                }
            }
        }
    }


    /// <summary>
    /// 按键输入状态
    /// </summary>

    public class InputKeyCommonCodeState : InputKeyState
    {

        private readonly KeyCode mKeyCode;

        public KeyCode KeyCode
        {
            get { return mKeyCode; }
        }


        public InputKeyCommonCodeState(KeyCode keyCode)
        {
            mKeyCode = keyCode;
        }

        public override void InterceptState(InputManager mngInputManager, float deltaTime)
        {
            InterceptKeyCodeState(mKeyCode, deltaTime);
        }
    }

}