using Mntone.SvgForXaml.Interfaces;

namespace Mntone.SvgForXaml.Primitives
{
	public struct SvgFillRule : ICssValue
	{
		public SvgFillRule(string fillRule)
		{
			if (fillRule == "nonzero") this.Value = SvgFillRuleType.NonZero;
			else if (fillRule == "evenodd") this.Value = SvgFillRuleType.EvenOdd;
			else this.Value = SvgFillRuleType.Unknown;
		}

		public SvgFillRuleType Value { get; }
	}
}