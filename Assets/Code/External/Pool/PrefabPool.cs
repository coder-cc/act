using System.Collections.Generic;
using Aqua.Pool.Interface;
using UnityEngine;

namespace Aqua.Pool
{
    public class PrefabPool
    {

        private Transform _prefab;
        private GameObject _prefabObject;

        private List<Transform> _spawned = new List<Transform>();
        private List<Transform> _activitySpawned = new List<Transform>();


        public SpawnPool PoolOwner
        {
            get; internal set;
        }


        public int TotalCount
        {
            get { return _spawned.Count + _activitySpawned.Count; }
        }


        public GameObject Prefab
        {
            get { return _prefabObject;}
        }


        public PrefabPool(Transform prefab)
        {
            _prefab = prefab;
            _prefabObject = prefab.gameObject;
        }



        public bool Contain(Transform inst)
        {
            for (int i = 0; i < _activitySpawned.Count; i++)
            {
                if (_activitySpawned[i] == inst)
                    return true;
            }

            for (int i = 0; i < _spawned.Count; i++)
            {
                if (_spawned[i] == inst)
                    return true;
            }

            return false;
        }


        public Transform SpawnInstance()
        {
            return SpawnInstance(Vector3.zero, Quaternion.identity);
        }


        public Transform SpawnInstance(Vector3 pos, Quaternion rot)
        {

            if (_spawned.Count > 0)
            {
                Transform first = _spawned[0];

                UseInstance(first);

                return first;
            }
            else
            {
                Transform inst;
                inst = SpawnNew(pos, rot);
                UseInstance(inst);
                return inst;
            }
          
        }


        public void PreloadInstance( int count )
        {

            if (_prefabObject == null)
            {
                Debug.LogError(string.Format(" prefab is null!"));
            }

            for (int i = 0; i < count; i++)
            {
                Transform inst = SpawnNew();
                SetActive(inst, false);
            }
            
        }


        private void UseInstance(Transform inst)
        {
            _activitySpawned.Add(inst);
            _spawned.Remove(inst);

            SetActive(inst, true);
        }


        public void DespawnInstance(Transform inst)
        {

            if (_spawned.Contains(inst) == false)
                _spawned.Add(inst);
            _activitySpawned.Remove(inst);

            //   标记为最后一个元素
            SetNameFlag(inst);

            SetActive(inst, false);
        }


        public Transform SpawnNew()
        {
            return SpawnNew(Vector3.zero, Quaternion.identity);
        }


        public Transform SpawnNew( Vector3 pos, Quaternion rot )
        {
            var inst = ((GameObject) Object.Instantiate(_prefabObject, pos, rot)).transform;

            _spawned.Add(inst);
            SetNameFlag(inst);

            if (PoolOwner != null)
            {
                inst.parent = PoolOwner.Group;
                inst.gameObject.layer = PoolOwner.Group.gameObject.layer;
            }

            return inst;
        }


        public void ClearAll()
        {
            for (int i = 0; i < _spawned.Count; i++)
            {
                Object.Destroy(_spawned[i]);
            }

            for (int i = 0; i < _activitySpawned.Count; i++)
            {
                Object.Destroy(_activitySpawned[i]);
            }

            PoolOwner = null;

            _prefab = null;
            _prefabObject = null;

            _spawned.Clear();
            _activitySpawned.Clear();
        }


        private void SetNameFlag(Transform instance)
        {
            instance.name = Prefab.name + (TotalCount + 1).ToString("#000");
        }


        private void SetActive(Transform inst, bool active)
        {
            GameObject obj = inst.gameObject;
            if (obj.activeSelf != active)
                obj.SetActive(active);
        }

    }
}