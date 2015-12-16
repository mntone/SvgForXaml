using System.Collections.Generic;

namespace Mntone.SvgForXaml.Interfaces
{
	public interface INode
	{
		SvgDocument OwnerDocument { get; }
		INode ParentNode { get; }
		IReadOnlyList<SvgElement> ChildNodes { get; }
		SvgElement FirstChild { get; }
		SvgElement LastChild { get; }

		INode CloneNode();
	}
}