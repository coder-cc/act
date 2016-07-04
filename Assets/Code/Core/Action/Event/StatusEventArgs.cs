using UnityEngine;
using System.Collections;

namespace Aqua.Action.Event
{

    /// <summary>
    /// 状态数据类型
    /// </summary>

    public class StatusEventArgs : ActionEventArgs
    {
        public string StatusName { get; set; }

        public string StatusValue { get; set; }
    }

}

