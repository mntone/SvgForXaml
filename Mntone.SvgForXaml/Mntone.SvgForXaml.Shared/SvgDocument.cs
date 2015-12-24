﻿using Mntone.SvgForXaml.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
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
		IReadOnlyCollection<SvgElement> INode.ChildNodes => this._ChildNodes;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		SvgElement INode.FirstChild => this.RootElement;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		SvgElement INode.LastChild => this.RootElement;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private IReadOnlyCollection<SvgElement> _ChildNodes;

		public INode CloneNode(bool deep = false)
		{
			var shallow = (SvgDocument)this.MemberwiseClone();
			if (deep)
			{
				throw new NotImplementedException();
			}
			return shallow;
		}

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

		public static SvgDocument Parse(byte[] document) => Parse(Encoding.UTF8.GetString(document));
		public static SvgDocument Parse(byte[] document, Encoding encoding) => Parse(encoding.GetString(document));
		public static SvgDocument Parse(string document)
		{
			using (var sr = new StringReader(document))
			using (var reader = XmlReader.Create(sr, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore }))
			{
				var xml = new XmlDocument();
				xml.Load(reader);
				return Parse(xml);
			}
		}

		public static SvgDocument Parse(XmlDocument document)
		{
			var svgDocument = new SvgDocument();
			var rootElement = new SvgSvgElement(svgDocument, document.DocumentElement);

			var childNodes = new Collection<SvgElement>();
			childNodes.Add(rootElement);
			svgDocument._ChildNodes = childNodes;
			svgDocument.RootElement = rootElement;

			return svgDocument;
		}
	}
}