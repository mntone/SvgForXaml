using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using System.Xml;

namespace Mntone.SvgForXaml.Shapes
{
	public sealed class SvgPolygonElement : SvgElement, ISvgStylable, ISvgTransformable
	{
		internal SvgPolygonElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(element);
			this._transformableHelper = new SvgTransformableHelper(element);

			this.Points = new SvgPointCollection(element.GetAttribute("points"));
		}

		protected override void DeepCopy(SvgElement element)
		{
			var casted = (SvgPolygonElement)element;
			casted._stylableHelper = this._stylableHelper.DeepCopy();
			casted._transformableHelper = this._transformableHelper.DeepCopy();
		}

		public override string TagName => "polygon";
		public SvgPointCollection Points { get; }

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