using Mntone.SvgForXaml.Interfaces;
using System.Linq;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Internal
{
	internal sealed class SvgStylableHelper
	{
		private static readonly string[] PRESENTATION_ATTRIBUTE_NAMES =
		{
			"color",
			"fill",
			"fill-opacity",
			"fill-rule",
			"stroke",
			"stroke-width",
			"stroke-linecap",
			"stroke-linejoin",
			"stroke-miterlimit",
			"stroke-opacity",
			"stop-color",
			"stop-opacity",
			"clip-path",
			"opacity",
		};

		public SvgStylableHelper(ISvgStylable parent, XmlElement element)
		{
			this.ClassName = element.GetAttribute("class");
			this.Style = new CssStyleDeclaration(parent, element.GetAttribute("style"));

			foreach (var pn in PRESENTATION_ATTRIBUTE_NAMES)
			{
				var value = element.GetAttribute(pn);
				if (!string.IsNullOrWhiteSpace(value)) this.Style.SetProperty(pn, value, string.Empty, true);
			}
		}

		internal SvgStylableHelper DeepCopy(ISvgStylable parent)
		{
			var obj = (SvgStylableHelper)this.MemberwiseClone();
			obj.Style = this.Style.DeepCopy(parent);
			return obj;
		}

		public string ClassName { get; }
		public CssStyleDeclaration Style { get; private set; }

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