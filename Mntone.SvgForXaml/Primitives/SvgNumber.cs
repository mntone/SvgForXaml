using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using System;

namespace Mntone.SvgForXaml.Primitives
{
	[System.Diagnostics.DebuggerDisplay("Number: {this.Value}")]
	public struct SvgNumber : ICssValue
	{
		internal SvgNumber(float value)
		{
			this.Value = value;
		}

		public float Value { get; }

		internal static SvgNumber Parse(string numberText)
		{
			SvgNumber result;
			if (!TryParse(numberText, out result)) throw new ArgumentException(nameof(numberText));
			return result;
		}

		internal static SvgNumber Parse(string numberText, float min, float max)
		{
			SvgNumber result;
			if (!TryParse(numberText, min, max, out result)) throw new ArgumentException(nameof(numberText));
			return result;
		}

		internal static bool TryParse(string numberText, out SvgNumber result)
		{
			var ptr = new StringPtr(numberText);
			ptr.AdvanceNumber();
			if (ptr.Index != numberText.Length)
			{
				result = 0.0F;
				return false;
			}

			result = float.Parse(numberText, System.Globalization.CultureInfo.InvariantCulture);
			return true;
		}

		internal static bool TryParse(string numberText, float min, float max, out SvgNumber result)
		{
			var ptr = new StringPtr(numberText);
			ptr.AdvanceNumber();
			if (ptr.Index != numberText.Length)
			{
				result = 0.0F;
				return false;
			}

			result = Math.Min(Math.Max(float.Parse(numberText), min), max);
			return true;
		}

		public static implicit operator SvgNumber(float f) => new SvgNumber(f);
		public static implicit operator float(SvgNumber n) => n.Value;
	}
}