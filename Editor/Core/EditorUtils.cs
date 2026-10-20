using UnityEditor;
using UnityEngine;
namespace GrygTools.Audio
{
	public static class EditorUtils
	{
		public static bool DropdownButton(int id, Rect position, GUIContent content)
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
