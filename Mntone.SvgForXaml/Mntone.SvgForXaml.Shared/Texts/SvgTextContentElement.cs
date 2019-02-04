using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Texts
{
	public abstract class SvgTextContentElement : SvgElement, ISvgStylable
	{
		internal SvgTextContentElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(this, element);

			this.TextLength = element.ParseLength("textLength", 0.0F);

			var lengthAdjust = LengthAdjustTypeHelper.Parse(element.GetAttribute("lengthAdjust"));
			if (lengthAdjust == LengthAdjustType.Unknown) lengthAdjust = LengthAdjustType.Spacing;
			this.LengthAdjust = lengthAdjust;
		}

		protected override void DeepCopy(SvgElement element)
		{
			var casted = (SvgTextContentElement)element;
			casted._stylableHelper = this._stylableHelper.DeepCopy(casted);
		}

		public SvgLength TextLength { get; }
		public LengthAdjustType LengthAdjust { get; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion
	}
}
