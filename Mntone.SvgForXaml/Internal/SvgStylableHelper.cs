using Mntone.SvgForXaml.Interfaces;
using System.Linq;
using System.Xml;

namespace Mntone.SvgForXaml.Internal
{
	internal sealed class SvgStylableHelper
	{
		private static readonly string[] PRESENTATION_ATTRIBUTE_NAMES = { "fill", "fill-opacity", "stroke", "stroke-width", "stroke-opacity", "stop-color", "stop-opacity" };

		public SvgStylableHelper(XmlElement element)
		{
			this.ClassName = element.GetAttributeOrNone("class", string.Empty);
			this.Style = new CssStyleDeclaration(element.GetAttribute("style"));

			foreach (var pn in PRESENTATION_ATTRIBUTE_NAMES)
			{
				var value = element.GetAttribute(pn);
				if (!string.IsNullOrWhiteSpace(value)) this.Style.SetProperty(pn, value, string.Empty, true);
			}
		}

		public string ClassName { get; }
		public CssStyleDeclaration Style { get; }

		public ICssValue GetPresentationAttribute(string name)
		{
			if (this.Style == null) return null;

			if (PRESENTATION_ATTRIBUTE_NAMES.Any(pn => pn == name))
			{
				return this.Style.GetPropertyCssValue(name);
			}
			return null;
		}
	}
}