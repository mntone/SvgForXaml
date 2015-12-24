using System;

namespace Mntone.SvgForXaml.Gradients
{
	public enum SvgSpreadMethodType : ushort
	{
		Unknown = 0,
		Pad,
		Reflect,
		Repeat,
	}

	public static class SvgSpreadMethodTypeHelper
	{
		public static SvgSpreadMethodType Parse(this string spreadMethodTypeText)
		{
			switch (spreadMethodTypeText)
			{
				case "pad": return SvgSpreadMethodType.Pad;
				case "reflect": return SvgSpreadMethodType.Repeat;
				case "repeat": return SvgSpreadMethodType.Repeat;
			}
			return SvgSpreadMethodType.Unknown;
		}

		public static string AsString(this SvgSpreadMethodType spreadMethodType)
		{
			switch (spreadMethodType)
			{
				case SvgSpreadMethodType.Pad: return "pad";
				case SvgSpreadMethodType.Reflect: return "reflect";
				case SvgSpreadMethodType.Repeat: return "repeat";
			}
			throw new ArgumentException(nameof(spreadMethodType));
		}
	}
}