using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace GrygToolsUtils
{
	/// <summary>
	/// Extension methods for IEnumerable<T> to get random values from collections like List, Array, HashSet, and Dictionary.
	/// RandomValue extensions for List, Array and hashset should have no garbage collection overhead. RandomValue extensions for IEnumerable will have garbage collection overhead due to the need to iterate through the collection to find a random value.
	/// TryGetValue extensions all have garbage overheads
	/// </summary>
	public static class RandomIEnumerableExtensions
	{
		private static Random r = new Random();
#region IEnumerable Random Value Extension Methods. Includes List, Array, HashSet, and IEnumerable.
		/// <summary>
		/// Returns random value from a List<T>. If the list is empty, returns default(T) and logs an error.
		/// </summary>
		/// <param name="sourceList"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
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
			
			int index = r.Next(sourceList.Count);
			return sourceList[index];
		}
		
		/// <summary>
		/// Returns a random value from a List<T> that matches the given predicate. If no matching values are found, returns default(T). Return false if List is null, empty or no values match predicate
		/// </summary>
		/// <param name="sourceList"></param>
		/// <param name="predicate">Predicate items much match to be considered. If null all values are considered valid</param>
		/// <param name="value"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static bool TryGetRandomValue<T>(this List<T> sourceList, Predicate<T> predicate, out T value)
		{
			if (sourceList == null)
			{
				throw new InvalidOperationException("sourceList is null.");
			}
			
			if(sourceList.Count == 0)
			{
				Debug.LogError($"Source Array is empty. Returning default value for type {typeof(T)}.");
				value = default(T);
				return false;
			}

			if (predicate == null)
			{
				value = sourceList.RandomValue();
				return true;
			}
			
			var validValues = sourceList.FindAll(predicate);
			if(validValues.Count == 0)
			{
				Debug.LogError($"No valid values found in sourceList for type {typeof(T)}.");
				value = default(T);
				return false;
			}
			
			value = validValues[r.Next(validValues.Count)];
			return true;
		}
		
		/// <summary>
		/// Returns random value from an Array<T>. If the array is empty, returns default(T) and logs an error.
		/// </summary>
		/// <param name="sourceArray"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
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
			
			int index = r.Next(sourceArray.Length);
			return sourceArray[index];
		}
		
		/// <summary>
		/// Returns a random value from a Array<T> that matches the given predicate. If no matching values are found, returns default(T). Return false if array is null, empty or no values match predicate
		/// </summary>
		/// <param name="sourceArray"></param>
		/// <param name="predicate">Predicate items much match to be considered. If null all values are considered valid</param>
		/// <param name="value"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static bool TryGetRandomValue<T>(this T[] sourceArray, Predicate<T> predicate, out T value)
		{
			if (sourceArray == null)
			{
				throw new InvalidOperationException("sourceArray is null");
			}
			
			if(sourceArray.Length == 0)
			{
				Debug.LogError($"Source Array is empty. Returning default value for type {typeof(T)}.");
				value = default(T);
				return false;
			}
			
			if (predicate == null)
			{
				value = sourceArray.RandomValue();
				return true;
			}
			
			var validValues = Array.FindAll(sourceArray, predicate);
			if(validValues.Length == 0)
			{
				Debug.LogError($"No valid values found in sourceArray for type {typeof(T)}.");
				value = default(T);
				return false;
			}
			
			int index = r.Next(validValues.Length);
			value = validValues[index];
			return true;
		}
		
		/// <summary>
		/// Returns random value from a HashSet<T>. If the hashset is empty, returns default(T) and logs an error.
		/// </summary>
		/// <param name="sourceHashSet"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
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
			
			int index = r.Next(sourceHashSet.Count);
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
		
		/// <summary>
		/// Returns random value from a IEnumerable<T>. If the IEnumberable is empty, returns default(T) and logs an error.
		/// Has garbage collection overhead due to the need to iterate through the collection to find a random value. Avoid in hotpaths or change to a List<T> or Array<T> if possible.
		/// </summary>
		/// <param name="sourceEnumerable"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
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
			
			int index = r.Next(count);
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
		
		/// <summary>
		/// Returns a random value from a IEnumerable<T> that matches the given predicate. If no matching values are found, returns default(T). Return false if IEnumerable is null, empty or no values match predicate
		/// This method has garbage collection overhead due to the need to iterate through the collection to find a random value. Avoid in hotpaths or change to a List<T> or Array<T> if possible.
		/// </summary>
		/// <param name="sourceEnumerable"></param>
		/// <param name="predicate"></param>
		/// <param name="value"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static bool TryGetRandomValue<T>(this IEnumerable<T> sourceEnumerable, Predicate<T> predicate, out T value)
		{
			if (sourceEnumerable == null)
			{
				throw new InvalidOperationException("sourceArray is null");
			}
			
			var validList = new List<T>();
			foreach (var item in sourceEnumerable)
			{
				if(predicate == null || predicate(item))
				{
					validList.Add((T)item);
				}
			}

			if (validList.Count == 0)
			{
				Debug.LogError($"Source Enumerable is empty. Returning default value for type {typeof(T)}.");
				value = default(T);
				return false;
			}
			value = validList[r.Next(validList.Count)];
			return true;
		}
#endregion
		
#region Dictionary Random extensions
		/// <summary>
		/// Returns random value from a Dictionary<TKey, TValue>. If the dictionary is empty, returns default(TValue) and logs an error.
		/// </summary>
		/// <param name="sourceDictionary"></param>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
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
			
			int index = r.Next(sourceDictionary.Count);
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
		
		/// /// <summary>
		/// Returns random key from a Dictionary<TKey, TValue>. If the dictionary is empty, returns default(TKey) and logs an error.
		/// </summary>
		/// <param name="sourceDictionary"></param>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
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
			
			int index = r.Next(sourceDictionary.Count);
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

		/// <summary>
		/// Get a random value from a Dictionary<TKey, TValue> that matches the given predicate. If no matching values are found, returns default(TValue). Return false if Dictionary is null, empty or no items match predicate
		/// </summary>
		/// <param name="sourceDictionary"></param>
		/// <param name="valuePredicate">Predicate to determine validity of values</param>
		/// <param name="value"></param>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <returns></returns>
		public static bool TryGetRandomValue<TKey, TValue>(this Dictionary<TKey, TValue> sourceDictionary, Predicate<TValue> valuePredicate, out TValue value)
		{
			return TryGetRandomValue(sourceDictionary, null, valuePredicate, out value);
		}
		
		/// <summary>
		/// Get a random value from a Dictionary<TKey, TValue> that matches the given predicate. If no matching values are found, returns default(TValue). Return false if Dictionary is null, empty or no items match predicate
		/// </summary>
		/// <param name="sourceDictionary"></param>
		/// <param name="keyPredicate">Predicate to determine validity of keys</param>
		/// <param name="valuePredicate">Predicate to determine validity of values</param>
		/// <param name="value"></param>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <returns></returns>
		public static bool TryGetRandomValue<TKey, TValue>(this Dictionary<TKey, TValue> sourceDictionary, Predicate<TKey> keyPredicate, Predicate<TValue> valuePredicate, out TValue value)
		{
			if (sourceDictionary == null || sourceDictionary.Count == 0)
			{
				value = default(TValue);
				return false;
			}
			
			var validList = new List<TValue>();
			foreach (var kvp in sourceDictionary)
			{
				if((keyPredicate == null || keyPredicate(kvp.Key)) && (valuePredicate == null || valuePredicate(kvp.Value)))
				{
					validList.Add(kvp.Value);
				}
			}

			if (validList.Count == 0)
			{
				Debug.LogError($"Source Dictionary is empty. Returning default value for type {typeof(TValue)}.");
				value = default(TValue);
				return false;
			}
			value = validList[r.Next(validList.Count)];
			return true;
		}
		
		/// <summary>
		/// Get a random key from a Dictionary<TKey, TValue> that matches the given predicate. If no matching keys are found, returns default(TValue). Return false if Dictionary is null, empty or no items match predicate
		/// </summary>
		/// <param name="sourceDictionary"></param>
		/// <param name="valuePredicate">Predicate to determine validity of values</param>
		/// <param name="value"></param>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <returns></returns>
		public static bool TryGetRandomKey<TKey, TValue>(this Dictionary<TKey, TValue> sourceDictionary, Predicate<TKey> keyPredicate, out TKey value)
		{
			return TryGetRandomKey(sourceDictionary, keyPredicate, null, out value);
		}
		
		/// <summary>
		/// Get a random key from a Dictionary<TKey, TValue> that matches the given predicate. If no matching keys are found, returns default(TValue). Return false if Dictionary is null, empty or no items match predicate
		/// </summary>
		/// <param name="sourceDictionary"></param>
		/// <param name="keyPredicate">Predicate to determine validity of keys</param>
		/// <param name="valuePredicate">Predicate to determine validity of values</param>
		/// <param name="value"></param>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <returns></returns>
		public static bool TryGetRandomKey<TKey, TValue>(this Dictionary<TKey, TValue> sourceDictionary, Predicate<TKey> keyPredicate, Predicate<TValue> valuePredicate, out TKey value)
		{
			if (sourceDictionary == null || sourceDictionary.Count == 0)
			{
				value = default(TKey);
				return false;
			}
			
			var validList = new List<TKey>();
			foreach (var kvp in sourceDictionary)
			{
				if((keyPredicate == null || keyPredicate(kvp.Key)) && (valuePredicate == null || valuePredicate(kvp.Value)))
				{
					validList.Add(kvp.Key);
				}
			}

			if (validList.Count == 0)
			{
				Debug.LogError($"Source Dictionary is empty. Returning default value for type {typeof(TKey)}.");
				value = default(TKey);
				return false;
			}
			value = validList[r.Next(validList.Count)];
			return true;
		}
#endregion

#region Shuffle Extensions
		public static void Shuffle<T>(this List<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = r.Next(n + 1);
				(list[k], list[n]) = (list[n], list[k]);
			}
		}
		
		public static void Shuffle<T>(this T[] array)
		{
			int n = array.Length;
			while (n > 1)
			{
				n--;
				int k = r.Next(n + 1);
				(array[k], array[n]) = (array[n], array[k]);
			}
		}
		
#endregion
	}
}
