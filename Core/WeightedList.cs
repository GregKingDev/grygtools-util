using GrygTools.Utils.Attributes;
using GrygTools.Utils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GrygTools.Utils.Core
{
	[Serializable]
	public class WeightedListEntry<T>
	{
		[HideInInspector]
		public string name = "Weighted Entry";
		
		[Min(0)]
		public uint Weight;
		public T Contents;

		public WeightedListEntry(uint weight, T contents)
		{
			this.Weight = weight;
			this.Contents = contents;
		}
	}
	
	[Serializable]
	public class WeightedList<T> : IEnumerable<T>, ISerializationCallbackReceiver
	{
		[HideInInspector]
		public string name = $"Weighted {nameof(T)}";
		
		[ReadOnly]
		[SerializeField]
		private uint m_TotalWeight = 0;

		[SerializeField]
		private List<WeightedListEntry<T>> m_Entries = new();
		
		private bool m_Initialized = false;

		public void Add(uint weight, T content)
		{
			m_TotalWeight += weight;
			m_Entries.Add(new WeightedListEntry<T>(weight, content));
		}

		public void Combine(WeightedList<T> otherList)
		{
			m_TotalWeight += otherList.m_TotalWeight;
			m_Entries.AddRange(otherList.m_Entries);
		}

		public void RemoveAtIndex(int index)
		{
			if (m_Entries.Count <= index)
			{
				throw new($"Index {index} out of bounds");
			}
			var entry = m_Entries[index];
			m_TotalWeight -= entry.Weight;
			m_Entries.RemoveAt(index);
		}

		public void RemoveRange(int index, int count)
		{
			if (m_Entries.Count <= index + count)
			{
				throw new($"Index {index + count} out of bounds");
			}
			for (int i = index; i < count; i++)
			{
				m_TotalWeight -= m_Entries[i].Weight;
			}
			m_Entries.RemoveRange(index, count);
		}
		
		public T RandomUnweightedValue()
		{
			if (m_Entries.Count <= 0)
			{
				throw new("Weighted List is empty, unable to get random entry");
			}
			return m_Entries.RandomValue().Contents;
		}
		
		public bool TryRandomUnweightedValue(Predicate<T> predicate, out T value)
		{
			if (m_Entries.Count <= 0)
			{
				value = default(T);
				return false;
			}

			List<WeightedListEntry<T>> validEntries = new List<WeightedListEntry<T>>();
			foreach (WeightedListEntry<T> entry in m_Entries)
			{
				if (predicate == null || predicate(entry.Contents))
				{
					validEntries.Add(entry);
				}
			}
			
			if (validEntries.Count <= 0)
			{
				value = default(T);
				return false;
			}

			value =  validEntries[Random.Range(0, (int)validEntries.Count)].Contents;
			return true;
		}

		public T RandomValue()
		{
			if(m_Initialized == false)
			{
				TallyWeight();
				m_Initialized = true;
			}
			
			if (m_Entries.Count <= 0)
			{
				throw new("Weighted List is empty, unable to get random entry");
			}
			
			int target = Random.Range(0, (int)m_TotalWeight);
			uint runningWeight = 0;
			for (int i = 0; i < m_Entries.Count; i++)
			{
				runningWeight += m_Entries[i].Weight;
				if (target < runningWeight && m_Entries[i].Weight > 0)
				{
					return m_Entries[i].Contents;
				}
			}
			
			throw new($"Weighted List failed to get random entry");
		}

		public bool TryGetRandomValue(Predicate<T> predicate, out T value)
		{
			if (m_Entries.Count <= 0)
			{
				value = default(T);
				return false;
			}

			if (predicate == null)
			{
				value = RandomValue();
				return true;
			}
			
			List<WeightedListEntry<T>> validEntries = new List<WeightedListEntry<T>>();
			uint newTally = 0;
			foreach (WeightedListEntry<T> entry in m_Entries)
			{
				if (predicate(entry.Contents))
				{
					newTally += entry.Weight;
					validEntries.Add(entry);
				}
			}
			
			int target = Random.Range(0, (int)newTally);
			uint runningTally = 0;
			for (int i = 0; i < validEntries.Count; i++)
			{
				runningTally += validEntries[i].Weight;
				if (target < runningTally && validEntries[i].Weight > 0)
				{
					value = validEntries[i].Contents;
					return true;
				}
			}
			
			value = default(T);
			return false;
		}
		
		public IEnumerator<T> GetEnumerator()
		{
			foreach (var item in m_Entries)
			{
				yield return item.Contents;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private void TallyWeight()
		{
			m_TotalWeight = 0;
			foreach (WeightedListEntry<T> entry in m_Entries)
			{
				m_TotalWeight += entry.Weight;
			}
		}
		
		public void OnBeforeSerialize() { }

		public void OnAfterDeserialize()
		{
			m_Initialized = false;
			TallyWeight();
		}
	}
}
