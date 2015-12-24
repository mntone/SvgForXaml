namespace Mntone.SvgForXaml.Path
{
	public sealed class SvgPathSegmentClosePath : SvgPathSegment
	{
		public SvgPathSegmentClosePath() { }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.ClosePath;
		public override sealed char PathSegmentTypeAsLetter => 'z';
	}
}