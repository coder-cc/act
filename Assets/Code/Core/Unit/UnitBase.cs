using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Unit
{
    public class UnitBase
    {

        private Dictionary<string, Transform> _skeletonCache;


        private GameObject mCacheGameObject;

        public GameObject CacheGameOject
        {
            get { return mCacheGameObject; }
            protected set { mCacheGameObject = value; }
        }


        private Transform mCacheTransform;

        public Transform CacheTransform
        {
            get { return mCacheTransform; }
            protected set { mCacheTransform = value; }
        }


        private GameObject mModelGameObject;

        public GameObject ModelGameObject
        {
            get { return mModelGameObject; }
            private set { mModelGameObject = value; }
        }


        private UnitInfo mInfo;

        public UnitInfo Info
        {
            get { return mInfo; }
            protected set { mInfo = value; }
        }


        private Animation mAnimation;

        public Animation CacheAnimation
        {
            get { return mAnimation; }
        }


        private CharacterController _characterController;

        public CharacterController CharacterController
        {
            get { return _characterController;}
        }


        protected AnimationState mCachedAnimationState = null;

        public AnimationState CachedAnimationState
        {
            get { return mCachedAnimationState; }
            set { mCachedAnimationState = value; }
        }


        protected float mAnimSpeed = 1.0f;

        public float AnimSpeed
        {
            get { return mAnimSpeed; }
            set { mAnimSpeed = value; }
        }

        public bool IsLoadModel
        {
            get { return mModelGameObject != null; }
        }






        public UnitBase(UnitInfo info)
        {
            mInfo = info;
        }


        protected void SetModel(GameObject actor)
        {
            //ModelGameObject = model;

            CacheTransform = actor.transform;
            ModelGameObject = actor.transform.FindChild("Model").gameObject;

            //modelTransform.localPosition = Vector2.zero;
            //modelTransform.localScale = Vector3.one;
            //modelTransform.localRotation = Quaternion.identity;

            mAnimation = mModelGameObject.GetComponent<Animation>();
            _characterController = CacheTransform.GetComponent<CharacterController>();
        }


        public virtual void Init()
        {

        }


        public virtual void Update(float deltaTime)
        {

        }


        public Transform GetSkeleton(string name)
        {

            if (_skeletonCache.ContainsKey(name))
            {
                return _skeletonCache[name];
            }

            if (CacheTransform == null)
                return null;

            var trans = CacheTransform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < trans.Length; i++)
            {
                var node = trans[i];
                if (node.name == name)
                {
                    _skeletonCache.Add(name, node);
                    return node;
                }
            }
            _skeletonCache.Add(name, null);
            return null;
        }


        public virtual void PlayAnim(AnimationState animState, float BlendTime, bool bForceRender)
        {
            mCachedAnimationState = animState;
            AnimSpeed = animState.speed;
            float fadeLength = BlendTime * 0.001f;
            if (fadeLength == 0)
                CacheAnimation.Play(animState.name);
            else
                CacheAnimation.CrossFade(animState.name, fadeLength);

            ChangeForceSkinAnimation(bForceRender);
        }


        void ChangeForceSkinAnimation(bool bForce)
        {
            CacheAnimation.cullingType = bForce ? AnimationCullingType.AlwaysAnimate : AnimationCullingType.BasedOnRenderers;
        }


        protected void PhysicsMove(Vector3 motion)
        {
            _characterController.Move(motion);
        }
    }
}
