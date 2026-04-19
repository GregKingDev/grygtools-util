using System;
using UnityEngine.PlayerLoop;

namespace GrygToolsUtils
{
	public abstract class SingletonBase<T> where T : class, ISingleton
	{
		private static readonly Lazy<T> PrivateInstance = new Lazy<T>(() => InstantiateT());
		
		public static T Instance { get { return PrivateInstance.Value; } }

		private static T InstantiateT()
		{
			T newInstance = Activator.CreateInstance(typeof(T), true) as T;
			newInstance.Init();
			return newInstance;
		}
	}

	public interface ISingleton
	{
		public void Init();
	}
}