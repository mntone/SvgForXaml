using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Filters
{
	[System.Diagnostics.DebuggerDisplay("Filter: FilterUnits = {this.FilterUnits}, PrimitiveUnits = {this.PrimitiveUnits}, X = {this.X.ValueAsString}, Y = {this.Y.ValueAsString}, Width = {this.Width.ValueAsString}, Height = {this.Height.ValueAsString}")]
	public sealed class SvgFilterElement : SvgElement, ISvgStylable
	{
		internal SvgFilterElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(this, element);

			this.FilterUnits = SvgUnitTypeHelper.Parse(element.GetAttribute("filterUnits"), SvgUnitType.ObjectBoundingBox);
			this.PrimitiveUnits = SvgUnitTypeHelper.Parse(element.GetAttribute("primitiveUnits"), SvgUnitType.UserSpaceOnUse);
			this.X = element.ParseCoordinate("x", 0.0F);
			this.Y = element.ParseCoordinate("y", 0.0F);
			this.Width = element.ParseLength("width", 0.0F);
			this.Height = element.ParseLength("height", 0.0F);
		}

		protected override void DeepCopy(SvgElement element)
		{
			var casted = (SvgFilterElement)element;
			casted._stylableHelper = this._stylableHelper.DeepCopy(casted);
		}

		public override string TagName => "filter";
		public SvgUnitType FilterUnits { get; }
		public SvgUnitType PrimitiveUnits { get; }
		public SvgLength X { get; }
		public SvgLength Y { get; }
		public SvgLength Width { get; set; }
		public SvgLength Height { get; set; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion
	}
}
