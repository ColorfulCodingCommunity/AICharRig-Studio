using UnityEngine;
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogWarning(typeof(T).ToString() + " is missing.");
            }

            return _instance;
        }
    }



    public virtual void Awake()
    {
        _instance = this as T;
    }


    public virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}