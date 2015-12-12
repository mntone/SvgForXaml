using System;
using System.Linq;

namespace Mntone.SvgForXaml.Primitives
{
	[System.Diagnostics.DebuggerDisplay("({this.X}, {this.Y})")]
	public struct SvgPoint : IEquatable<SvgPoint>
	{
		internal SvgPoint(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		public double X { get; }
		public double Y { get; }

		internal static SvgPoint Parse(string attributeValue)
		{
			var s = attributeValue.Split(new[] { ',' }).Select(t => double.Parse(t)).ToArray();
			return new SvgPoint(s[0], s[1]);
		}

		public bool Equals(SvgPoint other) => this.X == other.X && this.Y == other.Y;
	}
}