using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using System.Xml;

namespace Mntone.SvgForXaml.Maskings
{
	public sealed class SvgClipPathElement : SvgElement, ISvgStylable
	{
		internal SvgClipPathElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(element);

			var clipPathUnits = SvgUnitTypeHelper.Parse(element.GetAttribute("clipPathUnits"));
			if (clipPathUnits == SvgUnitType.Unknown) clipPathUnits = SvgUnitType.UserSpaceOnUse;
			this.ClipPathUnits = clipPathUnits;
		}

		public override string TagName => "clipPath";
		public SvgUnitType ClipPathUnits { get; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private readonly SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion
	}
}
