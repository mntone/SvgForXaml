namespace Mntone.SvgForXaml.Path
{
	public abstract class SvgPathSegmentCurveToQuadraticSmoothBase : SvgPathSegment
	{
		protected internal SvgPathSegmentCurveToQuadraticSmoothBase(float x, float y) { this.X = x; this.Y = y; }

		public float X { get; }
		public float Y { get; }

		public static SvgPathSegmentCurveToQuadraticSmoothBase Create(float x, float y, bool isAbsolute) => isAbsolute
			? (SvgPathSegmentCurveToQuadraticSmoothBase)new SvgPathSegmentCurveToQuadraticSmoothAbsolute(x, y)
			: new SvgPathSegmentCurveToQuadraticSmoothRelative(x, y);
	}

	[System.Diagnostics.DebuggerDisplay("T{this.X},{this.Y}")]
	public sealed class SvgPathSegmentCurveToQuadraticSmoothAbsolute : SvgPathSegmentCurveToQuadraticSmoothBase
	{
		public SvgPathSegmentCurveToQuadraticSmoothAbsolute(float x, float y) : base(x, y) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.CurveToQuadraticSmoothAbsolute;
		public override sealed char PathSegmentTypeAsLetter => 'T';
	}

	[System.Diagnostics.DebuggerDisplay("t{this.X},{this.Y}")]
	public sealed class SvgPathSegmentCurveToQuadraticSmoothRelative : SvgPathSegmentCurveToQuadraticSmoothBase
	{
		public SvgPathSegmentCurveToQuadraticSmoothRelative(float x, float y) : base(x, y) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.CurveToQuadraticSmoothRelative;
		public override sealed char PathSegmentTypeAsLetter => 't';
	}
}