using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using System.Xml;

namespace Mntone.SvgForXaml.Gradients
{
	public abstract class SvgGradientElement : SvgElement, ISvgStylable
	{
		protected internal SvgGradientElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(element);

			var gradientUnits = SvgUnitTypeHelper.Parse(element.GetAttribute("gradientUnits"));
			if (gradientUnits == SvgUnitType.Unknown) gradientUnits = SvgUnitType.ObjectBoundingBox;
			this.GradientUnits = gradientUnits;

			this.GradientTransform = SvgTransformParser.Parse(element.GetAttribute("gradientTransform"));

			var spreadMethod = SvgSpreadMethodTypeHelper.Parse(element.GetAttribute("spreadMethod"));
			if (spreadMethod == SvgSpreadMethodType.Unknown) spreadMethod = SvgSpreadMethodType.Pad;
			this.SpreadMethod = spreadMethod;
		}

		public SvgUnitType GradientUnits { get; }
		public SvgTransformCollection GradientTransform { get; }
		public SvgSpreadMethodType SpreadMethod { get; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private readonly SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion
	}
}