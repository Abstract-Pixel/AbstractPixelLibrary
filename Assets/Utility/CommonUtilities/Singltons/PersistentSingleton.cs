using UnityEngine;

namespace AbstractPixel.Utility
{
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        public bool AutoUnparentOnAwake = true;

        protected static T instance;
        protected static bool isApplicationQuitting = false;
        public static bool HasInstance => instance != null;
        public static T TryGetInstance() => HasInstance ? instance : null;

        public static T Instance
        {
            get
            {
                if (instance == null && !isApplicationQuitting)
                {
                    instance = FindAnyObjectByType<T>();
                    if (instance == null)
                    {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        instance = go.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
            if (!Application.isPlaying || isApplicationQuitting) return;
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            isApplicationQuitting = false;
            if (AutoUnparentOnAwake)
            {
                transform.SetParent(null);
            }

            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void OnDestroy()
        {
            instance = null;
            isApplicationQuitting = false;
        }

        private void OnApplicationQuit()
        {
            isApplicationQuitting = true;
        }
    }
}