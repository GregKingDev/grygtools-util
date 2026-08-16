using System.Collections.Generic;
namespace GrygToolsUtils
{
	using System;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(MonoBehaviour), true)]
	[CanEditMultipleObjects]
	public class MonoBehaviourExtensionDrawer : Editor
	{
        private static readonly Dictionary<string, object[]> m_ParameterValues = new();
        private static readonly Dictionary<string, bool> m_FoldoutStates = new();

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            Type type = target.GetType();
            
            #region PropertyInspector
            var targetType = target.GetType();

            var properties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            bool drawSpacer = false;
            
            EditorGUI.BeginDisabledGroup(true);
            foreach (var prop in properties)
            {
                ShowPropertyAttribute showPropAttribute = prop.GetCustomAttribute<ShowPropertyAttribute>();
                if(showPropAttribute == null) continue;

                if (!drawSpacer)
                {
                    EditorGUILayout.Space(16);
                    drawSpacer = true;
                }
                
                object value = prop.GetValue(target, null);
                
                DrawTypeField(prop.Name, prop.PropertyType, value);
            }
            EditorGUI.EndDisabledGroup();
            #endregion
            
            if(drawSpacer)
            {
                EditorGUILayout.Space(4);
            }
            
            #region InspecitorButtonAttribute
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MethodInfo method in methods)
            {
                InspectorButtonAttribute buttonAttr = method.GetCustomAttribute<InspectorButtonAttribute>();
                if (buttonAttr == null) continue;

                if ((Application.isPlaying && buttonAttr.DrawMode == InspectorButtonDrawMode.EditorOnly) ||
                    (!Application.isPlaying && buttonAttr.DrawMode == InspectorButtonDrawMode.GameplayOnly))
                    continue;

                string methodKey = $"{target.GetInstanceID()}.{method.Name}";
                string label = string.IsNullOrEmpty(buttonAttr.ButtonLabel) ? method.Name : buttonAttr.ButtonLabel;
                ParameterInfo[] parameters = method.GetParameters();

                if (!m_ParameterValues.ContainsKey(methodKey))
                {
                    object[] defaults = new object[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        ParameterInfo param = parameters[i];
                        if (param.HasDefaultValue && param.DefaultValue != null)
                        {
                            defaults[i] = param.DefaultValue;
                        }
                        else if (param.ParameterType.IsValueType)
                        {
                            defaults[i] = Activator.CreateInstance(param.ParameterType);
                        }
                        else if (param.ParameterType == typeof(string))
                        {
                            defaults[i] = string.Empty;
                        }
                        else if (param.ParameterType == typeof(AnimationCurve))
                        {
                            defaults[i] = new AnimationCurve();
                        }
                    }
                    m_ParameterValues[methodKey] = defaults;
                }

                if (!m_FoldoutStates.ContainsKey(methodKey))
                {
                    m_FoldoutStates[methodKey] = true;
                }
                
                object[] paramValues = m_ParameterValues[methodKey];
                
                m_FoldoutStates[methodKey] = EditorGUILayout.Foldout(m_FoldoutStates[methodKey], label, true, EditorStyles.foldoutHeader);

                if (m_FoldoutStates[methodKey])
                {
                    if (parameters.Length > 0)
                    {
                        EditorGUI.indentLevel++;

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            ParameterInfo param = parameters[i];
                            paramValues[i] = DrawParameterField(param, paramValues[i]);
                        }

                        EditorGUI.indentLevel--;
                    }

                    if (GUILayout.Button(parameters.Length > 0 ? $"Invoke {label}" : label))
                    {
                        Undo.RecordObject(target, $"Trigger {method.Name}");
                        foreach (UnityEngine.Object targetObj in targets)
                        {
                            method.Invoke(targetObj, paramValues);
                        }
                    }
                }

                EditorGUILayout.Space(4);
            }
            #endregion
        }

        private object DrawParameterField(ParameterInfo param, object currentValue)
        {
            string fieldLabel = ObjectNames.NicifyVariableName(param.Name);
            Type t = param.ParameterType;
            return DrawTypeField(fieldLabel, t, currentValue);
        }

        private object DrawTypeField(string fieldName, Type t, object currentValue = null)
        {
            string fieldLabel = ObjectNames.NicifyVariableName(fieldName);
            if (t == typeof(int))
                return EditorGUILayout.IntField(fieldLabel, currentValue is int v ? v : 0);

            if (t == typeof(float))
                return EditorGUILayout.FloatField(fieldLabel, currentValue is float v ? v : 0f);

            if (t == typeof(double))
                return (double)EditorGUILayout.DoubleField(fieldLabel, currentValue is double v ? v : 0.0);

            if (t == typeof(bool))
                return EditorGUILayout.Toggle(fieldLabel, currentValue is bool v && v);

            if (t == typeof(string))
                return EditorGUILayout.TextField(fieldLabel, currentValue is string v ? v : string.Empty);

            if (t == typeof(Vector2))
                return EditorGUILayout.Vector2Field(fieldLabel, currentValue is Vector2 v ? v : Vector2.zero);

            if (t == typeof(Vector3))
                return EditorGUILayout.Vector3Field(fieldLabel, currentValue is Vector3 v ? v : Vector3.zero);

            if (t == typeof(Color))
                return EditorGUILayout.ColorField(fieldLabel, currentValue is Color v ? v : Color.white);

            if (t == typeof(AnimationCurve))
                return EditorGUILayout.CurveField(fieldLabel, currentValue is AnimationCurve v ? v : new AnimationCurve());

            if (t.IsEnum)
                return EditorGUILayout.EnumPopup(fieldLabel, currentValue is Enum v ? v : (Enum)Enum.GetValues(t).GetValue(0));

            if (typeof(UnityEngine.Object).IsAssignableFrom(t))
                return EditorGUILayout.ObjectField(fieldLabel, currentValue as UnityEngine.Object, t, true);

            EditorGUILayout.HelpBox($"Unsupported parameter type: {t.Name}", MessageType.Warning);
            return currentValue;
        }
    }
}