using UnityEngine;

namespace Utils
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;
        private static bool _isQuitting;

        public static T Instance
        {
            get
            {
                if (_isQuitting)
                    return null;

                if (instance == null)
                    instance = FindAnyObjectByType<T>();

                if (instance == null && Application.isPlaying)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }

                return instance;
            }
        }

        public virtual void Awake()
        {
            Init();
        }

        protected virtual void OnDestroy()
        {
            if (instance == this as T)
                instance = null;
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        private void Init()
        {
            if (!Application.isPlaying)
                return;

            if (instance == null)
                instance = this as T;
        }
    }
}
