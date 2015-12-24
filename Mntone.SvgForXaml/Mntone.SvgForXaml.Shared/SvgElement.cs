using Mntone.SvgForXaml.Gradients;
using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Maskings;
using Mntone.SvgForXaml.Path;
using Mntone.SvgForXaml.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml
{
	public abstract class SvgElement : IElement
	{
		protected internal SvgElement(INode parent, string id)
		{
			this.ParentNode = parent;
			this.Id = id;
			if (!string.IsNullOrEmpty(this.Id))
			{
				this.OwnerDocument.AddIdCache(this.Id, this);
			}
		}

		protected internal SvgElement(INode parent, XmlElement element)
		{
			this.ParentNode = parent;
			this.Id = element.GetAttribute("id");
			this.ChildNodes = ParseChildren(this, element.ChildNodes);

			if (!string.IsNullOrEmpty(this.Id))
			{
				this.OwnerDocument.AddIdCache(this.Id, this);
			}
		}

		public string Id { get; }
		public abstract string TagName { get; }

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public SvgDocument OwnerDocument => this.ParentNode?.OwnerDocument;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public INode ParentNode { get; }

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
		public IReadOnlyCollection<SvgElement> ChildNodes { get; private set; }

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public SvgElement FirstChild => this.ChildNodes.FirstOrDefault();

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public SvgElement LastChild => this.ChildNodes.LastOrDefault();

		public virtual INode CloneNode(bool deep = false)
		{
			var shallow = (SvgElement)this.MemberwiseClone();
			if (deep)
			{
				if (this.ChildNodes != null)
				{
					var deepChildren = new List<SvgElement>();
					foreach (var child in this.ChildNodes)
					{
						var deepChild = child.CloneNode(deep);
						deepChildren.Add((SvgElement)deepChild);
					}
					shallow.ChildNodes = deepChildren;
				}

				this.DeepCopy(shallow);
			}
			return shallow;
		}

		protected virtual void DeepCopy(SvgElement element) { }

		protected static IReadOnlyList<SvgElement> ParseChildren(INode parent, XmlNodeList nodes)
		{
			var result = new Collection<SvgElement>();
			foreach (var node in nodes)
			{
				var elementNode = node as XmlElement;
				if (elementNode == null) continue;

				switch (elementNode.TagName)
				{
					case "svg":
						result.Add(new SvgSvgElement(parent, elementNode));
						break;

					case "g":
						result.Add(new SvgGroupElement(parent, elementNode));
						break;

					case "defs":
						result.Add(new SvgDefinitionsElement(parent, elementNode));
						break;

					case "desc":
						result.Add(new SvgDescriptionElement(parent, elementNode));
						break;

					case "symbol":
						result.Add(new SvgSymbolElement(parent, elementNode));
						break;

					case "use":
						result.Add(new SvgUseElement(parent, elementNode));
						break;

					case "title":
						result.Add(new SvgTitleElement(parent, elementNode));
						break;

					case "path":
						result.Add(new SvgPathElement(parent, elementNode));
						break;

					case "rect":
						result.Add(new SvgRectElement(parent, elementNode));
						break;

					case "circle":
						result.Add(new SvgCircleElement(parent, elementNode));
						break;

					case "ellipse":
						result.Add(new SvgEllipseElement(parent, elementNode));
						break;

					case "line":
						result.Add(new SvgLineElement(parent, elementNode));
						break;

					case "polyline":
						result.Add(new SvgPolylineElement(parent, elementNode));
						break;

					case "polygon":
						result.Add(new SvgPolygonElement(parent, elementNode));
						break;

					case "linearGradient":
						result.Add(new SvgLinearGradientElement(parent, elementNode));
						break;

					case "radialGradient":
						result.Add(new SvgRadialGradientElement(parent, elementNode));
						break;

					case "stop":
						result.Add(new SvgStopElement(parent, elementNode));
						break;

					case "clipPath":
						result.Add(new SvgClipPathElement(parent, elementNode));
						break;

					default:
						System.Diagnostics.Debug.WriteLine($"Not supported tag: {elementNode.TagName}");
						break;
				}
			}
			return result;
		}
	}
}