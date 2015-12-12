using System;

namespace Mntone.SvgForXaml.Primitives
{
	[System.Diagnostics.DebuggerDisplay("[{this.A} {this.B} {this.C} {this.D} {this.E} {this.F}]")]
	public struct SvgMatrix : IEquatable<SvgMatrix>
	{
		public static SvgMatrix Indentity { get; } = new SvgMatrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);

		internal SvgMatrix(double a, double b, double c, double d, double e, double f)
		{
			this.A = a;
			this.B = b;
			this.C = c;
			this.D = d;
			this.E = e;
			this.F = f;
		}

		public double A { get; }
		public double B { get; }
		public double C { get; }
		public double D { get; }
		public double E { get; }
		public double F { get; }

		public bool Equals(SvgMatrix other)
		{
			return this.A == other.A && this.B == other.B && this.C == other.C
				&& this.D == other.D && this.E == other.E && this.F == other.F;
		}

		public static SvgMatrix operator *(SvgMatrix left, SvgMatrix right)
		{
			return new SvgMatrix(
				left.A * right.A + left.B * right.C,
				left.A * right.B + left.B * right.D,
				left.A * right.C + left.C * right.D,
				left.B * right.C + left.D * right.D,
				left.A * right.E + left.C * right.F + left.E,
				left.B * right.E + left.D * right.F + left.F);
		}
	}
}