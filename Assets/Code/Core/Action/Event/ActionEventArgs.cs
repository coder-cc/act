using UnityEngine;
using System.Collections;


namespace Aqua.Action.Event
{

    public class ActionEventArgs
    {

        private int _time;
        private ActionEventType _eventType;


        public int Time
        {
            get { return _time;}
            set { _time = value; }
        }


        public ActionEventType EventType
        {
            get { return _eventType; }
        }


    }

}

