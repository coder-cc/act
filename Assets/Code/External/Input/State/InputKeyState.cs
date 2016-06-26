using UnityEngine;
using System.Collections;


namespace Aqua.InputEvent
{



    public class InputKeyState : InputStateBase
    {

        private readonly KeyCode mKeyCode;

        public KeyCode KeyCode
        {
            get { return mKeyCode; }
        }


        public InputKeyState(KeyCode keyCode)
        {
            mKeyCode = keyCode;
        }


        public override void InterceptState(InputManager mngInputManager, float deltaTime)
        {
            InterceptKeyCodeState(mKeyCode, deltaTime);
        }
    }

}

