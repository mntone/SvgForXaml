using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Gradients
{
	[System.Diagnostics.DebuggerDisplay("LinearGradient: 1 = ({this.X1}, {this.Y1}), 2 = ({this.X2}, {this.Y2})")]
	public sealed class SvgLinearGradientElement : SvgGradientElement
	{
		internal SvgLinearGradientElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this.X1 = element.ParseCoordinate("x1", 0.0F);
			this.Y1 = element.ParseCoordinate("y1", 0.0F);
			this.X2 = element.ParseCoordinate("x2", 1.0F);
			this.Y2 = element.ParseCoordinate("y2", 0.0F);
		}

		public override string TagName => "linearGradient";
		public SvgLength X1 { get; }
		public SvgLength Y1 { get; }
		public SvgLength X2 { get; }
		public SvgLength Y2 { get; }

		#region ISvgLocatable
		public override SvgRect GetBBox()
		{
			float left, right;
			if (this.X1 < this.X2)
			{
				left = this.X1.ValueAsPixel;
				right = this.X2.ValueAsPixel;
			}
			else
			{
				left = this.X2.ValueAsPixel;
				right = this.X1.ValueAsPixel;
			}

			float top, bottom;
			if (this.Y1 < this.Y2)
			{
				top = this.Y1.ValueAsPixel;
				bottom = this.Y2.ValueAsPixel;
			}
			else
			{
				top = this.Y2.ValueAsPixel;
				bottom = this.Y1.ValueAsPixel;
			}
			return new SvgRect(left, top, right - left, bottom - top);
		}
		#endregion
	}
}