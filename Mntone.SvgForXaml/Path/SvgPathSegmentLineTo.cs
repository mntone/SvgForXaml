namespace Mntone.SvgForXaml.Path
{
	public abstract class SvgPathSegmentLineToBase : SvgPathSegment
	{
		protected internal SvgPathSegmentLineToBase(float x, float y) { this.X = x; this.Y = y; }

		public float X { get; }
		public float Y { get; }

		public static SvgPathSegmentLineToBase Create(float x, float y, bool isAbsolute) => isAbsolute
			? (SvgPathSegmentLineToBase)new SvgPathSegmentLineToAbsolute(x, y)
			: new SvgPathSegmentLineToRelative(x, y);
	}

	public sealed class SvgPathSegmentLineToAbsolute : SvgPathSegmentLineToBase
	{
		public SvgPathSegmentLineToAbsolute(float x, float y) : base(x, y) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.LineToAbsolute;
		public override sealed char PathSegmentTypeAsLetter => 'L';
	}

	public sealed class SvgPathSegmentLineToRelative : SvgPathSegmentLineToBase
	{
		public SvgPathSegmentLineToRelative(float x, float y) : base(x, y) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.LineToRelative;
		public override sealed char PathSegmentTypeAsLetter => 'l';
	}
}