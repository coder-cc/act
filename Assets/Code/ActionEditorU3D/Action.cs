using System;
using UnityEngine;
using System.Collections;


namespace ActionEditorU3D
{
    public class Action 
    {

        private String mID = "A0000";
        public String ID { get { return mID; } set { mID = value; } }


        private String mName = "未命名";
        private String Name { get { return mName; } set { mName = value; } }


        private int mAnimTime = 500;
        public int AnimTime { get { return mAnimTime; }  set { mAnimTime = value; }}


        private int mPoseTime = 0;
        public int PoseTime { get { return mPoseTime; } set { mPoseTime = value; } }


        private int TotalTime { get { return mAnimTime + mPoseTime; } }


        private int mBlendTime = 0;
        public int BlendTime { get { return mBlendTime; } set { mBlendTime = value; } }


        private int mMoveSpeed = 0;
        public int MoveSpeed { get { return mMoveSpeed; } set { mMoveSpeed = value; } }

    }
}


