using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Maskings
{
	public sealed class SvgClipPathElement : SvgElement, ISvgStylable, ISvgTransformable
	{
		internal SvgClipPathElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(this, element);
			this._transformableHelper = new SvgTransformableHelper(element);

			var clipPathUnits = SvgUnitTypeHelper.Parse(element.GetAttribute("clipPathUnits"));
			if (clipPathUnits == SvgUnitType.Unknown) clipPathUnits = SvgUnitType.UserSpaceOnUse;
			this.ClipPathUnits = clipPathUnits;
		}

		protected override void DeepCopy(SvgElement element)
		{
			var casted = (SvgClipPathElement)element;
			casted._stylableHelper = this._stylableHelper.DeepCopy(casted);
		}

		public override string TagName => "clipPath";
		public SvgUnitType ClipPathUnits { get; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion
	
		#region ISvgTransformable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private SvgTransformableHelper _transformableHelper;
		public SvgTransformCollection Transform => this._transformableHelper.Transform;
		#endregion
	}
}
