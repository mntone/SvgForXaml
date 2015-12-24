using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using System;

namespace Mntone.SvgForXaml.Primitives
{
	[System.Diagnostics.DebuggerDisplay("{this.ValueAsString}")]
	public struct SvgLength : ICssValue, IEquatable<SvgLength>
	{
		public enum SvgLengthType : ushort { Unknown = 0, Number, Percentage, Ems, Exs, Pixel, Centimeter, Millimeter, Inch, Point, Pica };

		internal SvgLength(SvgLengthType unitType, float value)
		{
			this.UnitType = unitType;
			this.Value = value;
		}

		public SvgLengthType UnitType { get; }
		public float Value { get; }

		public float ValueAsPixel
		{
			get
			{
				switch (this.UnitType)
				{
					case SvgLengthType.Percentage:
					case SvgLengthType.Ems:
					case SvgLengthType.Exs:
						throw new InvalidOperationException();

					case SvgLengthType.Centimeter:
						return this.Value * 96.0F / 0.254F;

					case SvgLengthType.Millimeter:
						return this.Value * 96.0F / 25.4F;

					case SvgLengthType.Inch:
						return this.Value * 96.0F;

					case SvgLengthType.Point:
						return this.Value * 12.0F / 9.0F;

					case SvgLengthType.Pica:
						return this.Value * 16.0F;
				}
				return this.Value;
			}
		}

		public string ValueAsString
		{
			get
			{
				switch (this.UnitType)
				{
					case SvgLengthType.Number:
						return this.Value.ToString();

					case SvgLengthType.Percentage:
						return $"{this.Value}%";

					case SvgLengthType.Ems:
						return $"{this.Value}em";

					case SvgLengthType.Exs:
						return $"{this.Value}ex";

					case SvgLengthType.Pixel:
						return $"{this.Value}px";

					case SvgLengthType.Centimeter:
						return $"{this.Value}cm";

					case SvgLengthType.Millimeter:
						return $"{this.Value}mm";

					case SvgLengthType.Inch:
						return $"{this.Value}in";

					case SvgLengthType.Point:
						return $"{this.Value}pt";

					case SvgLengthType.Pica:
						return $"{this.Value}pc";
				}
				throw new InvalidOperationException();
			}
		}

		internal static SvgLength Parse(string lengthText, bool presentation = true)
		{
			SvgLength result;
			if (!TryParse(lengthText, presentation, out result)) throw new ArgumentException(nameof(lengthText));
			return result;
		}

		internal static bool TryParse(string lengthText, out SvgLength result) => TryParse(lengthText, true, out result);
		internal static bool TryParse(string lengthText, bool presentation, out SvgLength result)
		{
			var ptr = new StringPtr(lengthText);
			ptr.AdvanceNumber();

			var value = float.Parse(lengthText.Substring(0, ptr.Index), System.Globalization.CultureInfo.InvariantCulture);
			if (ptr.Index != lengthText.Length)
			{
				var unit = lengthText.Substring(ptr.Index);
				if (presentation) unit = unit.ToLower();
				switch (unit)
				{
					case "%": result = new SvgLength(SvgLengthType.Percentage, value); break;
					case "em": result = new SvgLength(SvgLengthType.Ems, value); break;
					case "ex": result = new SvgLength(SvgLengthType.Exs, value); break;
					case "px": result = new SvgLength(SvgLengthType.Pixel, value); break;
					case "cm": result = new SvgLength(SvgLengthType.Centimeter, value); break;
					case "mm": result = new SvgLength(SvgLengthType.Millimeter, value); break;
					case "in": result = new SvgLength(SvgLengthType.Inch, value); break;
					case "pt": result = new SvgLength(SvgLengthType.Point, value); break;
					case "pc": result = new SvgLength(SvgLengthType.Pica, value); break;
					default: result = null; return false;
				}
				return true;
			}
			result = new SvgLength(SvgLengthType.Number, value);
			return true;
		}

		public bool Equals(SvgLength other) => this.UnitType == other.UnitType && this.Value == other.Value;

		public static implicit operator SvgLength(float f) => new SvgLength(SvgLengthType.Number, f);

		public static implicit operator SvgLength(string s) => Parse(s);
		public static implicit operator string(SvgLength a) => a.ValueAsString;

		public static SvgLength operator *(float left, SvgLength right) => new SvgLength(right.UnitType, left * right.Value);
		public static SvgLength operator *(SvgLength left, float right) => new SvgLength(left.UnitType, left.Value * right);
		public static SvgLength operator /(SvgLength left, float right) => new SvgLength(left.UnitType, left.Value / right);

		public static bool operator <(SvgLength left, SvgLength right)
		{
			if (left.UnitType == right.UnitType)
			{
				return left.Value < right.Value;
			}
			throw new NotSupportedException();
		}
		public static bool operator <=(SvgLength left, SvgLength right)
		{
			if (left.UnitType == right.UnitType)
			{
				return left.Value <= right.Value;
			}
			throw new NotSupportedException();
		}
		public static bool operator >(SvgLength left, SvgLength right)
		{
			if (left.UnitType == right.UnitType)
			{
				return left.Value > right.Value;
			}
			throw new NotSupportedException();
		}
		public static bool operator >=(SvgLength left, SvgLength right)
		{
			if (left.UnitType == right.UnitType)
			{
				return left.Value >= right.Value;
			}
			throw new NotSupportedException();
		}
	}

}