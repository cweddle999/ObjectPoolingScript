using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPulling
{
    private static Dictionary<GameObject, Pool> _pools;
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (_pools == null)
        {
            _pools = new Dictionary<GameObject, Pool>();
        }

        if (_pools.ContainsKey(prefab) == false)
        {
            _pools[prefab] = new Pool(prefab);
        }

        return _pools[prefab].Spawn(position, rotation);
    }

    public static void Despawn(GameObject objectToRemove)
    {
        if (objectToRemove.GetComponent<PoolMember>() == null)
        {
            GameObject.Destroy(objectToRemove);
        }
        else
        {
            objectToRemove.GetComponent<PoolMember>().MyPool.Despawn(objectToRemove);
        }
    }
    
    private class Pool
    {
        private int _currentIndex = 0;
        private Stack<GameObject> _inactiveObjects;
        private GameObject _prefab;

        public Pool(GameObject prefab)
        {
            _prefab = prefab;
            _inactiveObjects = new Stack<GameObject>();
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            GameObject obj;

            if(_inactiveObjects.Count == 0)
            {
                obj = GameObject.Instantiate(_prefab, position, rotation);
                obj.name = _prefab.name + "_" + _currentIndex;
                _currentIndex++;
                //TODOO: make pool member and set this to be a pool member
                obj.AddComponent<PoolMember>().MyPool = this;

            }
            else
            {
                obj = _inactiveObjects.Pop();

                if(obj == null) 
                { 
                    return Spawn(position, rotation);
                }
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;

        }

        public void Despawn(GameObject objectToRemove)
        {
            objectToRemove.SetActive(false);
            _inactiveObjects.Push(objectToRemove);
        }
    }

    private class PoolMember : MonoBehaviour
    {
        public Pool MyPool { get; set; }
    }

}
