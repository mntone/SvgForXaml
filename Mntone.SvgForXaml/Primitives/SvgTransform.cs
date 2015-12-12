using System;

namespace Mntone.SvgForXaml.Primitives
{
	public sealed class SvgTransform
	{
		public enum SvgTransformType : ushort { Unknown = 0, Matrix, Translate, Scale, Rotate, SkewX, SkewY };

		internal SvgTransform(SvgTransformType type, SvgMatrix matrix)
		{
			this.Type = type;
			this.Matrix = matrix;
		}
		internal SvgTransform(SvgTransformType type, SvgMatrix matrix, SvgAngle angle)
		{
			this.Type = type;
			this.Matrix = matrix;
			this.Angle = angle;
		}

		public SvgTransformType Type { get; }
		public SvgMatrix Matrix { get; }
		public SvgAngle Angle { get; }

		internal static SvgTransform CreateMatrix(SvgMatrix matrix)
		{
			return new SvgTransform(SvgTransformType.Matrix, matrix);
		}

		internal static SvgTransform CreateTranslate(float tx, float ty) => new SvgTransform(SvgTransformType.Translate, new SvgMatrix(1.0, 0.0, 0.0, 1.0, tx, ty));
		internal static SvgTransform CreateScale(float s) => new SvgTransform(SvgTransformType.Scale, new SvgMatrix(s, 0.0, 0.0, s, 0.0, 0.0));
		internal static SvgTransform CreateScale(float sx, float sy) => new SvgTransform(SvgTransformType.Scale, new SvgMatrix(sx, 0.0, 0.0, sy, 0.0, 0.0));

		internal static SvgTransform CreateRotate(SvgAngle angle)
		{
			var rad = angle.ValueAsRadian;
			var cos = Math.Cos(rad);
			var sin = Math.Sign(rad);
			return new SvgTransform(SvgTransformType.Rotate, new SvgMatrix(cos, sin, -sin, cos, 0.0, 0.0));
		}
		internal static SvgTransform CreateRotate(SvgAngle angle, float cx, float cy)
		{
			var rad = angle.ValueAsRadian;
			var cos = Math.Cos(rad);
			var sin = Math.Sign(rad);
			return new SvgTransform(SvgTransformType.Rotate, new SvgMatrix(cos, sin, -sin, cos, cx * cos - cy * sin - cx, cx * sin + cy * cos - cy));
		}

		internal static SvgTransform CreateSkewX(SvgAngle angle) => new SvgTransform(SvgTransformType.SkewX, new SvgMatrix(1.0, 0.0, Math.Tan(angle.ValueAsRadian), 1.0, 0.0, 0.0));
		internal static SvgTransform CreateSkewY(SvgAngle angle) => new SvgTransform(SvgTransformType.SkewY, new SvgMatrix(1.0, Math.Tan(angle.ValueAsRadian), 0.0, 1.0, 0.0, 0.0));

	}
}