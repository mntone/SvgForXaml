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
				return (float)this.CanvasSize.X * length.Value / 100.0F;
			}
			return this.Convert(length);
		}

		public float ConvertY(SvgLength length)
		{
			if (length.UnitType == SvgLength.SvgLengthType.Percentage)
			{
				return (float)this.CanvasSize.Y * length.Value / 100.0F;
			}
			return this.Convert(length);
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