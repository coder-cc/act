using System.Collections.Generic;
using UnityEngine;

namespace Aqua.Pool
{

    /// <summary>
    /// 池管理
    /// </summary>

    public class PoolManager
    {

        public static readonly PoolManager Instance = new PoolManager();

        Dictionary<string, SpawnPool> _spawnPools = new Dictionary<string, SpawnPool>();


        public SpawnPool this[string key]
        {
            get
            {
                SpawnPool pool = null;
                _spawnPools.TryGetValue(key, out pool);
                return pool;
            }
        }


        public SpawnPool Create(string name)
        {
            var owner = new GameObject(name + "[Pool]");
            return owner.AddComponent<SpawnPool>();
        }


        public void DestroyAll()
        {
            var iter = _spawnPools.GetEnumerator();
            while (iter.MoveNext())
            {
                Object.Destroy(iter.Current.Value);
            }
        }


        public void Destroy(string poolName)
        {
            SpawnPool spawnPool = null;
            if (_spawnPools.TryGetValue(poolName, out spawnPool))
            {
                _spawnPools.Remove(poolName);
                Object.Destroy(spawnPool);
            }
        }


        public bool Contains(string poolName)
        {
            return _spawnPools.ContainsKey(poolName);
        }


        internal void Add(SpawnPool pool)
        {

            if (_spawnPools.ContainsKey(pool.PoolName))
            {
                Debug.LogError(string.Format("池名称以被使用 [{0}]", pool.PoolName));

                return;
            }
            
            _spawnPools.Add(pool.PoolName, pool);
        }


        internal void Remove(SpawnPool pool)
        {
            _spawnPools.Remove(pool.PoolName);
        }


    }


}