using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using System.Xml;

namespace Mntone.SvgForXaml.Path
{
	public sealed class SvgPathElement : SvgElement, ISvgStylable, ISvgTransformable
	{
		internal SvgPathElement(INode parent, XmlElement element)
			: base(parent, element.GetAttributeOrNone("id", string.Empty))
		{
			this._stylableHelper = new SvgStylableHelper(element);
			this._transformableHelper = new SvgTransformableHelper(element);

			this.Data = element.GetAttribute("d");
			this.Segments = SvgPathSegmentParser.Parse(this.Data);
		}

		protected override void DeepCopy(SvgElement element)
		{
			var casted = (SvgPathElement)element;
			casted._stylableHelper = this._stylableHelper.DeepCopy();
			casted._transformableHelper = this._transformableHelper.DeepCopy();
		}

		public override string TagName => "path";
		public string Data { get; }
		public SvgPathSegmentCollection Segments { get; }

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