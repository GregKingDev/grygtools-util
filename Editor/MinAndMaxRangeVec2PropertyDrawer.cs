using GrygToolsUtils;
using UnityEditor;
using UnityEngine;

namespace GrygToolsUtils
{
	[CustomPropertyDrawer(typeof(MinAndMaxRangeVec2Attribute))]
	public class MinAndMaxRangeVec2PropertyDrawer : PropertyDrawer
	{
		private float? oldMin = null;
		private float? oldMax = null;
		
		public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			float lowerLimit = float.MinValue;
			float upperLimit = float.MaxValue;
			if (attribute is MinAndMaxRangeVec2Attribute minMaxAttribute)
			{
				lowerLimit = minMaxAttribute.min;
				upperLimit = minMaxAttribute.max;
			}
			
			if (property.propertyType is SerializedPropertyType.Vector2 or SerializedPropertyType.Vector2Int)
			{
				float minValue = 0;
				float maxValue = 0;
				
				if (property.propertyType == SerializedPropertyType.Vector2)
				{
					minValue = property.vector2Value.x;
					maxValue = property.vector2Value.y;
				}
				else if (property.propertyType == SerializedPropertyType.Vector2Int)
				{
					minValue = property.vector2IntValue.x;
					maxValue = property.vector2IntValue.y;
				}
				
				float labelWidth = GUI.skin.label.CalcSize(new GUIContent(property.displayName)).x;
				float runningX = 0;
				const float floatFieldWidth = 80;
				const float spacer = 5f;
				EditorGUI.LabelField(new Rect(rect.x + runningX, rect.y, labelWidth, EditorGUIUtility.singleLineHeight),
					property.displayName);
				runningX += labelWidth + spacer;

				minValue = Mathf.Max(lowerLimit, EditorGUI.FloatField(new Rect(rect.x + runningX, rect.y, floatFieldWidth, EditorGUIUtility.singleLineHeight),
					minValue));
				runningX += floatFieldWidth + spacer;

				float sliderWidth = rect.width - (runningX + floatFieldWidth + spacer + spacer);
				EditorGUI.MinMaxSlider(new Rect(rect.x + runningX, rect.y, sliderWidth, EditorGUIUtility.singleLineHeight),
					ref minValue, ref maxValue, lowerLimit, upperLimit);
				runningX += sliderWidth + 5;
				
				maxValue = Mathf.Min(upperLimit, EditorGUI.FloatField(new Rect(rect.x + runningX, rect.y, floatFieldWidth, EditorGUIUtility.singleLineHeight),
					maxValue));

				if (oldMin == null)
				{
					oldMin = minValue;
				}
				else if (Mathf.Abs(minValue - oldMin.Value) > Mathf.Epsilon)
				{
					minValue = Mathf.Min(minValue, maxValue);
					oldMin = minValue;
				}

				if (oldMax == null)
				{
					oldMax = maxValue;
				}
				else if (Mathf.Abs(maxValue - oldMax.Value) > Mathf.Epsilon)
				{
					maxValue = Mathf.Max(minValue, maxValue);
					oldMax = maxValue;
				}

				if (property.propertyType == SerializedPropertyType.Vector2)
				{
					property.vector2Value = new Vector2(Mathf.Min(minValue, maxValue), Mathf.Max(minValue, maxValue));
				}
				else if (property.propertyType == SerializedPropertyType.Vector2Int)
				{
					property.vector2IntValue = new Vector2Int(Mathf.RoundToInt(Mathf.Min(minValue, maxValue)), Mathf.RoundToInt(Mathf.Max(minValue, maxValue)));
				}
			}
			else
			{
				GUIStyle errorStyle = "CN EntryErrorIconSmall";
				Rect r = new Rect(rect);
				r.width = errorStyle.fixedWidth;
				rect.xMin = r.xMax;
				GUI.Label(r, "", errorStyle);
				GUI.Label(rect,  "MinMaxRangePropertyDrawer can only be used on Vector2 and Vector2Int fields.");
			}
		}
	}
}