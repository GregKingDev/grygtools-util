using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace GrygToolsUtils
{
	[CustomPropertyDrawer(typeof(WeightedList<>))]
	public class WeightedListDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var entriesProp = property.FindPropertyRelative("m_Entries");
			return EditorGUI.GetPropertyHeight(entriesProp, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var totalWeightProp = property.FindPropertyRelative("m_TotalWeight");
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.PropertyField(position, totalWeightProp);
			EditorGUI.EndDisabledGroup();
			position.y += EditorGUI.GetPropertyHeight(totalWeightProp) + EditorGUIUtility.standardVerticalSpacing;
			var entriesProp = property.FindPropertyRelative("m_Entries");
			
			var genericArgs = fieldInfo.FieldType.GetGenericArguments();
			var typeName = genericArgs.Length > 0 ? genericArgs[0].Name : "Entry";
			var entriesLabel = new GUIContent($"{typeName} Entries");
			EditorGUI.PropertyField(position, entriesProp, entriesLabel, true);
		}
	}
}
