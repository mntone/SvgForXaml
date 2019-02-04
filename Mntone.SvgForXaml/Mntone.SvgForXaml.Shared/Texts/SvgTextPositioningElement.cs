using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Primitives;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Texts
{
	public abstract class SvgTextPositioningElement : SvgTextContentElement
	{
		public SvgTextPositioningElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this.X = new SvgLengthCollection(element.GetAttribute("x"));
			this.Y = new SvgLengthCollection(element.GetAttribute("y"));
			this.DX = new SvgLengthCollection(element.GetAttribute("dx"));
			this.DY = new SvgLengthCollection(element.GetAttribute("dy"));
		}

		public SvgLengthCollection X { get; }
		public SvgLengthCollection Y { get; }
		public SvgLengthCollection DX { get; }
		public SvgLengthCollection DY { get; }
	}
}
