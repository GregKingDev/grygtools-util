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
					singletonRoot = GameObject.Find("MBSingletons");
					if(singletonRoot == null)
					{
						singletonRoot = new GameObject("MBSingletons");
					}
					DontDestroyOnLoad(singletonRoot);
				}

				return singletonRoot;
			}
		}
	}
}