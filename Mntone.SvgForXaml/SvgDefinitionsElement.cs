using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using System.Xml;

namespace Mntone.SvgForXaml
{
	public sealed class SvgDefinitionsElement : SvgElement, ISvgStylable, ISvgTransformable
	{
		internal SvgDefinitionsElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(element);
			this._transformableHelper = new SvgTransformableHelper(element);
		}

		public override string TagName => "defs";

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