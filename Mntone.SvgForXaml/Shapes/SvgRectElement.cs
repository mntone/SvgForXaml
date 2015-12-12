using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using System.Xml;

namespace Mntone.SvgForXaml.Shapes
{
	[System.Diagnostics.DebuggerDisplay("Rect: X = {this.X}, Y = {this.Y}, Width = {this.Width}, Height = {this.Height}, RoundedX = {this.RoundedX}, RounedY = {this.RoundedY}")]
	public sealed class SvgRectElement : SvgElement, ISvgStylable
	{
		internal SvgRectElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(element);

			this.X = element.ParseCoordinate("x", 0.0F);
			this.Y = element.ParseCoordinate("y", 0.0F);
			this.Width = element.ParseLength("width", 0.0F);
			this.Height = element.ParseLength("height", 0.0F);

			SvgLength rx, ry;
			var fx = element.TryParseLength("rx", out rx);
			if (fx && rx < 0.0F) fx = false;

			var fy = element.TryParseLength("ry", out ry);
			if (fy && ry < 0.0F) fy = false;

			if (!fx && !fy)
			{
				rx = 0.0F;
				ry = 0.0F;
			}
			else if (fx && !fy)
			{
				ry = rx;
			}
			else if (!fx && fy)
			{
				rx = ry;
			}

			var hw = this.Width / 2.0F;
			if (rx > hw) rx = hw;

			var hh = this.Height / 2.0F;
			if (ry > hh) ry = hh;

			this.RoundedX = rx;
			this.RoundedY = ry;
		}

		public override string TagName => "rect";
		public SvgLength X { get; }
		public SvgLength Y { get; }
		public SvgLength Width { get; }
		public SvgLength Height { get; }
		public SvgLength RoundedX { get; }
		public SvgLength RoundedY { get; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private readonly SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion
	}
}