using Mntone.SvgForXaml.Interfaces;

namespace Mntone.SvgForXaml.Primitives
{
	[System.Diagnostics.DebuggerDisplay("StrokeLineCap: {this.Value}")]
	public struct SvgStrokeLineCap : ICssValue
	{
		public SvgStrokeLineCap(string strokeLineCap)
		{
			switch (strokeLineCap.ToLower())
			{
				case "butt":
					this.Value = SvgStrokeLineCapType.Butt;
					break;
				case "round":
					this.Value = SvgStrokeLineCapType.Round;
					break;
				case "square":
					this.Value = SvgStrokeLineCapType.Square;
					break;
				default:
					this.Value = SvgStrokeLineCapType.Unknown;
					break;
			}
		}

		public SvgStrokeLineCapType Value { get; }
	}
}
