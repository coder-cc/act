﻿using UnityEngine;
using System.Collections;
using Aqua.InputEvent;


namespace Aqua.InputEvent
{


    public class InputMoveState : InputStateBase
    {



        private Vector2 _moveVector;
        private Vector2 _axleVector;


        private void ProcessMove()
        {
            //  0 lerp to 1
            _moveVector.x = Input.GetAxis("Horizontal");
            _moveVector.y = -Input.GetAxis("Vertical");

            //  0 or 1
            _axleVector.x = Input.GetAxisRaw("Horizontal");
            _axleVector.y = -Input.GetAxisRaw("Vertical");
        }


        public bool WhetherToMove()
        {
            return !_axleVector.x.Equals(0f) || !_axleVector.y.Equals(0f);
        }


        public override void InterceptState(InputManager mngInputManager, float deltaTime)
        {

            ProcessMove();

            if (WhetherToMove())
            {
                if (State == InputState.KeyPressing)
                {
                    IsDown = false;
                    PressedTime += deltaTime;
                    InputManager.Instance.OnMove(new Vector3(_moveVector.x, 0f, -_moveVector.y), deltaTime);
                }
                else
                {
                    IsDown = true;
                    IsPress = true;
                    IsUp = false;
                    State = InputState.KeyPressing;
                    ReleasedTime = 0f;
                }
            }
            else
            {
                if (State == InputState.KeyPressing)
                {
                    IsDown = false;
                    IsPress = false;
                    IsUp = true;
                    State = InputState.KeyRelease;
                    PressedTime = 0f;
                }
                else
                {
                    IsUp = false;
                    ReleasedTime += deltaTime;
                }
            }
        }

    }

}


