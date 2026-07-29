using System;
namespace GrygToolsUtils
{
	public enum InspectorButtonDrawMode
	{
		EditorOnly,
		GameplayOnly,
		EditorAndGameplay
	}
	
	
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class InspectorButtonAttribute : Attribute
	{
		public string ButtonLabel { get; private set; }
		public InspectorButtonDrawMode DrawMode { get; private set; }

		public InspectorButtonAttribute(string buttonLabel = null, InspectorButtonDrawMode drawMode = InspectorButtonDrawMode.EditorAndGameplay)
		{
			ButtonLabel = buttonLabel;
			DrawMode = drawMode;
		}
	}
}
