namespace Mntone.SvgForXaml.Path
{
	public abstract class SvgPathSegmentCurveToQuadraticBase : SvgPathSegment
	{
		protected internal SvgPathSegmentCurveToQuadraticBase(float x1, float y1, float x, float y)
		{
			this.X1 = x1;
			this.Y1 = y1;
			this.X = x;
			this.Y = y;
		}

		public float X1 { get; }
		public float Y1 { get; }
		public float X { get; }
		public float Y { get; }

		public static SvgPathSegmentCurveToQuadraticBase Create(float x1, float y1, float x, float y, bool isAbsolute) => isAbsolute
			? (SvgPathSegmentCurveToQuadraticBase)new SvgPathSegmentCurveToQuadraticAbsolute(x1, y1, x, y)
			: new SvgPathSegmentCurveToQuadraticRelative(x1, y1, x, y);
	}

	[System.Diagnostics.DebuggerDisplay("Q{this.X1},{this.Y1} {this.X},{this.Y}")]
	public sealed class SvgPathSegmentCurveToQuadraticAbsolute : SvgPathSegmentCurveToQuadraticBase
	{
		public SvgPathSegmentCurveToQuadraticAbsolute(float x1, float y1, float x, float y) : base(x1, y1, x, y) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.CurveToQuadraticAbsolute;
		public override sealed char PathSegmentTypeAsLetter => 'Q';
	}

	[System.Diagnostics.DebuggerDisplay("q{this.X1},{this.Y1} {this.X},{this.Y}")]
	public sealed class SvgPathSegmentCurveToQuadraticRelative : SvgPathSegmentCurveToQuadraticBase
	{
		public SvgPathSegmentCurveToQuadraticRelative(float x1, float y1, float x, float y) : base(x1, y1, x, y) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.CurveToQuadraticRelative;
		public override sealed char PathSegmentTypeAsLetter => 'q';
	}
}