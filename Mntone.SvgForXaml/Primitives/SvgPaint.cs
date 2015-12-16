namespace Mntone.SvgForXaml.Primitives
{
	public sealed class SvgPaint : SvgColor
	{
		public SvgPaint(string paint)
		{
			if (paint.StartsWith("url("))
			{
				this.PaintType = SvgPaintType.Uri;
				this.Uri = paint.Substring(4, paint.Length - 5);
			}
			else if (paint == "none")
			{
				this.PaintType = SvgPaintType.None;
			}
			else if (paint == "currentColor")
			{
				this.PaintType = SvgPaintType.CurrentColor;
			}
			else
			{
				this.PaintType = SvgPaintType.RgbColor;
				this.RgbColor = Parse(paint);
			}
		}

		public SvgPaintType PaintType { get; }
		public string Uri { get; }
	}
}