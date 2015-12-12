namespace Mntone.SvgForXaml
{
	public interface ISvgStylable
	{
		string ClassName { get; }
		CssStyleDeclaration Style { get; }

		ICssValue GetPresentationAttribute(string name);
	}
}