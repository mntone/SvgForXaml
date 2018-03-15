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
	}
}