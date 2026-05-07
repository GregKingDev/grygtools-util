using System;
using System.Collections.Generic;

namespace GrygToolsUtils
{
    internal struct Entry
    {
        public readonly int Index;
        public readonly string Text;
        public Entry(int index, string text)
        {
            Index = index;
            Text = text;
        }
    }
    
	internal class FilterableList
    {
        private readonly string[] m_Items;
        public string Filter { get; private set; }
        public List<Entry> Entries { get; }
        public int MaxLength => m_Items.Length;
        
        public FilterableList(string[] mItems)
        {
            m_Items = mItems;
            Entries = new List<Entry>();
            UpdateFilter("");
        }

       public bool UpdateFilter(string filter)
        {
            if (Filter == filter)
            {
                return false;
            }

            Filter = filter;
            Entries.Clear();
            
            string[] searchFragments = filter.ToLower().Split(' ');
            
            for (int i = 0; i < m_Items.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(Filter) || MeetsFilter(m_Items[i], searchFragments))
                {
                    Entry entry = new Entry(i, m_Items[i]);
                    if (string.Equals(m_Items[i], Filter, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Entries.Insert(0, entry);
                    }
                    else
                    {
                        Entries.Add(entry);
                    }
                }
            }
            return true;
        }
        
        private bool MeetsFilter(string targetString, string[] filterStrings)
        {
            foreach (string s in filterStrings)
            {
                string searchStringLower = s.ToLower();
                if (searchStringLower.StartsWith("-"))
                {
                    searchStringLower = searchStringLower.Remove(0, 1);
                    if (NameContains(targetString, searchStringLower))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!NameContains(targetString, searchStringLower))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    
        private bool NameContains(string name, string text)
        {
            if (name == null)
            {
                return false;
            }
            return name.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
