namespace GrygToolsUtils
{
	using System;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(MonoBehaviour), true)]
	[CanEditMultipleObjects]
	public class InspectorButtonDrawer : Editor
	{
		public override void OnInspectorGUI()
		{
			// 1. Draw all standard serialized variables first
			DrawDefaultInspector();

			// 2. Fetch all methods on the target object
			Type type = target.GetType();
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (MethodInfo method in methods)
			{
				// check if the method has our [Button] attribute
				InspectorButtonAttribute buttonAttr = method.GetCustomAttribute<InspectorButtonAttribute>();
				if (buttonAttr == null) continue;

				// Enforce that the method cannot require arguments
				if (method.GetParameters().Length > 0)
				{
					EditorGUILayout.HelpBox($"Method {method.Name} requires parameters and cannot be drawn as a button.", MessageType.Warning);
					continue;
				}

				// Determine label: use custom label if provided, otherwise clean up method name
				string label = string.IsNullOrEmpty(buttonAttr.ButtonLabel) ? method.Name : buttonAttr.ButtonLabel;


				if ((Application.isPlaying && buttonAttr.DrawMode == InspectorButtonDrawMode.EditorOnly) || (!Application.isPlaying & buttonAttr.DrawMode == InspectorButtonDrawMode.RuntimeOnly))
				{
					continue;
				}
				// 3. Render the button
				if (GUILayout.Button(label))
				{
					// Record undo state so changes made by the method can be saved/undone
					Undo.RecordObject(target, $"Trigger {method.Name}");

					// Execute the method on all selected objects (supports multi-editing)
					foreach (UnityEngine.Object targetObj in targets)
					{
						method.Invoke(targetObj, null);
					}
				}
			}
		}
	}
}
