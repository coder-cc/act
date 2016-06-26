using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using CSTools;
using UnityEngine;

namespace Core.Manager
{

    public class InputManager : Singleton<InputManager>
    {

        private Vector2 mMoveVector;
        private Vector2 mAxleVector;
        private InputKeyState[] mInputKeyStates;



        /// <summary>
        /// 返回虚拟轴的X,Y偏移
        /// </summary>

        public Vector2 AxisVector
        {
            get { return mMoveVector; }
        }


        /// <summary>
        /// 正在使用的输入状态数组
        /// </summary>

        public InputKeyState[] InputKeyStates
        {
            get { return mInputKeyStates; }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        
        public void Init()
        {
            mInputKeyStates = new InputKeyState[2];
            mInputKeyStates[0] = InputKeyState.CreateStateByType(GameInputType.Move, KeyCode.None);
            mInputKeyStates[1] = InputKeyState.CreateStateByType(GameInputType.Attack, KeyCode.J);
        }


        public void Update(float deltaTime)
        {
            ProcessMove();
            UpdateKeycode(deltaTime);
        }


        /// <summary>
        /// 处理Axis数据
        /// </summary>

        private void ProcessMove()
        {
            mMoveVector.x = Input.GetAxis("Horizontal");
            mMoveVector.y = -Input.GetAxis("Vertical");
            mAxleVector.x = Input.GetAxisRaw("Horizontal");
            mAxleVector.y = -Input.GetAxisRaw("Vertical");
        }


        /// <summary>
        /// 是否在移动
        /// </summary>
        /// <returns></returns>

        public bool WhetherToMove()
        {
            return !mAxleVector.x.Equals(0f) || !mAxleVector.y.Equals(0f);
        }


        /// <summary>
        /// 更新所有绑定的输入键状态
        /// </summary>
        /// <param name="dletaTime"></param>

        public void UpdateKeycode( float dletaTime )
        {
            for (int i = 0; i < mInputKeyStates.Length; i++)
            {
                mInputKeyStates[i].InterceptState(this, dletaTime);
            }
        }


        /// <summary>
        /// 根据类型获取一个输入状态
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>

        public InputKeyState GetKeycodeState(GameInputType type)
        {
            for (int i = 0; i < mInputKeyStates.Length; i++)
            {
                if (mInputKeyStates[i].InputType == type)
                    return mInputKeyStates[i];
            }
            return null;
        }


        /// <summary>
        /// 获取一个InputKeyState实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="state">返回</param>
        /// <returns></returns>

        public bool TryGetKeycodeState(GameInputType type, out InputKeyState state)
        {
            state = GetKeycodeState(type);
            return state != null;
        }



        #region Editor Only

        //public void OnGUI()
        //{
        //    GUI.Label(new Rect(0, 0, 70, 30), WhetherToMove() ? "true" : "false");
        //}

        #endregion
    }





}
