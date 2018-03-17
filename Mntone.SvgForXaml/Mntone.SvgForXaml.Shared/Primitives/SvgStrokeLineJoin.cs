using Mntone.SvgForXaml.Interfaces;

namespace Mntone.SvgForXaml.Primitives
{
	[System.Diagnostics.DebuggerDisplay("StrokeLineJoin: {this.Value}")]
	public struct SvgStrokeLineJoin : ICssValue
	{
		public SvgStrokeLineJoin(string strokeLineJoin)
		{
			switch (strokeLineJoin.ToLower())
			{
				case "butt":
					this.Value = SvgStrokeLineJoinType.Miter;
					break;
				case "round":
					this.Value = SvgStrokeLineJoinType.Round;
					break;
				case "bevel":
					this.Value = SvgStrokeLineJoinType.Bevel;
					break;
				default:
					this.Value = SvgStrokeLineJoinType.Unknown;
					break;
			}
		}

		public SvgStrokeLineJoinType Value { get; }
	}
}