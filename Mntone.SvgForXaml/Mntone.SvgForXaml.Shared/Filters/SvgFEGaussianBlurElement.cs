using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Primitives;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Shared.Filters
{
	public sealed class SvgFEGaussianBlurElement : SvgElement
	{
		internal SvgFEGaussianBlurElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this.In1 = element.GetAttribute("in1");

			var stdDeviation = SvgPoint.Parse(element.GetAttribute("stdDeviation"));
			this.StdDeviationX = stdDeviation.X;
			this.StdDeviationY = stdDeviation.Y;
		}

		public void SetStdDeviation(SvgNumber stdDeviationX, SvgNumber stdDeviationY)
		{
			this.StdDeviationX = stdDeviationX;
			this.StdDeviationY = stdDeviationY;
		}

		public override string TagName => "feGaussianBlur";
		public string In1 { get; }
		public SvgNumber StdDeviationX { get; private set; }
		public SvgNumber StdDeviationY { get; private set; }
	}
}
