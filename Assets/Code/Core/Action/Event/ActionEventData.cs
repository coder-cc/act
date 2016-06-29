using UnityEngine;
using System.Collections;


namespace Aqua.Action.Event
{

    public class ActionEventData
    {

        private int _time;
        private ActionEventType _eventType;


        public int Time
        {
            get { return _time;}
        }


        public ActionEventType EventType
        {
            get { return _eventType; }
        }



        //public abstract void Execute(IActionEventOwner eventOwner);
    }

}

