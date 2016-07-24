using UnityEngine;
using System.Collections;



namespace Core.Unit
{


    public class ExternForce
    {

        public float XForce { get; private set; }


        public float YForce { get; private set; }


        public float ZForce { get; private set; }


        public Transform OwnerTransform { get; set; }

        private float _time;
        private float _passTime;
        private float _srcX, _srcY, _srcZ;
        //private float _curX, _curY, _curZ;

        public ExternForce(Transform trans)
        {
            OwnerTransform = trans;
        }


        public void SetForce(float x, float y, float z)
        {
            XForce = x;
            YForce = y;
            ZForce = z;

            _srcX = x;
            _srcY = y;
            _srcZ = z;
        }


        public Vector3 Sampling(float delta)
        {

            if (OwnerTransform == null || (YForce.Equals(0f) && XForce.Equals(0f) && ZForce.Equals(0f)))
            {
                return Vector3.zero;
            }

            Vector3 result;

            result.x = delta* _srcX;
            result.y = delta* _srcY;
            result.z = delta* _srcZ;

            XForce -= result.x;
            YForce -= result.y;
            ZForce -= result.z;
            result = OwnerTransform.TransformDirection(result);
            //result = Quaternion.Euler(0, _ownerTransform.localRotation.y, 0) * result;

            //_passTime += delta;

            //if (_passTime > _time)
            //    return Vector3.zero;

            //float factor = _passTime/_time;
            //_curX = Mathf.Lerp(0, XForce, factor);
            //_curY = Mathf.Lerp(0, YForce, factor);
            //_curZ = Mathf.Lerp(0, ZForce, factor);

            //Vector3 result = new Vector3(_curX - _befX, _curY - _befY, _curZ - _befZ);

            //_befX = _curX;
            //_befY = _curY;
            //_befZ = _curZ;

            //result = Quaternion.Euler(0, _ownerTransform.localRotation.y, 0) * result;

            return result;
        }


    }


}

