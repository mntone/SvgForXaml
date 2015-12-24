using Mntone.SvgForXaml.Primitives;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Internal
{
	internal sealed class SvgTransformableHelper
	{
		internal SvgTransformableHelper(SvgTransformCollection transform)
		{
			this.Transform = transform;
		}
		public SvgTransformableHelper(XmlElement element)
		{
			this.Transform = SvgTransformParser.Parse(element.GetAttribute("transform"));
		}

		internal SvgTransformableHelper DeepCopy()
		{
			return new SvgTransformableHelper(this.Transform.DeepCopy());
		}

		public SvgTransformCollection Transform { get; }
	}
}