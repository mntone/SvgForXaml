using System;

namespace Mntone.SvgForXaml.Path
{
	public abstract class SvgPathSegmentArcBase : SvgPathSegment
	{
		protected internal SvgPathSegmentArcBase(float x, float y, float r1, float r2, float angle, bool largeArcFlag, bool sweepFlag)
		{
			this.X = x;
			this.Y = y;
			this.RadiusX = r1;
			this.RadiusY = r2;
			this.Angle = angle;
			this.LargeArcFlag = largeArcFlag;
			this.SweepFlag = sweepFlag;
		}

		public float X { get; }
		public float Y { get; }
		public float RadiusX { get; }
		public float RadiusY { get; }
		public float Angle { get; }
		public bool LargeArcFlag { get; }
		public bool SweepFlag { get; }

		public static SvgPathSegmentArcBase Create(float x, float y, float radiusX, float radiusY, float angle, bool largeArcFlag, bool sweepFlag, bool isAbsolute) => isAbsolute
			? (SvgPathSegmentArcBase)new SvgPathSegmentArcAbsolute(x, y, radiusX, radiusY, angle, largeArcFlag, sweepFlag)
			: new SvgPathSegmentArcRelative(x, y, radiusX, radiusY, angle, largeArcFlag, sweepFlag);
	}

	public sealed class SvgPathSegmentArcAbsolute : SvgPathSegmentArcBase
	{
		public SvgPathSegmentArcAbsolute(float x, float y, float r1, float r2, float angle, bool largeArcFlag, bool sweepFlag)
			: base(x, y, r1, r2, angle, largeArcFlag, sweepFlag)
		{ }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.ArcAbsolute;
		public override sealed char PathSegmentTypeAsLetter => 'A';
	}

	public sealed class SvgPathSegmentArcRelative : SvgPathSegmentArcBase
	{
		public SvgPathSegmentArcRelative(float x, float y, float r1, float r2, float angle, bool largeArcFlag, bool sweepFlag)
			: base(x, y, r1, r2, angle, largeArcFlag, sweepFlag)
		{ }

		public override SvgPathSegmentType PathSegmentType => SvgPathSegmentType.ArcRelative;
		public override sealed char PathSegmentTypeAsLetter => 'a';
	}
}