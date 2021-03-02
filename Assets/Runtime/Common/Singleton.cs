using UnityEngine;

namespace JingYe.Common
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T s_Instance;
        public static T Instance
        {
            get
            {
                if (s_ApplicationIsQuitting)
                    return null;

                if (s_Instance == null)
                {
                    s_Instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return s_Instance;
                    }

                    if (s_Instance == null)
                    {
                        GameObject singleton = new GameObject();
                        s_Instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);
                    }
                    else
                    {
                        DontDestroyOnLoad(s_Instance.gameObject);
                    }
                }

                return s_Instance;
            }
        }

        protected virtual void Awake()
        {
            _ = Instance;
        }

        protected virtual void OnDestroy()
        {
            s_ApplicationIsQuitting = true;
        }

        private static bool s_ApplicationIsQuitting = false;

    } // END class
} // END namespace
