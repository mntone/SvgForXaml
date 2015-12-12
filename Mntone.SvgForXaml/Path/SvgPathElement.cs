using Mntone.SvgForXaml.Internal;
using System.Xml;

namespace Mntone.SvgForXaml.Path
{
	public sealed class SvgPathElement : SvgElement, ISvgStylable
	{
		internal SvgPathElement(INode parent, XmlElement element)
			: base(parent, element.GetAttributeOrNone("id", string.Empty))
		{
			this._stylableHelper = new SvgStylableHelper(element);

			this.Data = element.GetAttribute("d");
			this.Segments = SvgPathSegmentParser.Parse(this.Data);
		}

		public override string TagName => "path";
		public string Data { get; }
		public SvgPathSegmentCollection Segments { get; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private readonly SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion
	}
}