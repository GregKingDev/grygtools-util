using System;
using System.Collections;
using TMPro;
using UnityEngine;
namespace GrygTools.Utils.Extensions
{
	public static class TmpTextExtensions
	{
		public static IEnumerator TextAnimateTest(this TMP_Text text, float fromValue, float toValue, float duration, string formatString = "{0}", int decimalPlaces = 0, bool round = false)
		{
			float elapsedTime = 0f;
			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float t = Mathf.Clamp01(elapsedTime / duration);
				var floatValue = Mathf.Lerp(fromValue, toValue, t);
				string value;
				value = round ? MathF.Round(floatValue, decimalPlaces).ToString() : floatValue.ToString($"#.{new string('#', decimalPlaces)}");
				
				text.text = string.Format(formatString, value);
				yield return null;
			}
			
			string finalValue = round ? MathF.Round(toValue, decimalPlaces).ToString() : toValue.ToString($"#.{new string('#', decimalPlaces)}");
			text.text = string.Format(formatString, finalValue);
		}
	}
}
