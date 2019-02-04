using System.Collections.Generic;

namespace Mntone.SvgForXaml.Interfaces
{
	public interface INode
	{
		SvgDocument OwnerDocument { get; }
		INode ParentNode { get; }
		IReadOnlyCollection<INode> ChildNodes { get; }
		INode FirstChild { get; }
		INode LastChild { get; }
		INode PreviousSibling { get; }
		INode NextSibling { get; }

		INode CloneNode(bool deep = false);
	}
}