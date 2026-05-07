using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GrygToolsUtils
{
	public abstract class SearchablePropertyDrawerBase<TObj> : PropertyDrawer
	{
		protected int idHash => typeof(TObj).FullName.GetHashCode();
		protected Dictionary<int, TObj> optionsDictionary = new ();
		protected List<TObj> optionsList = new ();
		protected Dictionary<object, string> nameDictionary = new();
		
		protected abstract void OnSelect(SerializedProperty property, TObj obj);
		protected abstract bool TypeCheck(SerializedProperty property, out string error);

		protected abstract bool IndexComparison(SerializedProperty property, TObj obj);

		protected abstract void Populate();

		protected abstract string GetButtonText(SerializedProperty property);
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!TypeCheck(property, out string errorString))
            {
	            GUIStyle errorStyle = "CN EntryErrorIconSmall";
	            Rect r = new Rect(position);
	            r.width = errorStyle.fixedWidth;
	            position.xMin = r.xMax;
	            GUI.Label(r, "", errorStyle);
	            GUI.Label(position,  errorString);
	            return;
            }

            Populate();
            int id = GUIUtility.GetControlID(idHash, FocusType.Keyboard, position);
            
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);

            GUIContent buttonText;
	        buttonText = new GUIContent(GetButtonText(property));
            
            if (DropdownButton(id, position, buttonText))
            {
                Action<int> onSelect = i =>
                {
	                OnSelect(property, optionsList[i]);
                    property.serializedObject.ApplyModifiedProperties();
                };

                int index = 0;
                if (property.intValue != 0)
                {
                    index = optionsList.FindIndex(0, obj => IndexComparison(property, obj));
                }
                
                SearchablePopup.Show(position, nameDictionary.Values.ToArray(), index, onSelect);
            }
            EditorGUI.EndProperty();
        }
		
		protected static bool DropdownButton(int id, Rect position, GUIContent content)
		{
			Event current = Event.current;
			switch (current.type)
			{
				case EventType.MouseDown:
					if (position.Contains(current.mousePosition) && current.button == 0)
					{
						Event.current.Use();
						return true;
					}
					break;
				case EventType.KeyDown:
					if (GUIUtility.keyboardControl == id && current.character =='\n')
					{
						Event.current.Use();
						return true;
					}
					break;
				case EventType.Repaint:
					EditorStyles.popup.Draw(position, content, id, false);
					break;
			}
			return false;
		} 
	}
}
