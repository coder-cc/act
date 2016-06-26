using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using CSTools;
using UnityEngine;

namespace Aqua.InputEvent
{

    public class InputManager : Singleton<InputManager>
    {

        private Vector2 mMoveVector;
        private Vector2 mAxleVector;
        private InputStateBase[] _mInputStatesBase;



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

        public InputStateBase[] InputStatesBase
        {
            get { return _mInputStatesBase; }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        
        public void Init()
        {
            _mInputStatesBase = new InputStateBase[2];
            _mInputStatesBase[0] = InputStateBase.CreateStateByType(GameInputType.Move, KeyCode.None);
            _mInputStatesBase[1] = InputStateBase.CreateStateByType(GameInputType.Attack, KeyCode.J);
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
            //  0 lerp to 1
            mMoveVector.x = Input.GetAxis("Horizontal");
            mMoveVector.y = -Input.GetAxis("Vertical");

            //  0 or 1
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
            for (int i = 0; i < _mInputStatesBase.Length; i++)
            {
                _mInputStatesBase[i].InterceptState(this, dletaTime);
            }
        }


        /// <summary>
        /// 根据类型获取一个输入状态
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>

        public InputStateBase GetKeycodeState(GameInputType type)
        {
            for (int i = 0; i < _mInputStatesBase.Length; i++)
            {
                if (_mInputStatesBase[i].InputType == type)
                    return _mInputStatesBase[i];
            }
            return null;
        }


        /// <summary>
        /// 获取一个InputKeyState实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="stateBase">返回</param>
        /// <returns></returns>

        public bool TryGetKeycodeState(GameInputType type, out InputStateBase stateBase)
        {
            stateBase = GetKeycodeState(type);
            return stateBase != null;
        }



        #region Editor Only

        //public void OnGUI()
        //{
        //    GUI.Label(new Rect(0, 0, 70, 30), WhetherToMove() ? "true" : "false");
        //}

        #endregion
    }





}
