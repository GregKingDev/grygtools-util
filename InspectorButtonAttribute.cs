using System;
namespace GrygToolsUtils
{
	public enum InspectorButtonDrawMode
	{
		EditorOnly,
		RuntimeOnly,
		EditorAndRuntime
	}
	
	
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class InspectorButtonAttribute : Attribute
	{
		public string ButtonLabel { get; private set; }
		public InspectorButtonDrawMode DrawMode { get; private set; }

		public InspectorButtonAttribute(string buttonLabel = null, InspectorButtonDrawMode drawMode = InspectorButtonDrawMode.EditorAndRuntime)
		{
			ButtonLabel = buttonLabel;
			DrawMode = drawMode;
		}
	}
}
