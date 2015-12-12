using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using System.Xml;

namespace Mntone.SvgForXaml
{
	public sealed class SvgUseElement : SvgElement, ISvgStylable
	{
		internal SvgUseElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(element);

			this.X = element.ParseCoordinate("x", 0.0F);
			this.Y = element.ParseCoordinate("y", 0.0F);
			this.Width = element.ParseLength("width", 0.0F);
			this.Height = element.ParseLength("height", 0.0F);
			this.Href = element.GetAttribute("href", "xlink");
		}

		public override string TagName => "defs";
		public SvgLength X { get; }
		public SvgLength Y { get; }
		public SvgLength Width { get; }
		public SvgLength Height { get; }
		public string Href { get; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private readonly SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion
	}
}