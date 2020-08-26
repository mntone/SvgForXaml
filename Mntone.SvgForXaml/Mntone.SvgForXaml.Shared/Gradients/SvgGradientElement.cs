using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using System;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Gradients
{
	public abstract class SvgGradientElement : SvgElement, ISvgStylable, ISvgTransformable
	{
		protected internal SvgGradientElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(this, element);
			this._transformableHelper = new SvgTransformableHelper(element);

			this.GradientUnits = SvgUnitTypeHelper.Parse(element.GetAttribute("gradientUnits"), SvgUnitType.ObjectBoundingBox);

			this.GradientTransform = SvgTransformParser.Parse(element.GetAttribute("gradientTransform"));

			var spreadMethod = SvgSpreadMethodTypeHelper.Parse(element.GetAttribute("spreadMethod"));
			if (spreadMethod == SvgSpreadMethodType.Unknown) spreadMethod = SvgSpreadMethodType.Pad;
			this.SpreadMethod = spreadMethod;
		}

		protected override void DeepCopy(SvgElement element)
		{
			var casted = (SvgGradientElement)element;
			casted._stylableHelper = this._stylableHelper.DeepCopy(casted);
			casted._transformableHelper = this._transformableHelper.DeepCopy();
		}

		public SvgUnitType GradientUnits { get; }
		public SvgTransformCollection GradientTransform { get; }
		public SvgSpreadMethodType SpreadMethod { get; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion

		#region ISvgLocatable
		public virtual SvgRect GetBBox()
		{
			throw new NotSupportedException();
		}
		#endregion

		#region ISvgTransformable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private SvgTransformableHelper _transformableHelper;
		public SvgTransformCollection Transform => this._transformableHelper.Transform;
		#endregion
	}
}
