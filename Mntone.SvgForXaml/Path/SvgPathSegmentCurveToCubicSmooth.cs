namespace Mntone.SvgForXaml.Path
{
	public abstract class SvgPathSegmentCurveToCubicSmoothBase : SvgPathSegment
	{
		protected internal SvgPathSegmentCurveToCubicSmoothBase(float x2, float y2, float x, float y)
		{
			this.X2 = x2;
			this.Y2 = y2;
			this.X = x;
			this.Y = y;
		}

		public float X2 { get; }
		public float Y2 { get; }
		public float X { get; }
		public float Y { get; }

		public static SvgPathSegmentCurveToCubicSmoothBase Create(float x2, float y2, float x, float y, bool isAbsolute) => isAbsolute
			? (SvgPathSegmentCurveToCubicSmoothBase)new SvgPathSegmentCurveToCubicSmoothAbsolute(x2, y2, x, y)
			: new SvgPathSegmentCurveToCubicSmoothRelative(x2, y2, x, y);
	}

	public sealed class SvgPathSegmentCurveToCubicSmoothAbsolute : SvgPathSegmentCurveToCubicSmoothBase
	{
		public SvgPathSegmentCurveToCubicSmoothAbsolute(float x2, float y2, float x, float y) : base(x2, y2, x, y) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.CurveToCubicSmoothAbsolute;
		public override sealed char PathSegmentTypeAsLetter => 'S';
	}

	public sealed class SvgPathSegmentCurveToCubicSmoothRelative : SvgPathSegmentCurveToCubicSmoothBase
	{
		public SvgPathSegmentCurveToCubicSmoothRelative(float x2, float y2, float x, float y) : base(x2, y2, x, y) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.CurveToCubicSmoothRelative;
		public override sealed char PathSegmentTypeAsLetter => 's';
	}
}