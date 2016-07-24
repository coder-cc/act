using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Core.Unit;
using CSTools;
using UnityEngine;

namespace Aqua.InputEvent
{

    public class InputManager : Singleton<InputManager>
    {

        private InputStateBase[] _mInputStatesBase;


        /// <summary>
        /// 正在使用的输入状态数组
        /// </summary>

        public InputStateBase[] InputStatesBase
        {
            get { return _mInputStatesBase; }
        }


        public ActionUnit Owner { get; private set; }
        //List<ActionUnit> 

        /// <summary>
        /// 初始化
        /// </summary>

        public void Init(ActionUnit owner)
        {
            Owner = owner;
            _mInputStatesBase = new InputStateBase[2];
            _mInputStatesBase[0] = InputStateBase.CreateStateByType(GameInputType.Move, KeyCode.None);
            _mInputStatesBase[1] = InputStateBase.CreateStateByType(GameInputType.Attack, KeyCode.J);
        }



        public void OnMove(Vector3 dir, float delta)
        {
            if (Owner != null)
            {
                Owner.OnKeyMove(dir, delta);
            }
        }


        public bool IsControllable(UnitBase unit)
        {
            return Owner == unit;
        }

        //public void OnKeyState(GameInputType type, KeyCode code)
        //{
        //    if (Owner != null)
        //    {
        //        Owner.OnKeyState(type, code);
        //    }
        //}


        public void Update(float deltaTime)
        {
            //ProcessMove();
            UpdateKeycode(deltaTime);
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

    }





}
