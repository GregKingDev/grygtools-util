using System;
using System.Reflection;
using System.Collections;
using UnityEditor;

namespace GrygTools.Utils.Extensions
{
    public static class SerializedPropertyExtensions
    {
        public static T GetActualObject<T>(this SerializedProperty property)
        {
            if (property == null) return default;
            object currentObject = property.serializedObject.targetObject;
            string[] pathSteps = property.propertyPath.Split('.');

            for (int i = 0; i < pathSteps.Length; i++)
            {
                string step = pathSteps[i];
                if (step == "Array" && i + 1 < pathSteps.Length && pathSteps[i + 1].StartsWith("data["))
                {
                    i++;
                    string dataStep = pathSteps[i];
                    int startIndex = dataStep.IndexOf('[') + 1;
                    int endIndex = dataStep.IndexOf(']');
                    int index = int.Parse(dataStep.Substring(startIndex, endIndex - startIndex));

                    IList list = currentObject as IList;
                    if (list != null && index < list.Count)
                    {
                        currentObject = list[index];
                    }
                    else
                    {
                        return default;
                    }
                }
                else
                {
                    FieldInfo field = GetFieldIncludingBaseTypes(currentObject.GetType(), step);
                    if (field != null)
                    {
                        currentObject = field.GetValue(currentObject);
                    }
                    else
                    {
                        return default;
                    }
                }
            }

            return (T)currentObject;
        }

        private static FieldInfo GetFieldIncludingBaseTypes(Type type, string fieldName)
        {
            while (type != null)
            {
                FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null) return field;
                type = type.BaseType;
            }
            return null;
        }
    }
}