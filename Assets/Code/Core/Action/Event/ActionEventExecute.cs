using System;
using Aqua.Action.Event.Interface;
using UnityEngine;

namespace Aqua.Action.Event
{
    public static class ActionEventExecute
    {

        public static void ExecuteEvent(IEventOwner owner, ActionEventArgs args)
        {
            if (args is EffectEventArgs)
            {
                ExecuteEffectEvent(owner, (EffectEventArgs)(args));
            }   
        }



        private static void ExecuteEffectEvent(IEventOwner owner, EffectEventArgs args)
        {

            Transform parent = null;

            switch (args.BindingType)
            {
                case EffectBindingType.BindingOnwer:
                    {
                        parent = owner.ActionUnitOnwer.CacheTransform;
                    }
                    break;
                case EffectBindingType.BindingSkeleton:
                    {
                        parent = owner.ActionUnitOnwer.GetSkeletonPointByName(args.BindingSkeletonName);
                    }
                    break;
            }




        }
    }
}