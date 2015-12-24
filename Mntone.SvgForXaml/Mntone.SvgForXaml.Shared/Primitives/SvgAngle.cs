using Mntone.SvgForXaml.Internal;
using System;

namespace Mntone.SvgForXaml.Primitives
{
	[System.Diagnostics.DebuggerDisplay("{this.ValueAsString} ({this.ValueAsDegree}deg)")]
	public struct SvgAngle : IEquatable<SvgAngle>
	{
		public enum SvgAngleType : ushort { Unknown = 0, Unspecified, Degree, Radian, Grade };

		internal SvgAngle(SvgAngleType unitType, float value)
		{
			this.UnitType = unitType;
			this.Value = value;
		}

		public SvgAngleType UnitType { get; }
		public float Value { get; }

		public float ValueAsDegree
		{
			get
			{
				switch (this.UnitType)
				{
					case SvgAngleType.Unspecified:
					case SvgAngleType.Degree:
						return this.Value;

					case SvgAngleType.Radian:
						return 180.0F * this.Value / (float)Math.PI;

					case SvgAngleType.Grade:
						return 180.0F * this.Value / 200.0F;
				}
				throw new InvalidOperationException();
			}
		}

		public float ValueAsRadian
		{
			get
			{
				switch (this.UnitType)
				{
					case SvgAngleType.Unspecified:
					case SvgAngleType.Degree:
						return (float)Math.PI * this.Value / 180.0F;

					case SvgAngleType.Radian:
						return this.Value;

					case SvgAngleType.Grade:
						return (float)Math.PI * this.Value / 200.0F;
				}
				throw new InvalidOperationException();
			}
		}

		public string ValueAsString
		{
			get
			{
				switch (this.UnitType)
				{
					case SvgAngleType.Unspecified:
					case SvgAngleType.Degree:
						return $"{this.Value}deg";

					case SvgAngleType.Radian:
						return $"{this.Value}rad";

					case SvgAngleType.Grade:
						return $"{this.Value}grad";
				}
				throw new InvalidOperationException();
			}
		}

		internal static SvgAngle ParseInCss(string angleText)
		{
			var ptr = new StringPtr(angleText);
			ptr.AdvanceNumber();

			var value = float.Parse(angleText.Substring(0, ptr.Index), System.Globalization.CultureInfo.InvariantCulture);
			if (ptr.Index != angleText.Length)
			{
				var unit = angleText.Substring(ptr.Index).ToLower();
				switch (unit)
				{
					case "deg": return new SvgAngle(SvgAngleType.Degree, value);
					case "rad": return new SvgAngle(SvgAngleType.Radian, value);
					case "grad": return new SvgAngle(SvgAngleType.Grade, value);
				}
				throw new ArgumentException(nameof(angleText));
			}
			return new SvgAngle(SvgAngleType.Degree, value);
		}

		internal static SvgAngle Parse(string angleText)
		{
			var ptr = new StringPtr(angleText);
			ptr.AdvanceNumber();

			var value = float.Parse(angleText.Substring(0, ptr.Index), System.Globalization.CultureInfo.InvariantCulture);
			if (ptr.Index != angleText.Length)
			{
				var unit = angleText.Substring(ptr.Index);
				switch (unit)
				{
					case "deg": return new SvgAngle(SvgAngleType.Degree, value);
					case "rad": return new SvgAngle(SvgAngleType.Radian, value);
					case "grad": return new SvgAngle(SvgAngleType.Grade, value);
				}
				throw new ArgumentException(nameof(angleText));
			}
			return new SvgAngle(SvgAngleType.Degree, value);
		}

		public bool Equals(SvgAngle other) => this.UnitType == other.UnitType && this.Value == other.Value;

		public static implicit operator SvgAngle(float f) => new SvgAngle(SvgAngleType.Degree, f);

		public static implicit operator SvgAngle(string s) => Parse(s);
		public static implicit operator string(SvgAngle a) => a.ValueAsString;
	}
}