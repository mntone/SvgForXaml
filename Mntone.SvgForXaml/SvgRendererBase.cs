using Mntone.SvgForXaml.Path;
using Mntone.SvgForXaml.Shapes;
using System.Collections.Generic;

namespace Mntone.SvgForXaml
{
	public abstract class SvgRendererBase<TCanvas, TSession>
	{
		protected TCanvas RendererTarget { get; }
		protected SvgDocument TargetDocument { get; set; }

		public SvgRendererBase(TCanvas rendererTarget, SvgDocument targetDocument)
		{
			this.RendererTarget = rendererTarget;
			this.TargetDocument = targetDocument;
		}

		public virtual void Renderer(TSession session)
		{
			var root = this.TargetDocument.RootElement;
			this.RendererSvg(session, root);
		}

		protected abstract void RendererSvg(TSession session, SvgSvgElement element);
		protected abstract void RendererPath(TSession session, SvgPathElement element);
		protected abstract void RendererRect(TSession session, SvgRectElement element);
		protected abstract void RendererCircle(TSession session, SvgCircleElement element);
		protected abstract void RendererEllipse(TSession session, SvgEllipseElement element);
		protected abstract void RendererLine(TSession session, SvgLineElement element);
		protected abstract void RendererPolyline(TSession session, SvgPolylineElement element);
		protected abstract void RendererPolygon(TSession session, SvgPolygonElement element);

		protected virtual void RendererChildren(TSession session, IReadOnlyList<SvgElement> elements)
		{
			foreach (var element in elements)
			{
				if (element.GetType() == typeof(SvgSvgElement))
				{
					this.RendererSvg(session, (SvgSvgElement)element);
				}
				else if (element.GetType() == typeof(SvgGroupElement))
				{
					this.RendererChildren(session, element.ChildNodes);
				}
				else if (element.GetType() == typeof(SvgPathElement))
				{
					this.RendererPath(session, (SvgPathElement)element);
				}
				else if (element.GetType() == typeof(SvgRectElement))
				{
					this.RendererRect(session, (SvgRectElement)element);
				}
				else if (element.GetType() == typeof(SvgCircleElement))
				{
					this.RendererCircle(session, (SvgCircleElement)element);
				}
				else if (element.GetType() == typeof(SvgEllipseElement))
				{
					this.RendererEllipse(session, (SvgEllipseElement)element);
				}
				else if (element.GetType() == typeof(SvgLineElement))
				{
					this.RendererLine(session, (SvgLineElement)element);
				}
				else if (element.GetType() == typeof(SvgPolylineElement))
				{
					this.RendererPolyline(session, (SvgPolylineElement)element);
				}
				else if (element.GetType() == typeof(SvgPolygonElement))
				{
					this.RendererPolygon(session, (SvgPolygonElement)element);
				}
			}
		}
	}
}