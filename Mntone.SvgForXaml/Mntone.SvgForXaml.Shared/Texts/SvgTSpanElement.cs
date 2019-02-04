using Mntone.SvgForXaml.Interfaces;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Texts
{
	public sealed class SvgTSpanElement : SvgTextPositioningElement
	{
		public SvgTSpanElement(INode parent, XmlElement element)
			: base(parent, element)
		{ }

		public override string TagName => "tspan";
	}
}
