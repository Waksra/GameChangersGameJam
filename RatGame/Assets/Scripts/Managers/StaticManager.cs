using UnityEngine;

public abstract class StaticManager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T actualInstance;

    public static T Instance
    {
        get
        {
            if (actualInstance != null)
                return actualInstance;
            actualInstance = FindObjectOfType<T>();

            if (actualInstance == null)
            {
                actualInstance = CreateManager();
            }
            return actualInstance;
        }
    }
    
    private static T CreateManager()
    {
        GameObject managerParent = new GameObject($"{typeof(T).Name}");
        T          instance      = managerParent.AddComponent<T>();
        DontDestroyOnLoad(managerParent);
        return instance;
    }
}
