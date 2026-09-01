using UnityEngine;
using UnityEditor;

namespace GrygTools.Utils.Attributes
{
	/// <summary>
	/// Conditionally shows this serialized field in editor. First field is the name of the property to check second is the value.
	/// Currently works with bool, string, int, enum, float
	/// </summary>
	[CustomPropertyDrawer(typeof(ConditionalSerializedField))]
	public class ConditionalSerializedFieldEditor : PropertyDrawer
	{
		private bool m_PassesCheck = true;
		private bool m_Inverted = false;

		private bool ShouldShow => (m_Inverted && !m_PassesCheck) || (!m_Inverted && m_PassesCheck); 
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			ConditionalSerializedField localAttribute = (ConditionalSerializedField)this.attribute;
			string name = localAttribute.FieldName;
			m_Inverted = localAttribute.Invert;

			SerializedProperty field = property.serializedObject.FindProperty(name);

			switch (field.propertyType)
			{
				case SerializedPropertyType.Boolean:
				{
					m_PassesCheck = field.boolValue.CompareTo(localAttribute.Value) == 0;
					break;
				}

				case SerializedPropertyType.String:
				{
					m_PassesCheck = field.stringValue.CompareTo(localAttribute.Value) == 0;
					break;
				}

				case SerializedPropertyType.Enum:
				{
					m_PassesCheck = field.intValue == (int)localAttribute.Value;
					break;
				}

				case SerializedPropertyType.Integer:
				{
					m_PassesCheck = field.intValue.CompareTo(localAttribute.Value) == 0;
					break;
				}

				case SerializedPropertyType.Float:
				{
					m_PassesCheck = field.floatValue.CompareTo(localAttribute.Value) == 0;
					break;
				}

				case SerializedPropertyType.ObjectReference:
				{
					m_PassesCheck = (field.objectReferenceValue != null).CompareTo(localAttribute.Value) == 0;
					break;
				}

				default:
				{
					Debug.LogError(
						$"Unrecognized type of {field.propertyType}. Only int, float, bool, enum and string are supported by ConditionalSerializedField");
					m_PassesCheck = false;
					break;
				}
			}

			if (ShouldShow)
			{
				EditorGUI.PropertyField(position, property, label, true);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (ShouldShow)
			{
				return EditorGUI.GetPropertyHeight(property);
			}
			else
			{
				return -EditorGUIUtility.standardVerticalSpacing;
			}
		}
	}
}