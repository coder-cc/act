using UnityEngine;
using System.Collections;

namespace Aqua.Action.Event
{

    public enum EffectBindingType
    {
        /// <summary>
        /// 无绑定
        /// </summary>

        None,

        /// <summary>
        /// 绑定到主体
        /// </summary>

        BindingOnwer,

        /// <summary>
        /// 绑定到骨骼
        /// </summary>

        BindingSkeleton,
    }


    public enum EffectLifeType
    {
        /// <summary>
        /// 跟随动作结束
        /// </summary>

        FollowAction,

        /// <summary>
        /// 宿主受击
        /// </summary>

        OwnerHit,

        /// <summary>
        /// 动作中断
        /// </summary>

        InterruptAction,
    }


    /// <summary>
    /// 特效数据类型
    /// </summary>

    public class EffectEventArgs : ActionEventArgs
    {
        public string ResourcePath { get; set; }


        public string BindingSkeletonName { get; set; }


        public EffectBindingType BindingType { get; set; }


        public EffectLifeType LifeType { get; set; }



    }

}


