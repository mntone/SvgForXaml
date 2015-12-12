namespace Mntone.SvgForXaml.Path
{
	public abstract class SvgPathSegmentMoveToBase : SvgPathSegment
	{
		protected internal SvgPathSegmentMoveToBase(float x, float y) { this.X = x; this.Y = y; }

		public float X { get; }
		public float Y { get; }

		public static SvgPathSegmentMoveToBase Create(float x, float y, bool isAbsolute) => isAbsolute
			? (SvgPathSegmentMoveToBase)new SvgPathSegmentMoveToAbsolute(x, y)
			: new SvgPathSegmentMoveToRelative(x, y);
	}

	public sealed class SvgPathSegmentMoveToAbsolute : SvgPathSegmentMoveToBase
	{
		public SvgPathSegmentMoveToAbsolute(float x, float y) : base(x, y) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.MoveToAbsolute;
		public override sealed char PathSegmentTypeAsLetter => 'M';
	}

	public sealed class SvgPathSegmentMoveToRelative : SvgPathSegmentMoveToBase
	{
		public SvgPathSegmentMoveToRelative(float x, float y) : base(x, y) { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.MoveToRelative;
		public override sealed char PathSegmentTypeAsLetter => 'm';
	}
}