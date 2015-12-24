namespace Mntone.SvgForXaml.Path
{
	public abstract class SvgPathSegmentLineToVerticalBase : SvgPathSegment
	{
		protected internal SvgPathSegmentLineToVerticalBase(float y) { this.Y = y; }

		public float Y { get; }

		public static SvgPathSegmentLineToVerticalBase Create(float y, bool isAbsolute) => isAbsolute
			? (SvgPathSegmentLineToVerticalBase)new SvgPathSegmentLineToVerticalAbsolute(y)
			: new SvgPathSegmentLineToVerticalRelative(y);
	}

	public sealed class SvgPathSegmentLineToVerticalAbsolute : SvgPathSegmentLineToVerticalBase
	{
		public SvgPathSegmentLineToVerticalAbsolute(float y) : base(y) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.LineToVerticalAbsolute;
		public override sealed char PathSegmentTypeAsLetter => 'V';
	}

	public sealed class SvgPathSegmentLineToVerticalRelative : SvgPathSegmentLineToVerticalBase
	{
		public SvgPathSegmentLineToVerticalRelative(float y) : base(y) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.LineToVerticalRelative;
		public override sealed char PathSegmentTypeAsLetter => 'v';
	}
}