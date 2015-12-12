using System.Collections.Generic;
using System.Xml;

namespace Mntone.SvgForXaml
{
	public sealed class SvgDocument : INode
	{
		private Dictionary<string, SvgElement> _idCache;

		private SvgDocument()
		{
			this._idCache = new Dictionary<string, SvgElement>();
		}

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		SvgDocument INode.OwnerDocument => this;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		INode INode.ParentNode => null;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		IReadOnlyList<SvgElement> INode.ChildNodes => this._ChildNodes;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		SvgElement INode.FirstChild => this.RootElement;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		SvgElement INode.LastChild => this.RootElement;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private IReadOnlyList<SvgElement> _ChildNodes;

		public SvgSvgElement RootElement { get; private set; }

		internal void AddIdCache(string id, SvgElement element)
		{
			this._idCache.Add(id, element);
		}

		public SvgElement GetElementById(string id)
		{
			if (!this._idCache.ContainsKey(id)) return null;
			return this._idCache[id];
		}

		public static SvgDocument Parse(XmlDocument document)
		{
			var svgDocument = new SvgDocument();
			var rootElement = new SvgSvgElement(svgDocument, document.DocumentElement);

			var childNodes = new List<SvgElement>(1);
			childNodes.Add(rootElement);
			svgDocument._ChildNodes = childNodes;
			svgDocument.RootElement = rootElement;

			return svgDocument;
		}
	}
}