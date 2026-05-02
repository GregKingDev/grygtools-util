using UnityEngine;

namespace GrygToolsUtils
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
                    GameObject temp = new GameObject("Temp");
                    Object.DontDestroyOnLoad(temp);
                    foreach (GameObject rootGameObject in temp.scene.GetRootGameObjects())
                    {
                        if (rootGameObject.name == "MBSingletons")
                        {
                            singletonRoot = rootGameObject;
                            Destroy(temp);
                            return rootGameObject;
                        }
                    }
                    Destroy(temp);
                     
                    singletonRoot = new GameObject("MBSingletons");
                    DontDestroyOnLoad(singletonRoot);
                }

                return singletonRoot;
            }
        }
    }
}