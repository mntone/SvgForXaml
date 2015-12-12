namespace Mntone.SvgForXaml.Primitives
{
	[System.Diagnostics.DebuggerDisplay("RgbColor: Red = {this.Red}, Green = {this.Green}, Blue = {this.Blue}")]
	public struct RgbColor
	{
		internal RgbColor(byte red, byte green, byte blue)
		{
			this.Red = red;
			this.Green = green;
			this.Blue = blue;
		}

		public byte Red { get; }
		public byte Green { get; }
		public byte Blue { get; }
	}
}