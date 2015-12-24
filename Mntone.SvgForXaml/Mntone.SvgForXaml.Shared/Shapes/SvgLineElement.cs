using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Shapes
{
	[System.Diagnostics.DebuggerDisplay("Line: 1 = ({this.X1.ValueAsString}, {this.Y1.ValueAsString}), 2 = ({this.X2.ValueAsString}, {this.Y2.ValueAsString})")]
	public sealed class SvgLineElement : SvgElement, ISvgStylable, ISvgTransformable
	{
		internal SvgLineElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(this, element);
			this._transformableHelper = new SvgTransformableHelper(element);

			this.X1 = element.ParseCoordinate("x1", 0.0F);
			this.Y1 = element.ParseCoordinate("y1", 0.0F);
			this.X2 = element.ParseCoordinate("x2", 0.0F);
			this.Y2 = element.ParseCoordinate("y2", 0.0F);
		}

		protected override void DeepCopy(SvgElement element)
		{
			var casted = (SvgLineElement)element;
			casted._stylableHelper = this._stylableHelper.DeepCopy(casted);
			casted._transformableHelper = this._transformableHelper.DeepCopy();
		}

		public override string TagName => "line";
		public SvgLength X1 { get; }
		public SvgLength Y1 { get; }
		public SvgLength X2 { get; }
		public SvgLength Y2 { get; }

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