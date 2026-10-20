using GrygTools.Utils.Attributes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DefaultPropertyDrawerAttribute))]
public class DefaultPropertyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUIUtility.singleLineHeight + 2;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		Rect nameRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("_value"), GUIContent.none);
		EditorGUI.EndProperty();
	}
}