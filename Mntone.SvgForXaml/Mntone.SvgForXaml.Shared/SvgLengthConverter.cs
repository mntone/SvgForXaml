using Mntone.SvgForXaml.Primitives;
using System;

namespace Mntone.SvgForXaml
{
	public sealed class SvgLengthConverter
	{
		public SvgPoint CanvasSize { get; internal set; }

		public float ConvertX(SvgLength length)
		{
			if (length.UnitType == SvgLength.SvgLengthType.Percentage)
			{
				return this.CanvasSize.X * length.Value / 100.0F;
			}
			return this.Convert(length);
		}

		// Convert X for Object bounding box units [number 0-1 or percentage]
		public float ConvertXForOBBU(SvgLength length, float x, float width)
		{
			if (length.UnitType == SvgLength.SvgLengthType.Number)
			{
				return x + width * length.Value;
			}
			if (length.UnitType == SvgLength.SvgLengthType.Percentage)
			{
				return x + width * length.Value / 100.0F;
			}
			throw new InvalidOperationException();
		}
		
		public float ConvertY(SvgLength length)
		{
			if (length.UnitType == SvgLength.SvgLengthType.Percentage)
			{
				return this.CanvasSize.Y * length.Value / 100.0F;
			}
			return this.Convert(length);
		}

		// Convert Y for Object bounding box units [number 0-1 or percentage]
		public float ConvertYForOBBU(SvgLength length, float y, float height)
		{
			if (length.UnitType == SvgLength.SvgLengthType.Number)
			{
				return y + height * length.Value;
			}
			if (length.UnitType == SvgLength.SvgLengthType.Percentage)
			{
				return y + height * length.Value / 100.0F;
			}
			throw new InvalidOperationException();
		}

		public float Convert(SvgLength length)
		{
			switch (length.UnitType)
			{
				case SvgLength.SvgLengthType.Percentage:
				case SvgLength.SvgLengthType.Ems:
				case SvgLength.SvgLengthType.Exs:
					throw new NotSupportedException();
			}
			return length.ValueAsPixel;
		}
	}
}