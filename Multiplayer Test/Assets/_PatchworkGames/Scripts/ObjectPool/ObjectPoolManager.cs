using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PatchworkGames.ObjectPoolManager;

namespace PatchworkGames
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

        private GameObject _objectPoolEmptyHolder;

        private static GameObject _gameObjectsEmpty;
        private static GameObject _particleSystemsEmpty;

        private static PoolType PoolingType;

        public enum PoolType
        {
            Gameobject,
            ParticleSystem,
            None
        }

        private void Awake()
        {
            SetupEmpties();
        }

        private void SetupEmpties()
        {
            _objectPoolEmptyHolder = new GameObject("Pooled Objects");
            _gameObjectsEmpty = new GameObject("GameObjects");
            _gameObjectsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

            _particleSystemsEmpty = new GameObject("Particle Effects");
            _particleSystemsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
        }
        public static void PrewarmObject(GameObject obj, int count, PoolType poolType = PoolType.None)
        {
            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == obj.name);
            Vector3 prewarmPosition = new Vector3(0, -20, 0);

            //if pool doesnt exist
            if (pool == null)
            {
                pool = new PooledObjectInfo() { LookupString = obj.name };
                ObjectPools.Add(pool);
            }

            GameObject parentObject = SetParentObject(poolType);

            if (pool.InactiveObjects.Count >= count) return;

            for (int i = 0; i < count; i++)
            {
                GameObject spawnableObj = Instantiate(obj, prewarmPosition, Quaternion.identity);

                if (parentObject != null)
                {
                    spawnableObj.transform.SetParent(parentObject.transform);
                }

                ReturnObjectToPool(spawnableObj);
            }
        }

        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
        {
            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

            //if pool doesnt exist
            if (pool == null)
            {
                pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
                ObjectPools.Add(pool);
            }

            //check if inactive objects in pool
            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

            if (spawnableObj == null) //if not aleady in a pool
            {
                GameObject parentObject = SetParentObject(poolType);

                spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

                if (parentObject != null)
                {
                    spawnableObj.transform.SetParent(parentObject.transform);
                }
            }
            else //grab from pool
            {
                spawnableObj.transform.position = spawnPosition;
                spawnableObj.transform.rotation = spawnRotation;
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }

            return spawnableObj;
        }
        public static GameObject SpawnObject(GameObject objectToSpawn, Transform parentTransform)
        {
            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

            //if pool doesnt exist
            if (pool == null)
            {
                pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
                ObjectPools.Add(pool);
            }

            //check if inactive objects in pool
            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

            if (spawnableObj == null) //if not aleady in a pool
            {
                spawnableObj = Instantiate(objectToSpawn, parentTransform);
            }
            else //grab from pool
            {
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }

            return spawnableObj;
        }

        public static void ReturnObjectToPool(GameObject obj)
        {
            //remove "(Clone)" from object name
            string goName = obj.name.Substring(0, obj.name.Length - 7);

            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);

            if (pool == null)
            {
                Debug.LogWarning("Trying to release an object that is not pooled: " + obj.name);
            }
            else
            {
                obj.SetActive(false);
                pool.InactiveObjects.Add(obj);
            }
        }

        private static GameObject SetParentObject(PoolType poolType)
        {
            switch (poolType)
            {
                case PoolType.Gameobject:
                    return _gameObjectsEmpty;

                case PoolType.ParticleSystem:
                    return _particleSystemsEmpty;

                case PoolType.None:
                    return null;

                default:
                    return null;
            }
        }
    }
    public class PooledObjectInfo
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new List<GameObject>();
    }
}

