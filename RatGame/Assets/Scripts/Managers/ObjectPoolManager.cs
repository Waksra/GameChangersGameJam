using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : StaticManager<ObjectPoolManager>
{
    private Dictionary<GameObject, ObjectPool> Pools;

    private void Awake()
    {
        Pools = new Dictionary<GameObject, ObjectPool>();
    }
    
    public static GameObject GetPooledObject(GameObject pPrefab, Vector3 pPosition, Quaternion pRotation)
    {
        if (!Instance.Pools.ContainsKey(pPrefab))
        {
            Instance.Pools[pPrefab] = new ObjectPool(pPrefab);
        }

        return Instance.Pools[pPrefab].GetPooledObject(pPosition, pRotation);
    }

    public static void GrowPool(GameObject pPrefab, int pCount)
    {
        if (!Instance.Pools.ContainsKey(pPrefab))
        {
            Instance.Pools[pPrefab] = new ObjectPool(pPrefab);
        }

        Instance.Pools[pPrefab].GrowPool(pCount);
    }
}
