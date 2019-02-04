using System;

namespace Mntone.SvgForXaml.Texts
{
	public enum LengthAdjustType : ushort
	{
		Unknown = 0,
		Spacing,
		SpacingAndGlyphs,
	}

	public static class LengthAdjustTypeHelper
	{
		public static LengthAdjustType Parse(this string lengthAdjustTypeText)
		{
			switch (lengthAdjustTypeText)
			{
				case "spacing": return LengthAdjustType.Spacing;
				case "spacingAndGlyphs": return LengthAdjustType.SpacingAndGlyphs;
			}
			return LengthAdjustType.Unknown;
		}

		public static string AsString(this LengthAdjustType lengthAdjustType)
		{
			switch (lengthAdjustType)
			{
				case LengthAdjustType.Spacing: return "spacing";
				case LengthAdjustType.SpacingAndGlyphs: return "spacingAndGlyphs";
			}
			throw new ArgumentException(nameof(lengthAdjustType));
		}
	}
}
