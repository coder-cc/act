using System;
using System.Collections.Generic;
using Aqua.InputEvent;
using Core.Controller;
using UnityEngine;

namespace Core.Unit
{
    public class ActionUnit : UnitBase , ICameraController
    {

        private ActionStatus mActionStatus;
        private Dictionary<string, int> mIntParameter;


        /// <summary>
        /// 动作全局参数
        /// </summary>
        public Dictionary<string, int> IntParameter
        {
            get { return mIntParameter; }
        }

        public Transform OwnerTransform
        {
            get
            {
                return CacheTransform;
                //throw new NotImplementedException();
            }
        }

        public Vector2 CameraOffset
        {
            get
            {
                return Vector2.zero;
                //throw new NotImplementedException();
            }
        }

        public ActionUnit(UnitInfo info)
            :base(info)
        {
            mIntParameter = new Dictionary<string, int>();
            ApplyParameter();
            mActionStatus = new ActionStatus(this);
        }


        public void ApplyParameter()
        {
            mIntParameter.Clear();
            const int count = (int)GameInputType.Size;
            for (int i = 0; i < count; i++)
            {
                mIntParameter.Add(((GameInputType)i).ToString(), 0);
            }   
        }


        public override void Init()
        {
            base.Init();

            //CreateGameObject(Info.HierarchyName);
            CreateModel(Info.ResourcePath);

            mActionStatus.ChangeActionGroup(Info.RoleID, 0);
            mActionStatus.ChangeToIdle();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            mActionStatus.Update(deltaTime);
        }

        //private void CreateGameObject(string name = "")
        //{
        //    CacheGameOject = new GameObject(name);
        //    CacheTransform = CacheGameOject.transform;

        //    CacheTransform.localPosition = new Vector3(0, 0.14f, -3);
        //    CacheTransform.localRotation = Quaternion.Euler(0, 180, 0);
        //}


        private void CreateModel(string res)
        {
            GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(res + ".prefab");
            GameObject model = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            SetModel(model);
        }


        public void OnKeyMove(Vector3 dir, float delta)
        {
            if (mActionStatus.ActiveActionData.CanMove == false)
                return;


            //if (mActionStatus.ActiveActionData.CanRotate)
            {
                CacheTransform.forward = dir.normalized;
            }

            Vector3 distance = dir*mActionStatus.ActiveActionData.MoveSpeed*0.01f*delta;
            PhysicsMove(distance);
        }


        //public void OnKeyState(GameInputType type, KeyCode code, float delta)
        //{
        //    mActionStatus.ProcessActionInterruptList(delta);
        //}


        /// <summary>
        /// action status call 动作切换
        /// </summary>
        /// <param name="actionData"></param>

        public void OnChangeAction(ActionData actionData)
        {
            PlayAnimation(actionData);
        }


        public void OnEnterPoseTime()
        {
            SetAnimationSpeed(0.001f);
        }


        public void SetAnimationSpeed(float speed)
        {
            if (mCachedAnimationState == null)
                return;
            AnimSpeed = speed;
            mCachedAnimationState.speed = speed;
        }


        virtual public void ProcessActiveAnimation(ActionData ActiveAction, float dirSpeed)
        {
            PlayAnimation(ActiveAction);
        }


        public void PlayAnimation(ActionData action)
        {
            AnimationState animState = FetchAnimation(action, 0, null);
            if (animState == null)
                return;

            PlayAnim(animState, action.BlendTime, false);
        }


        public void MoveDirector(Vector3 distance)
        {
            PhysicsMove(distance);
        }


        AnimationState FetchAnimation(ActionData action, float startTime, AnimSlotData data)
        {
            if (action == null || action.AnimSlotList.Count == 0 || CacheAnimation == null)
                return null;


            AnimSlotData animSlot = action.AnimSlotList[UnityEngine.Random.Range(0, action.AnimSlotList.Count - 1)];
            //if (action.MoveChange)
            //{
            //    for (int i = 0; i < action.AnimSlotList.Count; ++i)
            //    {
            //        if (!action.AnimSlotList[i].UseDir)
            //        {
            //            animSlot = action.AnimSlotList[i];
            //            break;
            //        }
            //    }

            //    if (data != null)
            //        animSlot = data;

            //    if (mActiveSlot != null)
            //        animSlot = mActiveSlot;

            //    if (mActiveSlot != null
            //        && CachedAnimationState != null
            //        && CachedAnimationState.name == animSlot.Animation)
            //        startTime = CachedAnimationState.normalizedTime;
            //}

            if (animSlot == null)
                return null;

            AnimationState animState = null;
            animState = CacheAnimation[animSlot.Animation];
            if (animState == null)
            {
                Debug.LogError(string.Format("Fail to change animation: {0}/{1}/{2}", Info.RoleID, action.AnimId, animSlot.Animation));
                return null;
            }

            //  动画起始帧
            animState.normalizedTime = startTime.Equals(0) ? animSlot.Start * 0.01f : startTime;

            //  动画速度
            //  0 - 100 = 1
            //  20 -100 = 0.8
            animState.speed = (animSlot.End - animSlot.Start) * animState.length * 10.0f / (action.AnimTime);

            return animState;
        }

    }
}
