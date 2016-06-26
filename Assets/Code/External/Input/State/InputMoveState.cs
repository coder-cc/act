using UnityEngine;
using System.Collections;
using Aqua.InputEvent;


namespace Aqua.InputEvent
{


    public class InputMoveState : InputStateBase
    {

        public override void InterceptState(InputManager mngInputManager, float deltaTime)
        {
            if (mngInputManager.WhetherToMove())
            {
                if (State == InputState.KeyPressing)
                {
                    PressedTime += deltaTime;
                }
                else
                {
                    State = InputState.KeyPressing;
                    ReleasedTime = 0f;
                }
            }
            else
            {
                if (State == InputState.KeyPressing)
                {
                    State = InputState.KeyRelease;
                    PressedTime = 0f;
                }
                else
                {
                    ReleasedTime += deltaTime;
                }
            }
        }

    }

}


