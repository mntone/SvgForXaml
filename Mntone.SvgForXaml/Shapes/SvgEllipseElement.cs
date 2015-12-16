using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using System.Xml;

namespace Mntone.SvgForXaml.Shapes
{
	[System.Diagnostics.DebuggerDisplay("Ellipse: Center = ({this.CenterX}, {this.CenterY}), RadiusX = {this.RadiusX}, RadiusY = {this.RadiusY}")]
	public sealed class SvgEllipseElement : SvgElement, ISvgStylable
	{
		internal SvgEllipseElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(element);
			this._transformableHelper = new SvgTransformableHelper(element);

			this.CenterX = element.ParseCoordinate("cx", 0.0F);
			this.CenterY = element.ParseCoordinate("cy", 0.0F);
			this.RadiusX = element.ParseCoordinate("rx", 0.0F);
			this.RadiusY = element.ParseCoordinate("ry", 0.0F);
		}

		public override string TagName => "ellipse";
		public SvgLength CenterX { get; }
		public SvgLength CenterY { get; }
		public SvgLength RadiusX { get; }
		public SvgLength RadiusY { get; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private readonly SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion

		#region ISvgTransformable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private readonly SvgTransformableHelper _transformableHelper;
		public SvgTransformCollection Transform => this._transformableHelper.Transform;
		#endregion
	}
}