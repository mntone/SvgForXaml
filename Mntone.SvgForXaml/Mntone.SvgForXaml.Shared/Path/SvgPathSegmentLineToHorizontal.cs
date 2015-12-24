namespace Mntone.SvgForXaml.Path
{
	public abstract class SvgPathSegmentLineToHorizontalBase : SvgPathSegment
	{
		protected internal SvgPathSegmentLineToHorizontalBase(float x) { this.X = x; }

		public float X { get; }

		public static SvgPathSegmentLineToHorizontalBase Create(float x, bool isAbsolute) => isAbsolute
			? (SvgPathSegmentLineToHorizontalBase)new SvgPathSegmentLineToHorizontalAbsolute(x)
			: new SvgPathSegmentLineToHorizontalRelative(x);
	}

	public sealed class SvgPathSegmentLineToHorizontalAbsolute : SvgPathSegmentLineToHorizontalBase
	{
		public SvgPathSegmentLineToHorizontalAbsolute(float x) : base(x) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.LineToHorizontalAbsolute;
		public override sealed char PathSegmentTypeAsLetter => 'H';
	}

	public sealed class SvgPathSegmentLineToHorizontalRelative : SvgPathSegmentLineToHorizontalBase
	{
		public SvgPathSegmentLineToHorizontalRelative(float x) : base(x) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.LineToHorizontalRelative;
		public override sealed char PathSegmentTypeAsLetter => 'h';
	}
}