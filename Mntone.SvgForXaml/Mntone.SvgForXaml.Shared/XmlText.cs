using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mntone.SvgForXaml
{
	public sealed class XmlText : ICharacterData
	{
		internal XmlText(INode parent, string data)
		{
			this.Data = data;
		}

		public string Data { get; }

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public SvgDocument OwnerDocument => this.ParentNode?.OwnerDocument;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public INode ParentNode { get; }

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
		public IReadOnlyCollection<INode> ChildNodes { get; private set; }

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public INode FirstChild => this.ChildNodes.FirstOrDefault();

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public INode LastChild => this.ChildNodes.LastOrDefault();

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public INode PreviousSibling => this.ParentNode?.ChildNodes?.PreviousOrDefault(e => e == this);

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public INode NextSibling => this.ParentNode?.ChildNodes?.NextOrDefault(e => e == this);

		public INode CloneNode(bool deep = false)
		{
			var shallow = (XmlText)this.MemberwiseClone();
			if (deep)
			{
				if (this.ChildNodes != null)
				{
					var deepChildren = new Collection<INode>();
					foreach (var child in this.ChildNodes)
					{
						var deepChild = child.CloneNode(deep);
						deepChildren.Add(deepChild);
					}
					shallow.ChildNodes = deepChildren;
				}
			}
			return shallow;
		}
	}
}
