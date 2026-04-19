using System;
using UnityEditor;
using UnityEngine;

namespace GrygToolsUtils
{
    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
	public class SearchableEnumDrawer: PropertyDrawer
    {
        private const string TYPE_ERROR =
            "SearchableEnum can only be used on enum fields.";

        private int idHash;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.type != "Enum")
            {
                GUIStyle errorStyle = "CN EntryErrorIconSmall";
                Rect r = new Rect(position);
                r.width = errorStyle.fixedWidth;
                position.xMin = r.xMax;
                GUI.Label(r, "", errorStyle);
                GUI.Label(position, TYPE_ERROR);
                return;
            }
            
            if (idHash == 0) idHash = "SearchableEnumDrawer".GetHashCode();
            int id = GUIUtility.GetControlID(idHash, FocusType.Keyboard, position);
            
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, id, label);

            GUIContent buttonText;
            if (property.enumValueIndex < 0 || property.enumValueIndex >= property.enumDisplayNames.Length) {
                buttonText = new GUIContent();
            }
            else {
                buttonText = new GUIContent(property.enumDisplayNames[property.enumValueIndex]);
            }
            
            if (DropdownButton(id, position, buttonText))
            {
                Action<int> onSelect = i =>
                {
                    property.enumValueIndex = i;
                    property.serializedObject.ApplyModifiedProperties();
                };
                
                SearchablePopup.Show(position, property.enumDisplayNames,
                    property.enumValueIndex, onSelect);
            }
            EditorGUI.EndProperty();
        }
        
        private static bool DropdownButton(int id, Rect position, GUIContent content)
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