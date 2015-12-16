using Mntone.SvgForXaml.Primitives;
using System.Xml;

namespace Mntone.SvgForXaml.Internal
{
	internal sealed class SvgTransformableHelper
	{
		public SvgTransformableHelper(XmlElement element)
		{
			this.Transform = SvgTransformParser.Parse(element.GetAttribute("transform"));
		}

		public SvgTransformCollection Transform { get; }
	}
}