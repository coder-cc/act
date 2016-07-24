using CSTools;
using UnityEngine;
using System.Collections;


namespace Core.Controller
{


    public interface ICameraController
    {
        Transform OwnerTransform { get; }

        Vector2 CameraOffset { get; }
    }


    public class CameraController : Singleton<CameraController>
    {
        public static Camera mainCam;

        public bool isPlaying { get { return mIsPlaying; } }

        public Vector3 CameraOffset
        {
            get { return mCameraOffset; }
            set { mCameraOffset = value; }
        }


        public Transform targetTrans;
        protected Vector3 CameraPos = new Vector3(0, 9, -7);
        protected Vector3 CacheCameraPos = new Vector3(0, 9, -7);


        private bool mIsPlaying;
        private Vector3 mUnitCachePosition;
        private Vector3 mUnitCurrentVelocity;

        private float CameraView = 45f;
        private Vector3 CameraLookAtOffset = Vector3.up;

        private Vector3 mCameraPosCache = Vector3.zero;
        private Vector3 mCameraOffset = Vector3.zero;
        private Vector3 mCameraShakeOffset = Vector3.zero;


        public void SetCameraPos(Vector3 vPos, bool bForceUpdate)
        {
            if (bForceUpdate)
                CameraPos = vPos;

            CacheCameraPos = vPos;
        }


        public void Init(bool bPlaying)
        {
            mainCam = Camera.main;

            mainCam.orthographic = false;
            mainCam.nearClipPlane = 0.1f;
            mainCam.farClipPlane = 10000;
            mIsPlaying = bPlaying;
        }


        public void StartUnitCamera(ICameraController target, bool show = true)
        {
            targetTrans = target.OwnerTransform;
            mIsPlaying = true;
            ToggleCamera(show);
            UpdatePositionNow(targetTrans.position);
        }


        public void ToggleCamera(bool toggle)
        {
            if (mainCam != null)
                mainCam.enabled = toggle;
        }


        public void Stop()
        {
            mIsPlaying = false;
            mainCam.backgroundColor = Color.black;
        }


        public void Dispose()
        {
            Stop();
            ToggleCamera(false);
        }


        public void UpdatePositionNow(Vector3 wpos)
        {
            if (mIsPlaying)
            {
                UpdateCamPos(wpos);
            }
        }


        public void Update()
        {
            if (mIsPlaying)
            {
                //bool bForceUpdate = false;

                if (targetTrans != null && CameraView != null)
                {
                    Vector3 pos = targetTrans.position;

                    if (NearlyEqual(ref mUnitCachePosition, ref pos) == false )//|| bForceUpdate)
                    {
                        mUnitCachePosition = VectorSmoothSlerp(ref mUnitCachePosition, ref pos, Time.deltaTime, 8);
                    }

                    UpdateCamPos(mUnitCachePosition);
                }
            }
        }


        private bool NearlyEqual(ref Vector3 point1, ref Vector3 point2, float sqrValue = 0.00001f)
        {
            return (Vector3.SqrMagnitude(point1 - point2) < sqrValue);
        }


        private Vector3 VectorSmoothSlerp(ref Vector3 SrcPos, ref Vector3 DstPos, float DeltaTime, float PowParam)
        {
            return SrcPos + (DstPos - SrcPos) * (1 - Mathf.Pow(0.5f, PowParam * DeltaTime));
        }


        private void UpdateCamPos(Vector3 pos)
        {
            if (mainCam == null)
            {
                Init(true);
            }

            if (mCameraPosCache != CameraPos)
            {
                mCameraPosCache = CameraPos;
            }
            mainCam.fieldOfView = CameraView;

            //Vector3 oldCameraPos = mainCam.transform.position;

            if (NearlyEqual(ref CameraPos, ref CacheCameraPos, 0.001f) == false)
                CameraPos = VectorSmoothSlerp(ref CameraPos, ref CacheCameraPos, Time.deltaTime, 8);

            Vector3 vCameraOffset = CameraPos;

            //尽量不要使用lookat
            mainCam.transform.position = pos + vCameraOffset;
            mainCam.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(pos + CameraLookAtOffset - mainCam.transform.position));
            mainCam.transform.Translate(mCameraOffset + mCameraShakeOffset);
        }


        public void UpdatePosOffset(Transform t)
        {
            mainCam.transform.position = t.position;
            CameraPos = mainCam.transform.localPosition;
        }


        public void ResetPosOffset()
        {
            SetCameraPos(CameraPos, true);
            CameraView = 45f;
        }

    }

}


