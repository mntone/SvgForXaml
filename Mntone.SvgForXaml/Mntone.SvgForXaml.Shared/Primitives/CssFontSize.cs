using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using System;

namespace Mntone.SvgForXaml.Primitives
{
	[System.Diagnostics.DebuggerDisplay("{this.ValueAsString}")]
	public struct CssFontSize : ICssValue, IEquatable<CssFontSize>
	{
		public enum CssFontSizeType : ushort
		{
			XXSmall, XSmall, Small, Medium, Large, XLarge, XXLarge,
			Larger, Smaller,
			Number, Percentage,
			Ems, Exs, Pixel, Centimeter, Millimeter, Inch, Point, Pica
		}

		internal CssFontSize(CssFontSizeType sizeType)
		{
			this.SizeType = sizeType;
			this.Value = 0f;
		}

		internal CssFontSize(CssFontSizeType sizeType, float value)
		{
			this.SizeType = sizeType;
			this.Value = value;
		}

		public CssFontSizeType SizeType { get; }
		public float Value { get; }

		public float ValueAsPixel
		{
			get
			{
				switch (this.SizeType)
				{
					case CssFontSizeType.XXSmall:
					case CssFontSizeType.XSmall:
					case CssFontSizeType.Small:
					case CssFontSizeType.Medium:
					case CssFontSizeType.Large:
					case CssFontSizeType.XLarge:
					case CssFontSizeType.XXLarge:
					case CssFontSizeType.Larger:
					case CssFontSizeType.Smaller:
					case CssFontSizeType.Percentage:
					case CssFontSizeType.Ems:
					case CssFontSizeType.Exs:
						throw new InvalidOperationException();

					case CssFontSizeType.Centimeter:
						return this.Value * 96.0F / 0.254F;

					case CssFontSizeType.Millimeter:
						return this.Value * 96.0F / 25.4F;

					case CssFontSizeType.Inch:
						return this.Value * 96.0F;

					case CssFontSizeType.Point:
						return this.Value * 12.0F / 9.0F;

					case CssFontSizeType.Pica:
						return this.Value * 16.0F;
				}
				return this.Value;
			}
		}

		public string ValueAsString
		{
			get
			{
				switch (this.SizeType)
				{
					case CssFontSizeType.XXSmall:
						return "xx-small";

					case CssFontSizeType.XSmall:
						return "x-small";

					case CssFontSizeType.Small:
						return "small";

					case CssFontSizeType.Medium:
						return "medium";

					case CssFontSizeType.Large:
						return "large";

					case CssFontSizeType.XLarge:
						return "x-large";

					case CssFontSizeType.XXLarge:
						return "xx-large";

					case CssFontSizeType.Larger:
						return "larger";

					case CssFontSizeType.Smaller:
						return "smaller";

					case CssFontSizeType.Number:
						return this.Value.ToString();

					case CssFontSizeType.Percentage:
						return $"{this.Value}%";

					case CssFontSizeType.Ems:
						return $"{this.Value}em";

					case CssFontSizeType.Exs:
						return $"{this.Value}ex";

					case CssFontSizeType.Pixel:
						return $"{this.Value}px";

					case CssFontSizeType.Centimeter:
						return $"{this.Value}cm";

					case CssFontSizeType.Millimeter:
						return $"{this.Value}mm";

					case CssFontSizeType.Inch:
						return $"{this.Value}in";

					case CssFontSizeType.Point:
						return $"{this.Value}pt";

					case CssFontSizeType.Pica:
						return $"{this.Value}pc";
				}
				throw new InvalidOperationException();
			}
		}

		internal static CssFontSize Parse(string fontSizeText, bool presentation = true)
		{
			CssFontSize result;
			if (!TryParse(fontSizeText, presentation, out result)) throw new ArgumentException(nameof(fontSizeText));
			return result;
		}

		internal static bool TryParse(string lengthText, out CssFontSize result) => TryParse(lengthText, true, out result);
		internal static bool TryParse(string lengthText, bool presentation, out CssFontSize result)
		{
			switch (lengthText.ToLowerInvariant())
			{
				case "xx-small": result = new CssFontSize(CssFontSizeType.XXSmall); return true;
				case "x-small": result = new CssFontSize(CssFontSizeType.XSmall); return true;
				case "small": result = new CssFontSize(CssFontSizeType.Small); return true;
				case "medium": result = new CssFontSize(CssFontSizeType.Medium); return true;
				case "large": result = new CssFontSize(CssFontSizeType.Large); return true;
				case "x-large": result = new CssFontSize(CssFontSizeType.XLarge); return true;
				case "xx-large": result = new CssFontSize(CssFontSizeType.XXLarge); return true;

				case "larger": result = new CssFontSize(CssFontSizeType.Larger); return true;
				case "smaller": result = new CssFontSize(CssFontSizeType.Smaller); return true;
			}

			var ptr = new StringPtr(lengthText);
			ptr.AdvanceNumber();

			var value = float.Parse(lengthText.Substring(0, ptr.Index), System.Globalization.CultureInfo.InvariantCulture);
			if (ptr.Index != lengthText.Length)
			{
				var unit = lengthText.Substring(ptr.Index);
				if (presentation) unit = unit.ToLower();
				switch (unit)
				{
					case "%": result = new CssFontSize(CssFontSizeType.Percentage, value); break;
					case "em": result = new CssFontSize(CssFontSizeType.Ems, value); break;
					case "ex": result = new CssFontSize(CssFontSizeType.Exs, value); break;
					case "px": result = new CssFontSize(CssFontSizeType.Pixel, value); break;
					case "cm": result = new CssFontSize(CssFontSizeType.Centimeter, value); break;
					case "mm": result = new CssFontSize(CssFontSizeType.Millimeter, value); break;
					case "in": result = new CssFontSize(CssFontSizeType.Inch, value); break;
					case "pt": result = new CssFontSize(CssFontSizeType.Point, value); break;
					case "pc": result = new CssFontSize(CssFontSizeType.Pica, value); break;
					default: result = null; return false;
				}
				return true;
			}
			result = new CssFontSize(CssFontSizeType.Number, value);
			return true;
		}

		public bool Equals(CssFontSize other) => this.SizeType == other.SizeType && this.Value == other.Value;

		public static implicit operator CssFontSize(float f) => new CssFontSize(CssFontSizeType.Number, f);

		public static implicit operator CssFontSize(string s) => Parse(s);
		public static implicit operator string(CssFontSize a) => a.ValueAsString;

		public static CssFontSize operator *(float left, CssFontSize right) => new CssFontSize(right.SizeType, left * right.Value);
		public static CssFontSize operator *(CssFontSize left, float right) => new CssFontSize(left.SizeType, left.Value * right);
		public static CssFontSize operator /(CssFontSize left, float right) => new CssFontSize(left.SizeType, left.Value / right);

		public static bool operator <(CssFontSize left, CssFontSize right)
		{
			if (left.SizeType == right.SizeType)
			{
				return left.Value < right.Value;
			}
			throw new NotSupportedException();
		}
		public static bool operator <=(CssFontSize left, CssFontSize right)
		{
			if (left.SizeType == right.SizeType)
			{
				return left.Value <= right.Value;
			}
			throw new NotSupportedException();
		}
		public static bool operator >(CssFontSize left, CssFontSize right)
		{
			if (left.SizeType == right.SizeType)
			{
				return left.Value > right.Value;
			}
			throw new NotSupportedException();
		}
		public static bool operator >=(CssFontSize left, CssFontSize right)
		{
			if (left.SizeType == right.SizeType)
			{
				return left.Value >= right.Value;
			}
			throw new NotSupportedException();
		}
	}

}