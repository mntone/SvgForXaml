namespace Mntone.SvgForXaml.Path
{
	public abstract class SvgPathSegment
	{
		public enum SvgPathSegmentType : ushort
		{
			Unknown = 0,
			ClosePath,
			MoveToAbsolute,
			MoveToRelative,
			LineToAbsolute,
			LineToRelative,
			CurveToCubicAbsolute,
			CurveToCubicRelative,
			CurveToQuadraticAbsolute,
			CurveToQuadraticRelative,
			ArcAbsolute,
			ArcRelative,
			LineToHorizontalAbsolute,
			LineToHorizontalRelative,
			LineToVerticalAbsolute,
			LineToVerticalRelative,
			CurveToCubicSmoothAbsolute,
			CurveToCubicSmoothRelative,
			CurveToQuadraticSmoothAbsolute,
			CurveToQuadraticSmoothRelative,
		}

		protected internal SvgPathSegment() { }

		public abstract SvgPathSegmentType PathSegmentType { get; }
		public abstract char PathSegmentTypeAsLetter { get; }
	}
}