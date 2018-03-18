namespace Mntone.SvgForXaml.Primitives
{
	public sealed class SvgStopColor : SvgColor
	{
		public SvgStopColor(string paint)
		{
			if (string.Compare(paint, "currentColor", System.StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.StopColorType = SvgStopColorType.CurrentColor;
			}
			else
			{
				this.StopColorType = SvgStopColorType.RgbColor;
				this.RgbColor = Parse(paint);
			}
		}

		public SvgStopColorType StopColorType { get; }
	}
}