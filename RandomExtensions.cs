using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace GrygToolsUtils
{
	public static class RandomExtensions
	{
#region IEnumerable Random Value Extension Methods. Includes List, Array, HashSet, and IEnumerable.
		public static T RandomValue<T>(this List<T> sourceList)
		{
			if (sourceList == null)
			{
				throw new InvalidOperationException("sourceList is null.");
			}
			
			if(sourceList.Count == 0)
			{
				Debug.LogError($"Source Array is empty. Returning default value for type {typeof(T)}.");
				return default(T);
			}
			
			Random random = new Random();
			int index = random.Next(sourceList.Count);
			return sourceList[index];
		}
		
		public static T RandomValue<T>(this T[] sourceArray)
		{
			if (sourceArray == null)
			{
				throw new InvalidOperationException("sourceArray is null");
			}
			
			if(sourceArray.Length == 0)
			{
				Debug.LogError($"Source Array is empty. Returning default value for type {typeof(T)}.");
				return default(T);
			}
			
			Random random = new Random();
			int index = random.Next(sourceArray.Length);
			return sourceArray[index];
		}
		
		public static T RandomValue<T>(this HashSet<T> sourceHashSet)
		{
			if (sourceHashSet == null)
			{
				throw new InvalidOperationException("sourceHashSet is null.");
			}
			
			if(sourceHashSet.Count == 0)
			{
				Debug.LogError($"Source HashSet is empty. Returning default value for type {typeof(T)}.");
				return default(T);
			}
			
			Random random = new Random();
			int index = random.Next(sourceHashSet.Count);
			int currentIndex = 0;
			foreach (var item in sourceHashSet)
			{
				if (currentIndex == index)
				{
					return item;
				}
				currentIndex++;
			}
			throw new InvalidOperationException("Source HashSet is empty.");
		}

		public static T RandomValue<T>(this IEnumerable<T> sourceEnumerable)
		{
			if (sourceEnumerable == null)
			{
				throw new InvalidOperationException("Source enumerable is null.");
			}
			
			int count = 0;
			foreach (var item in sourceEnumerable)
			{
				count++;
			}

			if (count == 0)
			{
				Debug.LogError($"Source Enumerable is empty. Returning default value for type {typeof(T)}.");
				return default(T);
			}
			Random random = new Random();
			int index = random.Next(count);
			int currentIndex = 0;
			foreach (var item in sourceEnumerable)
			{
				if (currentIndex == index)
				{
					return item;
				}
				currentIndex++;
			}
			throw new InvalidOperationException("Source enumerable is empty.");
		}
#endregion
		
#region Dictionary Random extensions
		public static TValue RandomValue<TKey, TValue>(this Dictionary<TKey, TValue> sourceDictionary)
		{
			if (sourceDictionary == null)
			{
				throw new InvalidOperationException($"Source dictionary is null.");
			}
			
			if(sourceDictionary.Count == 0)
			{
				Debug.LogError($"Source Dictionary is empty. Returning default value for type {typeof(TValue)}.");
				return default(TValue);
			}
			
			Random random = new Random();
			int index = random.Next(sourceDictionary.Count);
			int currentIndex = 0;
			foreach (var kvp in sourceDictionary)
			{
				if (currentIndex == index)
				{
					return kvp.Value;
				}
				currentIndex++;
			}
			throw new InvalidOperationException("Source enumerable is empty.");
		}
		
		public static TKey RandomKey<TKey, TValue>(this Dictionary<TKey, TValue> sourceDictionary)
		{
			if (sourceDictionary == null)
			{
				throw new InvalidOperationException($"Source dictionary is null.");
			}
			
			if(sourceDictionary.Count == 0)
			{
				Debug.LogError($"Source Dictionary is empty. Returning default value for type {typeof(TValue)}.");
				return default(TKey);
			}
			
			Random random = new Random();
			int index = random.Next(sourceDictionary.Count);
			int currentIndex = 0;
			foreach (var kvp in sourceDictionary)
			{
				if (currentIndex == index)
				{
					return kvp.Key;
				}
				currentIndex++;
			}
			throw new InvalidOperationException("Source enumerable is empty.");
		}
#endregion
	}
}
