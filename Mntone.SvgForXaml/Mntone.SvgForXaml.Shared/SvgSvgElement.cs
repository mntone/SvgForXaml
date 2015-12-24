using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml
{
	public sealed class SvgSvgElement : SvgElement, ISvgStylable
	{
		internal SvgSvgElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(this, element);

			this.ViewPort = SvgRect.Parse(element.GetAttribute("viewBox"));
		}

		protected override void DeepCopy(SvgElement element)
		{
			var casted = (SvgSvgElement)element;
			casted._stylableHelper = this._stylableHelper.DeepCopy(casted);
		}

		public override string TagName => "svg";
		public SvgRect? ViewPort { get; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion
	}

}