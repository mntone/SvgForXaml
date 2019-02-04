using Mntone.SvgForXaml.Primitives;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Internal
{
	internal static class SvgHelper
	{
		public static SvgNumber ParseNumber(this XmlElement element, string attributeName)
		{
			return SvgNumber.Parse(element.GetAttribute(attributeName));
		}
		public static SvgNumber ParseNumber(this XmlElement element, string attributeName, float defaultValue)
		{
			var attribute = element.GetAttribute(attributeName);
			if (string.IsNullOrEmpty(attribute)) return defaultValue;

			return element.ParseNumber(attributeName);
		}
		public static SvgNumber ParseNumber(this XmlElement element, string attributeName, SvgNumber defaultValue)
		{
			var attribute = element.GetAttribute(attributeName);
			if (string.IsNullOrEmpty(attribute)) return defaultValue;

			return element.ParseNumber(attributeName);
		}

		public static float ParseNumberOrPercentage(this XmlElement element, string attributeName)
		{
			float result;
			if (!TryParseNumberOrPercentage(element.GetAttribute(attributeName), out result)) result = 0.0F;
			return result;
		}
		public static bool TryParseNumberOrPercentage(string numberOrPercentageText, out float result)
		{
			var ptr = new StringPtr(numberOrPercentageText);
			ptr.AdvanceNumber();

			var percentage = false;
			if (!ptr.IsEnd && ptr.Char == '%')
			{
				percentage = true;
				++ptr;
			}
			if (ptr.Index != numberOrPercentageText.Length)
			{
				result = 0.0F;
				return false;
			}

			if (percentage) numberOrPercentageText = numberOrPercentageText.Substring(0, numberOrPercentageText.Length - 1);
			result = float.Parse(numberOrPercentageText, System.Globalization.CultureInfo.InvariantCulture);
			if (percentage) result /= 100.0F;
			return true;
		}

		public static SvgLength ParseLength(this XmlElement element, string attributeName)
		{
			return SvgLength.Parse(element.GetAttribute(attributeName));
		}
		public static SvgLength ParseLength(this XmlElement element, string attributeName, float defaultValue)
		{
			var attribute = element.GetAttribute(attributeName);
			if (string.IsNullOrEmpty(attribute)) return defaultValue;

			return element.ParseLength(attributeName);
		}
		public static SvgLength ParseLength(this XmlElement element, string attributeName, SvgLength defaultValue)
		{
			var attribute = element.GetAttribute(attributeName);
			if (string.IsNullOrEmpty(attribute)) return defaultValue;

			return element.ParseLength(attributeName);
		}
		public static bool TryParseLength(this XmlElement element, string attributeName, out SvgLength result)
		{
			result = 0.0F;

			var attribute = element.GetAttribute(attributeName);
			if (string.IsNullOrEmpty(attribute)) return false;

			var lengthText = element.GetAttribute(attributeName);
			return SvgLength.TryParse(lengthText, true, out result);
		}

		public static SvgLength ParseCoordinate(this XmlElement element, string attributeName) => element.ParseLength(attributeName);
		public static SvgLength ParseCoordinate(this XmlElement element, string attributeName, float defaultValue) => element.ParseLength(attributeName, defaultValue);
		public static SvgLength ParseCoordinate(this XmlElement element, string attributeName, SvgLength defaultValue) => element.ParseLength(attributeName, defaultValue);
		public static bool TryParseCoordinate(this XmlElement element, string attributeName, out SvgLength result) => element.TryParseLength(attributeName, out result);
	}
}