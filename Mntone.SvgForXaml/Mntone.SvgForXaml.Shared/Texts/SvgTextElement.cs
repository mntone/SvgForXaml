using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Texts
{
	public sealed class SvgTextElement : SvgTextPositioningElement, ISvgTransformable
	{
		public SvgTextElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._transformableHelper = new SvgTransformableHelper(element);
		}

		protected override void DeepCopy(SvgElement element)
		{
			base.DeepCopy(element);

			var casted = (SvgTextElement)element;
			casted._transformableHelper = this._transformableHelper.DeepCopy();
		}

		public override string TagName => "text";

		#region ISvgTransformable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private SvgTransformableHelper _transformableHelper;
		public SvgTransformCollection Transform => this._transformableHelper.Transform;
		#endregion
	}
}
