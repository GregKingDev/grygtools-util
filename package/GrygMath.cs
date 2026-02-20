using UnityEngine;

namespace GrygTools
{
	public static class GrygMath
	{
		public static bool AlmostEqual(float x, float y) 
		{
			return Mathf.Abs(x - y) <= float.Epsilon * Mathf.Abs(x + y) * 2
			       || Mathf.Abs(x - y) < float.MinValue;
		}
	}
}