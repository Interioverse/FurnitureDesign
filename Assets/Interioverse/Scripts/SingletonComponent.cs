using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interioverse
{
    public class SingletonComponent<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;
        private static readonly object _instanceLock = new object();
        protected static bool _quitting = false;

        public static T Instance
        {
            get
            {
                if (_instance == null && !_quitting)
                {
                    lock (_instanceLock)
                    {
                        _instance = GameObject.FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            GameObject go = new GameObject(typeof(T).ToString());
                            _instance = go.AddComponent<T>();

                            DontDestroyOnLoad(_instance.gameObject);
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null) _instance = gameObject.GetComponent<T>();
            else if (_instance.GetInstanceID() != GetInstanceID())
            {
                Destroy(gameObject);
                throw new System.Exception(string.Format("Instance of {0} already exists, removing {1}", GetType().FullName, ToString()));
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _quitting = true;
        }
    }
}