using System;
using UnityEngine;

namespace GrygToolsUtils
{
	/// <summary>
	/// Hides a SerializeField in the editor based the name of another field in the class and comparison value
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ConditionalSerializedField : PropertyAttribute
	{
		public readonly string FieldName;
		public readonly object Value;
		public readonly bool Invert;

		/// <summary>
		/// Hides a SerializeField in the editor based the name of another field in the class and comparison value
		/// </summary>
		/// <param name="name">Name of the other field used to determine visibility</param>
		/// <param name="v">Value the other param is checked against</param>
		/// <param name="invert">If invert is true field will be hidden when values match</param>
		public ConditionalSerializedField(string name, object v, bool invert = false)
		{
			FieldName = name;
			Value = v;
			Invert = invert;
		}
	}
}