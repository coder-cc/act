using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Aqua.Action.Event;
using Aqua.InputEvent;

//using GameKeycodeType = Core.Manager.GameKeycodeType;


namespace Core.Unit
{

    public class ActionStatus
    {

        private ActionUnit _ownerUnit;
        private float _totalTime;
        private int _actionTime;
        private int _actionKey = -1;

        //private bool testChange;

        private bool _isActionChange;
        //{
        //    get
        //    {
        //        return testChange;
        //    }
        //    set
        //    {


        //        testChange = value;
        //        //Idle
        //    }
        //}

        private int _eventIndex = 0;

        private ActionData _activeActionData;
        private ActionGroupData _actionGroupData;

        private int _actionInterruptFlag = 0;
        private int _queuedInterruptTime = 0;
        private ActionInterrupt _queuedInterrupt;


        ExternForce _externForce;

        public ActionUnit Onwer
        {
            get
            {
                return _ownerUnit;
            }
        }



        public ActionData ActiveActionData
        {
            get { return _activeActionData;}
        }


        public ActionStatus(ActionUnit owner)
        {
            _ownerUnit = owner;
            _externForce = new ExternForce(_ownerUnit.OwnerTransform);
            //mIntParameter = new Dictionary<string, int>();
            //InitIntParameter();
        }

        public void ChangeActionGroup(int roleid, int groupnum)
        {
            _actionGroupData = ActionHelp.Instance.GetActionGroupData(roleid, groupnum);
        }


        public void Update(float deltaTime)
        {
            int preTime = (int)_totalTime;
            _totalTime = (_totalTime + (deltaTime * 1000)) % 9000000;
            if (preTime > _totalTime)
                preTime = 0;

            int curTime = (int)_totalTime;

            //UpdateInput();

            if (_isActionChange)
            {
                ResetActionState();
                _isActionChange = false;
            }

            if (_activeActionData != null)
            {
                TickAction(curTime - preTime, deltaTime);
            }
        }



        public void ChangeToIdle()
        {
            ChangeAction("N0000", 0);
        }


        void TickAction(int deltaTime, float dt)
        {

            //if (ProcessInterruptEveryFrame())
            //{
            //    return;
            //}

            // 检测是否有[等待]中断动画
            if (ProcessQueuedInterrupt(deltaTime))
            {
                return;
            }


            int nextActionTime = 0;
            bool thisActionIsFinished = false;
            if ((_actionTime + deltaTime) > ActionHelp.GetActionTotalTime(_activeActionData))
            {
                nextActionTime = deltaTime;

                deltaTime = ActionHelp.GetActionTotalTime(_activeActionData) - _actionTime;
                nextActionTime -= deltaTime;

                thisActionIsFinished = true;
            }



            int nextActionKey = GetNextKey(deltaTime);
            tempNextKey = nextActionKey;
            if (nextActionKey > _actionKey)
            {
                int nextKeyTime;
                if (nextActionKey > 100)
                {
                    nextKeyTime = (_activeActionData.AnimTime + _activeActionData.PoseTime)*nextActionKey/200;
                }
                else
                    nextKeyTime = _activeActionData.AnimTime*nextActionKey/100;

                //  事件处理
                ProcessEvent(nextKeyTime, deltaTime);

                //  hit define

                //  中断处理
                if (ProcessActionInterruptList(_actionKey, nextActionKey))
                {
                    return;
                }


                //  如果有Pose,尝试进入Pose状态
                if (_activeActionData.PoseTime > 0 && _actionKey < 100 && nextActionKey >= 100)
                {
                    //  enter posetime 
                    //  animation speed 0.001f
                    _ownerUnit.OnEnterPoseTime();
                }

                //  hack the event interrupts. 
            }


            ProcessMoving(dt);

            _actionTime += deltaTime;
            beforKey = _actionKey;
            _actionKey = nextActionKey;

            if (thisActionIsFinished)
            {
                ProcessAnimFinish(nextActionTime);
            }
        }


        void ProcessEvent(int nextKey, int deltaTime)
        {
            if (_activeActionData.EventArgses == null || _activeActionData.EventArgses.Count == 0)
                return;

            if (_eventIndex >= _activeActionData.EventArgses.Count)
                return;

            for (int i = _eventIndex; i < _activeActionData.EventArgses.Count; i++)
            {
                var eventValue = _activeActionData.EventArgses[i];
                if (eventValue.Time >= _actionKey && eventValue.Time <= nextKey)
                {
                    // 触发
                    EventExecute.ExecuteEvent(this, eventValue, deltaTime);
                    ++_eventIndex;
                }
            }

        }


        /// <summary>
        /// 检测中断
        /// </summary>
        /// <returns></returns>
        bool ProcessInterruptEveryFrame()
        {
            if (_queuedInterrupt != null)
                return false;

            if (_activeActionData.InterruptList.Count == 0)
                return false;

            int iCount = _activeActionData.InterruptList.Count;
            for (int i = 0; i < iCount; i++)
            {
                ActionInterrupt interrupt = _activeActionData.InterruptList[i];

                if (InterruptEnabled(i))
                {
                    if (ProcessActionInterrupt(interrupt))
                    {
                        return true;
                    }
                }


            }


            return false;
        }



        //public void ProcessKeyActionInterrupt(float delta)
        //{
        //    int preKey = _actionKey;
        //    int nextKey = GetNextKey((int) (delta*1000));

        //    if (_queuedInterrupt != null)
        //        return;

        //    // check the action interrupts
        //    if (_activeActionData.InterruptList.Count == 0)
        //        return;

        //    int iCount = _activeActionData.InterruptList.Count;

        //    for (int i = 0; i < iCount; i++)
        //    {
        //        ActionInterrupt interrupt = _activeActionData.InterruptList[i];

        //        //if (interrupt.)
        //        //if (interrupt.ke)

        //        if ((preKey >= interrupt.DetectionStartTime || nextKey >= interrupt.DetectionStartTime) &&
        //            (preKey <= interrupt.DetectionEndTime || nextKey <= interrupt.DetectionEndTime))
        //        {
        //            //if ((_actionInterruptFlag & (1 << i)) == 0 && interrupt.InterruptName.Contains("Idle") == false)
        //            //{
        //            //    Debug.Log(string.Format("开始中断检测 {0} cur {1} next {2} [start {3} end {4}]", interrupt.InterruptName, _actionKey, tempNextKey, interrupt.DetectionStartTime, interrupt.DetectionEndTime));
        //            //}
        //            _actionInterruptFlag |= (1 << i);
        //        }
        //        else
        //        {
        //            //if ((_actionInterruptFlag & (1 << i)) == 1 && interrupt.InterruptName.Contains("Idle") == false)
        //            //{
        //            //    Debug.Log(string.Format("结束中断检测 {0} cur {1} next {2} [start {3} end {4}]", interrupt.InterruptName, _actionKey, tempNextKey, interrupt.DetectionStartTime, interrupt.DetectionEndTime));
        //            //}
        //            _actionInterruptFlag &= ~(1 << i);
        //        }

        //        if (InterruptEnabled(i))
        //        {
        //            if (ProcessActionInterrupt(interrupt))
        //                return;
        //        }
        //    }
        //    //return false;
        //    //ProcessActionInterruptList(_actionKey, GetNextKey((int) (delta*1000)));
        //}


        public bool ProcessActionInterruptList(int preKey, int nextKey)
        {
            if (_queuedInterrupt != null)
                return false;

            // check the action interrupts
            if (_activeActionData.InterruptList.Count == 0)
                return false;

            int iCount = _activeActionData.InterruptList.Count;

            for (int i = 0; i < iCount; i++)
            {
                ActionInterrupt interrupt = _activeActionData.InterruptList[i];
                //if (interrupt.ke)

                if ((preKey >= interrupt.DetectionStartTime || nextKey >= interrupt.DetectionStartTime) &&
                    (preKey <= interrupt.DetectionEndTime || nextKey <= interrupt.DetectionEndTime))
                {
                    if ((_actionInterruptFlag & (1 << i)) == 0 && interrupt.InterruptName.Contains("Idle") == false)
                    {
                        Debug.Log(string.Format("开始中断检测 {0} cur {1} next {2} [start {3} end {4}]", interrupt.InterruptName, _actionKey, tempNextKey, interrupt.DetectionStartTime, interrupt.DetectionEndTime));
                    }
                    _actionInterruptFlag |= (1 << i);
                }
                else
                {
                    if ((_actionInterruptFlag & (1 << i)) == 1 && interrupt.InterruptName.Contains("Idle") == false)
                    {
                        Debug.Log(string.Format("结束中断检测 {0} cur {1} next {2} [start {3} end {4}]", interrupt.InterruptName, _actionKey, tempNextKey, interrupt.DetectionStartTime, interrupt.DetectionEndTime));
                    }
                    _actionInterruptFlag &= ~(1 << i);
                }
                //if (/*interrupt.DetectionStartTime != 0 &&*/ interrupt.DetectionStartTime > preKey &&
                //    interrupt.DetectionStartTime <= nextKey)
                //{
                //    if ((_actionInterruptFlag & (1 << i)) == 0)
                //    {
                //        Debug.Log(string.Format("开始中断检测 {0} cur {1} next {2} [start {3} end {4}]", interrupt.InterruptName, _actionKey, tempNextKey, interrupt.DetectionStartTime, interrupt.DetectionEndTime));
                //    }
                //    _actionInterruptFlag |= (1 << i);
                //}

                //if (/*interrupt.DetectionEndTime != 200 &&*/ interrupt.DetectionEndTime > preKey &&
                //    interrupt.DetectionEndTime <= nextKey)
                //{

                //    if ((_actionInterruptFlag & (1 << i)) == 1)
                //    {
                //        Debug.Log(string.Format("结束中断检测 {0} cur {1} next {2} [start {3} end {4}]", interrupt.InterruptName, _actionKey, tempNextKey, interrupt.DetectionStartTime, interrupt.DetectionEndTime));
                //    }

                //    _actionInterruptFlag &= ~(1 << i);
                //}

                if (InterruptEnabled(i))
                {
                    if (ProcessActionInterrupt(interrupt))
                        return true;

                    //Debug.Log(string.Format("{0} key {1} next {2}", interrupt.InterruptName, _actionKey, nextKey));
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
            return (_actionInterruptFlag & (1 << index)) != 0;
        }

        private int tempNextKey;
        private int beforKey;

        /// <summary>
        /// 检测中断条件
        /// </summary>
        /// <param name="interrupt">true 触发中断,否则不处理.</param>
        /// <returns></returns>

        bool CheckActionInterrupt(ActionInterrupt interrupt)
        {

            //  检测中断条件 (与)
            if (interrupt.ConditionType == ConditionType.And)
            {
                for (int i = 0; i < interrupt.Conditions.Count; i++)
                {
                    if (interrupt.Conditions[i].Check(_ownerUnit) == false)
                        return false;
                }

                return true;
            }
            //  检测中断条件 (或)
            else
            {
                for (int i = 0; i < interrupt.Conditions.Count; i++)
                {
                    if (interrupt.Conditions[i].Check(_ownerUnit))
                    {
                        if (interrupt.ActionID == "W10020")
                        {
                            Debug.Log(string.Format("中断成功 {0} key {1} next key {2} State {3} Interrupt {4} Frame {5}", interrupt.InterruptName, _actionKey, tempNextKey,
                                InputManager.Instance.GetKeycodeState(GameInputType.Attack).State, _actionInterruptFlag, Time.frameCount));
                            //Debug.log
                        }
                        
                        return true;
                    }
                    //else
                    //    Debug.Log(string.Format("中断失败 {0} key {1} next key {2}", interrupt.InterruptName, _actionKey, tempNextKey));
                }
                return false;
            }
        }


        /// <summary>
        /// 处理中断
        /// 立即模式 立即由切换Action.
        /// 等待完成 等待动画播放完成后执行.
        /// </summary>
        /// <param name="interrupt"></param>
        /// <returns></returns>

        bool LinkAction(ActionInterrupt interrupt)
        {

            if (ActionHelp.GetActionIndex(_actionGroupData, interrupt.ActionID) == -1)
            {
                Debug.LogError(string.Format("Can't find ActionID {0}", interrupt.ActionID.ToString()));
                return false;
            }

            bool connectImmediately = interrupt.ConnectMode == ActionInterruptConnectMode.Immediately;
            if (interrupt.ConnectMode == ActionInterruptConnectMode.WaitFinish)
            {
                //  中断触发的时机是当前动画播放完成
                int actualQueuedTime = interrupt.ConnectTime <= 100 ?
                    _activeActionData.AnimTime * interrupt.ConnectTime / 100 :	// [0-100] AnimTime
                    _activeActionData.AnimTime + _activeActionData.PoseTime * (interrupt.ConnectTime - 100) / 100;

                if (actualQueuedTime <= _actionTime)
                    connectImmediately = true;

                _queuedInterrupt = interrupt;
                _queuedInterruptTime = actualQueuedTime;
            }

            //  立即中断到指定动画
            if (connectImmediately || interrupt.ConnectMode == ActionInterruptConnectMode.Immediately)
            {
                ChangeAction(interrupt.ActionID, 0);
            }

            return true;
        }


        /// <summary>
        /// 当前生效的中断事件,是否到达执行时间.
        /// </summary>
        /// <param name="deltaTime">当前帧间隔时间</param>
        /// <returns></returns>
        bool CheckTime(int deltaTime)
        {
            return (_actionTime == 0 && _queuedInterruptTime == 0) || 
                   (_actionTime < _queuedInterruptTime && _actionTime + deltaTime >= _queuedInterruptTime);
        }


        /// <summary>
        /// 等待中断处理程序
        /// </summary>
        /// <param name="deltaTime">当前帧间隔时间</param>
        /// <returns></returns>
        bool ProcessQueuedInterrupt(int deltaTime)
        {

            if (_queuedInterrupt == null)
                return false;

            //  时间是否满足
            if (!CheckTime(deltaTime))
                return false;

            //  误差时间,下一帧切换, 动画播放时长 - 中断时间.
            int nextActionTime = _actionTime + deltaTime - _queuedInterruptTime;
            ChangeAction(_queuedInterrupt.ActionID, nextActionTime);
            _queuedInterrupt = null;

            return true;
        }


        /// <summary>
        /// 动画完成处理程序
        /// (切换动画, 每个动画结束都有一个默认动画连接ID)
        /// </summary>
        /// <param name="nextActionTime"></param>

        void ProcessAnimFinish(int nextActionTime)
        {
            string nextAction = _activeActionData.DefaultLinkActionId;

            //  切换动画, 每个动画结束都有一个默认动画连接ID
            ChangeAction(nextAction, nextActionTime);
        }


        /// <summary>
        /// 获取下一个动画Key
        /// 0 - 100 <= AnimTime
        /// 101-199 <= TotalTime (这个是算上pose时间)
        /// 200 pose时间也结束了
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        int GetNextKey(int deltaTime)
        {
            if (_activeActionData == null) return -1;

            int currentTime = _actionTime + deltaTime;

            // [0-100]
            if (currentTime <= _activeActionData.AnimTime)
                return currentTime * 100 / _activeActionData.AnimTime;

            // [200-...]
            if (currentTime >= ActionHelp.GetActionTotalTime(_activeActionData))
                return 200;

            // [101 - TotalTime(animTime + poseTime)]
            //int leftTime = currentTime - _activeActionData.AnimTime;
            return 100 + (currentTime - _activeActionData.AnimTime) * 100 / _activeActionData.PoseTime;
        }


        /// <summary>
        /// 切换动画
        /// </summary>
        /// <param name="id">动画ID</param>
        /// <param name="animDeltaTime">补偿上下帧间的误差</param>
        /// <returns></returns>

        bool ChangeAction(string id, int animDeltaTime)
        {

            int indexAction = ActionHelp.GetActionIndex(_actionGroupData, id);
            if (indexAction < 0)
            {
                return false;
            }

            ChangeAction(indexAction, animDeltaTime);

            //if (beforName == _activeActionData.Name)
            //{
            //    //if (oldAction.Name != "Idle")
            //    //    Debug.Log("asdf");
            //    beforName = _activeActionData.Name;
            //}
            //else
            //{
            //    if (beforName != "Idle")
            //        Debug.Log("asdf");
            //}

            return true;
        }


        public void ChangeAction(int indexAction, int animDeltaTime)
        {
            ActionData oldAction = _activeActionData;
            ActionData newAction = _actionGroupData.ActionDatas[indexAction];
            
            //  新动画需处理
            _activeActionData = newAction;
            //_activeActionData.

  

            ResetAfterChange();
        }



        /// <summary>
        /// 动画切换时重置一些数据
        /// </summary>

        void ResetAfterChange()
        {
            ResetInterruptList();

            _actionTime = 0;
            _isActionChange = true;


            //if (beforName != _activeActionData.Name)
            //{
            //    //if (oldAction.Name != "Idle")
            //    //    Debug.Log("asdf");
            //    beforName = _activeActionData.Name;
            //}
            //else
            //{
            //    if (beforName != "Idle")
            //        Debug.Log("asdf");
            //}


            _eventIndex = 0;
        }
        //private string beforName = string.Empty;

        /// <summary>
        /// 重置中断标记
        /// </summary>

        void ResetInterruptList()
        {
            _actionInterruptFlag = 0;
            _queuedInterrupt = null;
            // copy the action request enabled/disabled flags.
            //for (int i = 0; i < _activeActionData.InterruptList.Count; i++)
            //{
            //    ActionInterrupt actionInterrupt = _activeActionData.InterruptList[i];
            //    _actionInterruptFlag |= (1 << i);
            //    //if (actionInterrupt.InputEnable)
            //    //    _actionInterruptFlag |= (1 << i);
            //}
        }


        /// <summary>
        /// 重置动画状态
        /// </summary>

        void ResetActionState()
        {
            _ownerUnit.OnChangeAction(_activeActionData);
 
            //Debug.Log("change " + _activeActionData.Name + " " + Time.frameCount);
        }




        public void SetVelocity(float x, float y, float z, int DeltaTime)
        {
            _externForce.OwnerTransform = _ownerUnit.OwnerTransform;
            _externForce.SetForce(x, y, z);

            ProcessMoving(DeltaTime * 0.001f);
        }

        bool ProcessMoving(float dt)
        {

            _ownerUnit.MoveDirector(_externForce.Sampling(dt));
            //Vector3 MoveDistance = Vector3.zero;

            //MoveDistance += mExternVelocity.GetMove(dt) + mMoveVelocity * dt;

            //MoveDistance = _ownerUnit.OwnerTransform.TransformDirection(MoveDistance);
            //_ownerUnit.MoveDirector(MoveDistance);

            //Debug.Log(MoveDistance);
            return true;
        }

        //void ResetRush()
        //{
        //    mRushVelocity = Vector2.zero;
        //    mRushDirection = Vector2.zero;
        //    mEventRushDirection = Vector2.zero;
        //    mRushStrange = 0;
        //    mVelocityY = 0;
        //}

        //float mRushStrange = 0;
        //float mXZAttenuation = 0;
        //float mVelocityY = 0;
        //Vector2 mRushVelocity = Vector2.zero;
        //Vector2 mRushDirection = Vector2.zero;
        //Vector2 mEventRushDirection = Vector2.zero;

        //void ProcessActionMove(ref Vector3 Distance, float dt)
        //{
        //    Vector2 vMoveXZ = Vector2.zero;

        //    if (mRushStrange > 0)
        //    {
        //        float XZDis = mRushStrange * dt - mXZAttenuation * dt * dt * 0.5f;
        //        mRushStrange -= mXZAttenuation * dt;
        //        mRushStrange = mRushStrange < 0 ? 0 : mRushStrange;

        //        vMoveXZ = mRushDirection * XZDis;

        //        if (mRushStrange == 0)
        //        {
        //            ResetRush();
        //        }


        //        Debug.Log("123");
        //    }

        //    Distance.x += vMoveXZ.x;
        //    Distance.z += vMoveXZ.y;

        //    //if (mIgnoreGravity || ignoreGravityGlobal)
        //    //{
        //    //    Distance.y += mVelocityY * dt;
        //    //}
        //    //else
        //    //{
        //    //    Distance.y += mVelocityY * dt - mGravity * dt * dt * 0.5f;
        //    //}
        //}

    }
}
