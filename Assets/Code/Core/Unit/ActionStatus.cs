using System.Collections.Generic;
using UnityEngine;
using System.Collections;
//using GameKeycodeType = Core.Manager.GameKeycodeType;


namespace Core.Unit
{

    public class ActionStatus
    {

        private ActionUnit mOwnerUnit;
        private float mTotalTime;
        private int mActionTime;
        private int mActionKey = -1;
        private bool mIsActionChange;

        private ActionData mActiveAction;
        private ActionGroupData mActionGroupData;

        private int mActionInterruptEnabled = 0;

        private int mQueuedInterruptTime = 0;
        private ActionInterrupt mQueuedInterrupt;

        /// <summary>
        /// 数据映射
        /// </summary>
        //private Dictionary<string, int> mIntParameter;


        private Dictionary<string, int> ActionParameter
        {
            get { return mOwnerUnit.IntParameter; }
        }



        public ActionStatus(ActionUnit owner)
        {
            mOwnerUnit = owner;
            //mIntParameter = new Dictionary<string, int>();
            //InitIntParameter();
        }

        public void ChangeActionGroup(int roleid, int groupnum)
        {
            mActionGroupData = ActionHelp.Instance.GetActionGroupData(roleid, groupnum);
        }


        public void Update(float deltaTime)
        {
            int preTime = (int)mTotalTime;
            mTotalTime = (mTotalTime + (deltaTime * 1000)) % 9000000;
            if (preTime > mTotalTime)
                preTime = 0;

            int curTime = (int)mTotalTime;

            //UpdateInput();

            if (mIsActionChange)
            {
                ResetActionState();
                mIsActionChange = false;
            }

            if (mActiveAction != null)
            {
                TickAction(curTime - preTime, deltaTime);
            }
        }


    //    private void UpdateInput()
    //    {
    //        InputStateBase inputKeyState = null;
    //        if (InputManager.Instance.TryGetKeycodeState(GameInputType.Move, out inputKeyState))
    //        {
    //            SetIntParameter(GameInputType.Move.ToString(), (int) inputKeyState.State);
				//Debug.Log(string.Format("Type [{0}] State [{1}]", GameInputType.Move, inputKeyState.State));
    //        }

    //        if (InputManager.Instance.TryGetKeycodeState(GameInputType.Attack, out inputKeyState))
    //        {
    //            SetIntParameter(GameInputType.Attack.ToString(), (int)inputKeyState.State);
    //        }

    //    }


        //private void SetIntParameter(string key, int value)
        //{
        //    if (ActionParameter.ContainsKey(key))
        //        ActionParameter[key] = value;
        //}


        //private bool IntParameterExist(string key)
        //{
        //    return ActionParameter.ContainsKey(key);
        //}


        public void ChangeToIdle()
        {
            ChangeAction("N0000", 0);
        }


        void TickAction(int deltaTime, float dt)
        {

            if (ProcessInterruptEveryFrame())
            {
                return;
            }

            // 检测是否有[等待]中断动画
            if (ProcessQueuedAction(deltaTime))
            {
                return;
            }


            int nextActionTime = 0;
            bool thisActionIsFinished = false;
            if ((mActionTime + deltaTime) > ActionHelp.GetActionTotalTime(mActiveAction))
            {
                nextActionTime = deltaTime;

                deltaTime = ActionHelp.GetActionTotalTime(mActiveAction) - mActionTime;
                nextActionTime -= deltaTime;

                thisActionIsFinished = true;
            }



            int nextActionKey = GetNextKey(deltaTime);
            if (nextActionKey > mActionKey)
            {
                int nextKeyTime;
                if (nextActionKey > 100)
                {
                    nextKeyTime = (mActiveAction.AnimTime + mActiveAction.PoseTime)*nextActionKey/200;
                }
                else
                    nextKeyTime = mActiveAction.AnimTime*nextActionKey/100;

                //  event

                //  hitdefine

                //  Interrupt
                if (ProcessActionInterruptList(mActionKey, nextActionKey))
                {
                    return;
                }


                if (mActiveAction.PoseTime > 0 && mActionKey < 100 && nextActionKey >= 100)
                {
                    //  enter posetime
                    mOwnerUnit.OnEnterPoseTime();
                }

                //  hack the event interrupts. 
            }


            mActionTime += deltaTime;
            mActionKey = nextActionKey;

            if (thisActionIsFinished)
            {
                ProcessAnimFinish(nextActionTime);
            }
        }



        /// <summary>
        /// 检测中断
        /// </summary>
        /// <returns></returns>
        bool ProcessInterruptEveryFrame()
        {
            if (mQueuedInterrupt != null)
                return false;

            if (mActiveAction.InterruptList.Count == 0)
                return false;

            int iCount = mActiveAction.InterruptList.Count;
            for (int i = 0; i < iCount; i++)
            {
                ActionInterrupt interrupt = mActiveAction.InterruptList[i];

                if (InterruptEnabled(i))
                {
                    if (ProcessActionInterrupt(interrupt))
                    {
                        return true;
                    }

                    //if (InterruptEnabled(i))
                    //{
                    //    return LinkAction(interrupt);
                    //}
                }


            }


            return false;
        }


        bool ProcessActionInterruptList(int preKey, int nextKey)
        {
            if (mQueuedInterrupt != null)
                return false;

            // check the action interrupts
            if (mActiveAction.InterruptList.Count == 0)
                return false;

            int iCount = mActiveAction.InterruptList.Count;

            for (int i = 0; i < iCount; i++)
            {
                ActionInterrupt interrupt = mActiveAction.InterruptList[i];

                //  是否满足检测时间段,标记为开启
                if (interrupt.DetectionStartTime != 0 && interrupt.DetectionStartTime > preKey && interrupt.DetectionStartTime <= nextKey)
                    mActionInterruptEnabled |= (1 << i);

                //  是否超过检测时间段,标记为关闭
                if (interrupt.DetectionEndTime != 200 && interrupt.DetectionEndTime > preKey && interrupt.DetectionEndTime <= nextKey)
                    mActionInterruptEnabled &= ~(1 << i);

                //if (interrupt.SkillID != 0 && mOwner.HasSkillInput(interrupt.SkillID) && GetInterruptEnabled(i))
                //{
                //    return LinkAction(interrupt);
                //}

                if (InterruptEnabled(i))
                {
                    if (ProcessActionInterrupt(interrupt))
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 处理中断
        /// </summary>
        /// <param name="interrupt"></param>
        /// <returns></returns>

        bool ProcessActionInterrupt(ActionInterrupt interrupt)
        {
            if (!CheckActionInterrupt(interrupt))
                return false;

            return LinkAction(interrupt);
        }

        bool InterruptEnabled(int index)
        {
            return (mActionInterruptEnabled & (1 << index)) != 0;
        }


        /// <summary>
        /// 检测中断条件
        /// </summary>
        /// <param name="interrupt"></param>
        /// <returns></returns>

        bool CheckActionInterrupt(ActionInterrupt interrupt)
        {

            //  检测中断条件 (与)
            if (interrupt.ConditionType == ConditionType.And)
            {
                for (int i = 0; i < interrupt.Conditions.Count; i++)
                {
                    if (interrupt.Conditions[i].Check(mOwnerUnit) == false)
                        return false;
                }

                return true;
            }
            //  检测中断条件 (或)
            else
            {
                for (int i = 0; i < interrupt.Conditions.Count; i++)
                {
                    if (interrupt.Conditions[i].Check(mOwnerUnit))
                        return true;
                }
                return false;
            }
        }


        /// <summary>
        /// 处理中断动作连接
        /// </summary>
        /// <param name="interrupt"></param>
        /// <returns></returns>

        bool LinkAction(ActionInterrupt interrupt)
        {
            if (ActionHelp.GetActionIndex(mActionGroupData, interrupt.ActionID) == -1)
            {
                Debug.LogError(string.Format("Can't find ActionID {0}", interrupt.ActionID.ToString()));
                return false;
            }


            bool connectImmediately = interrupt.ConnectMode == ActionInterruptConnectMode.Immediately;
            if (interrupt.ConnectMode == ActionInterruptConnectMode.WaitFinish)
            {
                //  中断触发的时机是当前动画播放完成
                int actualQueuedTime = interrupt.ConnectTime <= 100 ?
                    mActiveAction.AnimTime * interrupt.ConnectTime / 100 :	// [0-100] AnimTime
                    mActiveAction.AnimTime + mActiveAction.PoseTime * (interrupt.ConnectTime - 100) / 100;

                if (actualQueuedTime <= mActionTime)
                    connectImmediately = true;

                mQueuedInterrupt = interrupt;
                mQueuedInterruptTime = actualQueuedTime;
            }

            //  立即中断到指定动画
            if (connectImmediately || interrupt.ConnectMode == ActionInterruptConnectMode.Immediately)
            {
                ChangeAction(interrupt.ActionID, 0);
            }

            //else
            //{
            //    ChangeAction(interrupt.ActionID, 0);
            //}


            return true;

            //else if (interrupt.ConnectMode == ActionInterruptConnectMode.WaitFinish)
            //{

            //}

        }


        /// <summary>
        /// 检测等待中断的时间是否到达, 一般是当前动画完成.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        bool CheckTime(int deltaTime)
        {
            return (mActionTime == 0 && mQueuedInterruptTime == 0) || 
                   (mActionTime < mQueuedInterruptTime && mActionTime + deltaTime >= mQueuedInterruptTime);
        }

        /// <summary>
        /// 处理等待中断
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        bool ProcessQueuedAction(int deltaTime)
        {

            if (mQueuedInterrupt == null)
                return false;

            //  时间是否满足
            if (!CheckTime(deltaTime))
                return false;

            //  切换
            int nextActionTime = mActionTime + deltaTime - mQueuedInterruptTime;
            ChangeAction(mQueuedInterrupt.ActionID, nextActionTime);
            mQueuedInterrupt = null;

            return true;
        }



        void ProcessAnimFinish(int nextActionTime)
        {
            //if ()
            //{

            //}

            string nextAction = mActiveAction.DefaultLinkActionId;


            //Change
            ChangeAction(nextAction, nextActionTime);

        }

        /// <summary>
        /// 当前动画百分比 
        /// 0 - 100 <= AnimTime
        /// 101-199 <= TotalTime (这个是算上pose时间)
        /// 200 pose时间也结束了
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        int GetNextKey(int deltaTime)
        {
            if (mActiveAction == null) return -1;

            int currentTime = mActionTime + deltaTime;

            // [0-100]
            if (currentTime <= mActiveAction.AnimTime)
                return currentTime * 100 / mActiveAction.AnimTime;

            // [200-...]
            if (currentTime >= ActionHelp.GetActionTotalTime(mActiveAction))
                return 200;

            // [101-199]
            //int leftTime = currentTime - mActiveAction.AnimTime;
            return 100 + currentTime - mActiveAction.AnimTime * 100 / mActiveAction.PoseTime;
        }

        bool ChangeAction(string id, int animDeltaTime)
        {

            int indexAction = ActionHelp.GetActionIndex(mActionGroupData, id);
            if (indexAction < 0)
            {
                return false;
            }

            ChangeAction(indexAction, animDeltaTime);
            return true;
        }


        public void ChangeAction(int indexAction, int animDeltaTime)
        {
            ActionData oldAction = mActiveAction;
            ActionData newAction = mActionGroupData.ActionDatas[indexAction];
            
            //  新动画需处理
            mActiveAction = newAction;
            //mActiveAction.


            ResetAfterChange();
        }



        /// <summary>
        /// 动画切换时重置一些数据
        /// </summary>

        void ResetAfterChange()
        {
            ResetInterruptList();

            mActionTime = 0;
            mIsActionChange = true;
        }

        void ResetInterruptList()
        {
            mActionInterruptEnabled = 0;

            // copy the action request enabled/disabled flags.
            for (int i = 0; i < mActiveAction.InterruptList.Count; i++)
            {
                ActionInterrupt actionInterrupt = mActiveAction.InterruptList[i];
                mActionInterruptEnabled |= (1 << i);
                //if (actionInterrupt.InputEnable)
                //    mActionInterruptEnabled |= (1 << i);
            }
        }

        /// <summary>
        /// 重置动画状态
        /// </summary>

        void ResetActionState()
        {
            
            mOwnerUnit.OnChangeAction(mActiveAction);
        }


        //void Reset()
        //{

        //    //mOwnerUnit.
        //}
    }
}
