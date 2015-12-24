using System;

namespace Mntone.SvgForXaml.Primitives
{
	public enum SvgUnitType : ushort
	{
		Unknown = 0,
		UserSpaceOnUse,
		ObjectBoundingBox,
	}

	public static class SvgUnitTypeHelper
	{
		public static SvgUnitType Parse(this string unitTypeText)
		{
			switch (unitTypeText)
			{
				case "userSpaceOnUse": return SvgUnitType.UserSpaceOnUse;
				case "objectBoundingBox": return SvgUnitType.ObjectBoundingBox;
			}
			return SvgUnitType.Unknown;
		}

		public static string AsString(this SvgUnitType unitType)
		{
			switch (unitType)
			{
				case SvgUnitType.UserSpaceOnUse: return "userSpaceOnUse";
				case SvgUnitType.ObjectBoundingBox: return "objectBoundingBox";
			}
			throw new ArgumentException(nameof(unitType));
		}
	}
}