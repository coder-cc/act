using UnityEngine;
using System.Collections;
using Aqua.Pool.Interface;
using System;
using System.Collections.Generic;

namespace Aqua.Pool
{



    public class SpawnPool : MonoBehaviour 
    {


        private string _poolPoolName;
        private Transform _transform;
        private GameObject _gameObject;
  
        List<PrefabPool> _prefabPools = new List<PrefabPool>(); 


        #region IPool


        public string PoolName
        {
            get
            {
                return _poolPoolName;
            }
        }



        public Transform Group
        {
            get
            {
                return _transform;
            }
            set
            {
                if (_transform != null)
                    _transform.name = PoolName;
            }
        }


        #endregion



        #region Mono Call


        void Awake()
        {
            _transform = transform;
            _gameObject = gameObject;

            _poolPoolName = _gameObject.name.Replace("[Pool]", string.Empty);

            PoolManager.Instance.Add(this);
        }


        void OnDestroy()
        {
            
            this.StopAllCoroutines();

            for (int i = 0; i < _prefabPools.Count; i++)
            {
                _prefabPools[i].ClearAll();
            }
            _prefabPools.Clear();

            PoolManager.Instance.Remove(this);
        }


        #endregion



        #region Spawn


        public Transform Spawn(Transform prefab)
        {
            return Spawn(prefab, Vector3.zero, Quaternion.identity, null);
        }


        public Transform Spawn(Transform prefab, Transform parent )
        {
            return Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
        }


        private Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, Transform parent)
        {

            PrefabPool pool = null;

            for (int i = 0; i < _prefabPools.Count; i++)
            {
                pool = _prefabPools[i];
                if (pool.Prefab == prefab)
                {
                    break;
                }
            }


            if (pool == null)
            {
                pool = new PrefabPool(prefab);
                pool.PoolOwner = this;
                _prefabPools.Add(pool);
            }


            Transform inst = pool.SpawnInstance(pos, rot);
            if (parent != null && inst.parent != parent)
            {
                inst.parent = parent;
                inst.gameObject.layer = parent.gameObject.layer;
            }

            return inst;
        }


        public void Despawn(Transform inst)
        {
            PrefabPool prefabPool = null;

            for (int i = 0; i < _prefabPools.Count; i++)
            {
                prefabPool = _prefabPools[i];

                if (prefabPool.Contain(inst))
                {
                    break;
                }
            }


            if (prefabPool != null)
            {
                prefabPool.DespawnInstance(inst);
            }
        }


        public void Despawn(Transform inst, float delaySecond)
        {
            StartCoroutine(DelayDespawn(inst, delaySecond));
        }


        private IEnumerator DelayDespawn(Transform inst, float delaySecond)
        {
            GameObject obj = inst.gameObject;
            while (delaySecond > 0)
            {
                yield return null;

                // If the instance was deactivated while waiting here, just quit
                if (!obj.activeInHierarchy)
                    yield break;

                delaySecond -= Time.deltaTime;
            }

            Despawn(inst);
        }


        #endregion

    }


}

