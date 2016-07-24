using System;
using System.Collections.Generic;
using System.IO;
using Aqua.Action.Event.Interface;
using Aqua.Pool;
using Aqua.Resource;
using Aqua.Util;
using Core.Unit;
using UnityEngine;

namespace Aqua.Action.Event
{
    public static class EventExecute
    {
        //static PoolManager _poolManager = new PoolManager();

        static Dictionary<string , PrefabPool> _prefabInst = new Dictionary<string, PrefabPool>();


        public static void ExecuteEvent(ActionStatus owner, ActionEventArgs args, int deltaTime)
        {
            if (args is EffectEventArgs)
            {
                ExecuteEffectEvent(owner, (EffectEventArgs)(args));
            }   
            else if (args is VelocityEventArgs)
            {
                ExecuteVelocity(owner, (VelocityEventArgs) args, deltaTime);
            }
        }



        private static void ExecuteVelocity(ActionStatus status, VelocityEventArgs args, int deltaTime)
        {
            status.SetVelocity(args.VelocityX*0.01f, args.VelocityY*0.01f, args.VelocityZ*0.01f, deltaTime);
        }


        private static void ExecuteEffectEvent(ActionStatus status, EffectEventArgs args)
        {

            Transform parent = null;

            switch (args.BindingType)
            {
                case EffectBindingType.BindingOnwer:
                    {
                        parent = status.Onwer.CacheTransform;
                    }
                    break;
                case EffectBindingType.BindingSkeleton:
                    {
                        parent = status.Onwer.GetSkeleton(args.BindingSkeletonName);
                    }
                    break;
            }

            GameObject inst = null;
            PrefabPool prefabPool;

            if (_prefabInst.TryGetValue(args.ResourcePath, out prefabPool))
            {
                inst = prefabPool.SpawnInstance().gameObject;
            }
            else
            {
                var prefab = (GameObject)ResourceCenter.LoadAsset(args.ResourcePath);
                prefabPool = new PrefabPool(prefab.transform);
                inst = prefabPool.SpawnInstance().gameObject;
                _prefabInst.Add(args.ResourcePath, prefabPool);
            }


            inst.transform.parent = parent;
            inst.transform.localPosition = new Vector3(args.PositionX * 0.01f, args.PositionY * 0.01f, args.PositionZ * 0.01f);
            inst.transform.localRotation = Quaternion.Euler(args.RotateX, args.RotateY, args.RotateZ);
          


            switch (args.LifeType)
            {
                case EffectLifeType.OwnerHit:
                    break;
                case EffectLifeType.InterruptAction:
                    break;
                case EffectLifeType.FollowAction:
                case EffectLifeType.TimeOut:
                    {
                        DelayDestroy delayDestroy = inst.GetComponent<DelayDestroy>();
                        if (delayDestroy == null)
                        {
                            delayDestroy = inst.AddComponent<DelayDestroy>();
                            delayDestroy.OnCompleta = OnRecycle;
                            
                        }
                        delayDestroy.ResourceName = args.ResourcePath;
                        delayDestroy.TimeMs = args.LifeTime;
                        delayDestroy.Start();
                    }
                    break;
            }


            inst.SetActive(true);
        }


        public static void OnRecycle(string resPath, GameObject inst)
        {
            PrefabPool prefabPool;
            if (_prefabInst.TryGetValue( resPath, out prefabPool))
            {
                prefabPool.DespawnInstance(inst.transform);
            }
        }

    }
}