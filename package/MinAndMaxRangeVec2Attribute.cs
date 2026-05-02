using System;
using UnityEngine;

namespace GrygToolsUtils
{
	[AttributeUsage(AttributeTargets.Field)]
	public class MinAndMaxRangeVec2Attribute : PropertyAttribute
	{
		public float min;
		public float max;

		public MinAndMaxRangeVec2Attribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}