using System;
using System.Linq;

namespace Mntone.SvgForXaml.Primitives
{
	[System.Diagnostics.DebuggerDisplay("({this.X}, {this.Y})")]
	public struct SvgPoint : IEquatable<SvgPoint>
	{
		internal SvgPoint(float x, float y)
		{
			this.X = x;
			this.Y = y;
		}

		public float X { get; }
		public float Y { get; }

		internal static SvgPoint Parse(string attributeValue)
		{
			var s = attributeValue.Split(new[] { ',' }).Select(t => float.Parse(t, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
			return new SvgPoint(s[0], s[1]);
		}

		public bool Equals(SvgPoint other) => this.X == other.X && this.Y == other.Y;
	}
}