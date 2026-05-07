using System;
using UnityEditor;
using UnityEngine;

namespace GrygToolsUtils
{
    public class SearchablePopup : PopupWindowContent
    {
        private const float k_SearchOffset = 6f;
        private const float k_WindowHeight = 600f;
        
        private readonly Action<int> m_OnSelect;
        private readonly int m_CurrentIndex;
        private readonly FilterableList m_FilterableList;
        private Vector2 m_Scroll;
        private int m_HoverIndex;
        private int m_ScrollToIndex;
        private float m_ScrollOffset;
        
        private readonly float m_MaxWidth = 200;
        
        public static void Show(Rect sourceRect, string[] options, int current, Action<int> onSelect)
        {
            SearchablePopup win = new SearchablePopup(options, current, onSelect);
            PopupWindow.Show(sourceRect, win);
        }

        private static void DrawHighlight(Rect rect, Color color)
        {
            Color tempColor = GUI.color;
            GUI.color = color;
            GUI.Box(rect, new GUIContent(), "SelectionRect");
            GUI.color = tempColor;
        }
        
        private SearchablePopup(string[] names, int mCurrentIndex, Action<int> mOnSelect)
        {
            m_FilterableList = new FilterableList(names);
            
            foreach (string name in names)
            {
                m_MaxWidth = Mathf.Max(GUI.skin.label.CalcSize(new GUIContent(name)).x, m_MaxWidth);
            }
            m_CurrentIndex = mCurrentIndex;
            m_OnSelect = mOnSelect;
            
            m_HoverIndex = mCurrentIndex;
            m_ScrollToIndex = mCurrentIndex;
            m_ScrollOffset = GetWindowSize().y - EditorGUIUtility.singleLineHeight * 2;
        }
        
        public override Vector2 GetWindowSize()
        {
            return new Vector2(m_MaxWidth, Mathf.Min(k_WindowHeight, m_FilterableList.MaxLength * EditorGUIUtility.singleLineHeight + EditorStyles.toolbar.fixedHeight));
        }
        
        public override void OnGUI(Rect rect)
        {
            Rect searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
            Rect scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

            HandleKeyboard();
            DrawSearch(searchRect);
            DrawSelectionArea(scrollRect);
        }
        
        private void DrawSearch(Rect rect)
        {
            Rect searchRect = new Rect(rect);
            searchRect.xMin += k_SearchOffset;
            searchRect.xMax -= k_SearchOffset;
            searchRect.y += 2;
            
            GUI.FocusControl("SearchablePopup");
            GUI.SetNextControlName("SearchablePopup");
            string newText = EditorGUI.TextField(searchRect, m_FilterableList.Filter);

            if (m_FilterableList.UpdateFilter(newText))
            {
                m_HoverIndex = 0;
                m_Scroll = Vector2.zero;
            }

            searchRect.x = searchRect.xMax;
        }
        
        private void DrawSelectionArea(Rect scrollRect)
        {
            Rect contentRect = new Rect(0, 0, scrollRect.width - GUI.skin.verticalScrollbar.fixedWidth, m_FilterableList.Entries.Count * EditorGUIUtility.singleLineHeight);
            Rect rowRect = new Rect(0, 0, scrollRect.width, EditorGUIUtility.singleLineHeight);
            
            m_Scroll = GUI.BeginScrollView(scrollRect, m_Scroll, contentRect);

            for (int i = 0; i < m_FilterableList.Entries.Count; i++)
            {
                if (m_ScrollToIndex == i && (Event.current.type == EventType.Repaint || Event.current.type == EventType.Layout))
                {
                    Rect r = new Rect(rowRect);
                    r.y += m_ScrollOffset;
                    GUI.ScrollTo(r);
                    m_ScrollToIndex = -1;
                    m_Scroll.x = 0;
                }

                if (rowRect.Contains(Event.current.mousePosition))
                {
                    switch (Event.current.type)
                    {
                        case EventType.MouseMove:
                        case EventType.ScrollWheel:
                            m_HoverIndex = i;
                            break;
                        case EventType.MouseDown:
                            m_OnSelect(m_FilterableList.Entries[i].Index);
                            EditorWindow.focusedWindow.Close();
                            break;
                    }
                }

                DrawEntry(rowRect, i);

                rowRect.y = rowRect.yMax;
            }

            GUI.EndScrollView();
        }

        private void DrawEntry(Rect rect, int i)
        {
            Color temp = GUI.color;
            
            if (m_FilterableList.Entries[i].Index == m_CurrentIndex)
            {
                DrawHighlight(rect, Color.cyan);
                GUI.color = Color.yellow;
            }
            else if (i == m_HoverIndex)
            {
                DrawHighlight(rect, Color.white);
            }

            Rect labelRect = new Rect(rect);
            EditorGUI.indentLevel++;
            EditorGUI.LabelField(labelRect, m_FilterableList.Entries[i].Text);
            EditorGUI.indentLevel--;
            GUI.color = temp;
        }
        
        private void HandleKeyboard()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.DownArrow:
                        m_HoverIndex = Mathf.Min(m_FilterableList.Entries.Count - 1, m_HoverIndex + 1);
                        Event.current.Use();
                        m_ScrollToIndex = m_HoverIndex;
                        m_ScrollOffset = EditorGUIUtility.singleLineHeight;
                        break;
                    case KeyCode.UpArrow:
                        m_HoverIndex = Mathf.Max(0, m_HoverIndex - 1);
                        Event.current.Use();
                        m_ScrollToIndex = m_HoverIndex;
                        m_ScrollOffset = -EditorGUIUtility.singleLineHeight;
                        break;
                    case KeyCode.Return:
                        if (m_HoverIndex >= 0 && m_HoverIndex < m_FilterableList.Entries.Count)
                        {
                            m_OnSelect(m_FilterableList.Entries[m_HoverIndex].Index);
                            EditorWindow.focusedWindow.Close();
                        }
                        break;
                    case KeyCode.Escape:
                        EditorWindow.focusedWindow.Close();
                        break;
                }
            }
        }
    }
}