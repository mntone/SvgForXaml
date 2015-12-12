using System.Collections.Generic;

namespace Mntone.SvgForXaml
{
	public interface INode
	{
		SvgDocument OwnerDocument { get; }
		INode ParentNode { get; }
		IReadOnlyList<SvgElement> ChildNodes { get; }
		SvgElement FirstChild { get; }
		SvgElement LastChild { get; }
	}
}