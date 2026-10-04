using UnityEditor;
using UnityEngine;
namespace GrygTools.Utils.Attributes
{
	[CustomPropertyDrawer(typeof(TagAttribute))]
	public class TagPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType == SerializedPropertyType.String)
			{
				property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
			}
			else
			{
				EditorGUI.LabelField(position, label.text, "Use [Tag] on strings only.");
			}
		}
	}
}
