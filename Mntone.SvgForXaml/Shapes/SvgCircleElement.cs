using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using System.Xml;

namespace Mntone.SvgForXaml.Shapes
{
	[System.Diagnostics.DebuggerDisplay("Circle: Center = ({this.CenterX}, {this.CenterY}), Radius = {this.Radius}")]
	public sealed class SvgCircleElement : SvgElement, ISvgStylable
	{
		internal SvgCircleElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(element);

			this.CenterX = element.ParseCoordinate("cx", 0.0F);
			this.CenterY = element.ParseCoordinate("cy", 0.0F);
			this.Radius = element.ParseLength("r", 0.0F);
		}

		public override string TagName => "circle";
		public SvgLength CenterX { get; }
		public SvgLength CenterY { get; }
		public SvgLength Radius { get; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private readonly SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion
	}
}