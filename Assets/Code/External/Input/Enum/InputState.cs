using UnityEngine;
using System.Collections;

namespace Aqua.InputEvent
{


    public enum InputState
    {
        None = 0,

        /// <summary>
        /// 按下
        /// </summary>
        KeyDown,

        /// <summary>
        /// 持续按下
        /// </summary>
        KeyPressing,

        /// <summary>
        /// 双击
        /// </summary>
        KeyDoubleClick,

        /// <summary>
        /// 释放
        /// </summary>
        KeyRelease,
    }


}

