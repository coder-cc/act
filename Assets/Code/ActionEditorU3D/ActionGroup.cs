using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using System.Collections;



namespace ActionEditorU3D
{
    public class ActionGroup : MonoBehaviour
    {

        private String mRoleID;
        public String RoleID { get { return mRoleID; } set { mRoleID = value; } }


        private List<Action> mActionList = new List<Action>();
        public List<Action> ActionList { get { return mActionList; } set { mActionList = value; } }


        private String mGroupName;
        public String GroupName { get { return mGroupName; } set { mGroupName = value; } }


        private String mDefaultAction = "a0000";
        public String DefaultAction { get { return mDefaultAction; } set { mDefaultAction = value; } }

    }


}

