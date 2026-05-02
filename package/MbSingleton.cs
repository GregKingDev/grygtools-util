using UnityEngine;
namespace GrygToolsUtils
{
    [DisallowMultipleComponent]
    public abstract class MbSingleton<T> : MbSingletonBase where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = SingletonRoot.GetComponent<T>();

                    if (instance == null)
                    {
                        instance = SingletonRoot.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        protected static T instance;

        protected virtual void Init()
        {
			
        }
		
        private void Awake()
        {
            if (instance == null)
            {
                instance = this.gameObject.GetComponent<T>();
                Init();
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
    }
}