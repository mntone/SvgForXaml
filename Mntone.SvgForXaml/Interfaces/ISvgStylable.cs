namespace Mntone.SvgForXaml.Interfaces
{
	public interface ISvgStylable
	{
		string ClassName { get; }
		CssStyleDeclaration Style { get; }

		ICssValue GetPresentationAttribute(string name);
	}
}