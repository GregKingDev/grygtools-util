using UnityEngine;

namespace GrygTools
{
    public abstract class MbSingletonBase : MonoBehaviour
    {
        private static GameObject singletonRoot;

        protected static GameObject SingletonRoot
        {
            get
            {
                if (singletonRoot == null)
                {
                    singletonRoot = new GameObject("MBSingletons");
                }

                return singletonRoot;
            }
        }
    }
}