using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private GameObject       SourceObject = null;
    private List<GameObject> Pool = new List<GameObject>();
    
    public ObjectPool(GameObject pMasterObject)
    {
        SourceObject = pMasterObject;
    }
    
    public GameObject GetPooledObject(Vector3 pPosition, Quaternion pRotation)
    {
        GameObject objectToReturn     = null;
        bool       unusedObjectExists = false;
        foreach (GameObject g in Pool)
        {
            if (!g.activeInHierarchy)
            {
                objectToReturn     = g;
                unusedObjectExists = true;
                break;
            }
        }

        if (unusedObjectExists)
        {
            objectToReturn.transform.SetPositionAndRotation(pPosition, pRotation);
            objectToReturn.SetActive(true);
            return objectToReturn;
        }
        objectToReturn = GameObject.Instantiate(SourceObject, pPosition, pRotation);
        Pool.Add(objectToReturn);
        return objectToReturn;
    }

    public void GrowPool(int pCount)
    {
        for(int i = 0; i < pCount; i++)
        {
            GameObject newObject = GameObject.Instantiate(SourceObject);
            newObject.SetActive(false);
            Pool.Add(newObject);
        }
    }
}
