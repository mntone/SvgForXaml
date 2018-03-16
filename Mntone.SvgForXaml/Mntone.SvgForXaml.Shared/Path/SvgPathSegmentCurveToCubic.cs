namespace Mntone.SvgForXaml.Path
{
	public abstract class SvgPathSegmentCurveToCubicBase : SvgPathSegment
	{
		protected internal SvgPathSegmentCurveToCubicBase(float x1, float y1, float x2, float y2, float x, float y)
		{
			this.X1 = x1;
			this.Y1 = y1;
			this.X2 = x2;
			this.Y2 = y2;
			this.X = x;
			this.Y = y;
		}

		public float X1 { get; }
		public float Y1 { get; }
		public float X2 { get; }
		public float Y2 { get; }
		public float X { get; }
		public float Y { get; }

		public static SvgPathSegmentCurveToCubicBase Create(float x1, float y1, float x2, float y2, float x, float y, bool isAbsolute) => isAbsolute
			? (SvgPathSegmentCurveToCubicBase)new SvgPathSegmentCurveToCubicAbsolute(x1, y1, x2, y2, x, y)
			: new SvgPathSegmentCurveToCubicRelative(x1, y1, x2, y2, x, y);
	}

	[System.Diagnostics.DebuggerDisplay("C{this.X1},{this.Y1} {this.X2},{this.Y2} {this.X},{this.Y}")]
	public sealed class SvgPathSegmentCurveToCubicAbsolute : SvgPathSegmentCurveToCubicBase
	{
		public SvgPathSegmentCurveToCubicAbsolute(float x1, float y1, float x2, float y2, float x, float y)
			: base(x1, y1, x2, y2, x, y)
		{ }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.CurveToCubicAbsolute;
		public override sealed char PathSegmentTypeAsLetter => 'C';
	}

	[System.Diagnostics.DebuggerDisplay("c{this.X1},{this.Y1} {this.X2},{this.Y2} {this.X},{this.Y}")]
	public sealed class SvgPathSegmentCurveToCubicRelative : SvgPathSegmentCurveToCubicBase
	{
		public SvgPathSegmentCurveToCubicRelative(float x1, float y1, float x2, float y2, float x, float y)
			: base(x1, y1, x2, y2, x, y)
		{ }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.CurveToCubicRelative;
		public override sealed char PathSegmentTypeAsLetter => 'c';
	}
}