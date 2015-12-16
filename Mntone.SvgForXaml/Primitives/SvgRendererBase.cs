using Mntone.SvgForXaml.Path;
using Mntone.SvgForXaml.Primitives;
using Mntone.SvgForXaml.Shapes;
using System.Collections.Generic;

namespace Mntone.SvgForXaml
{
	public abstract class SvgRendererBase<TSession>
	{
		protected SvgDocument TargetDocument { get; set; }
		protected SvgLengthConverter LengthConverter { get; }

		public SvgRendererBase(SvgDocument targetDocument)
		{
			this.TargetDocument = targetDocument;
			this.LengthConverter = new SvgLengthConverter();
		}

		public virtual void Renderer(float width, float height, TSession session)
		{
			this.LengthConverter.CanvasSize = new SvgPoint(width, height);

			var root = this.TargetDocument.RootElement;
			this.RendererSvg(session, root);
		}

		protected abstract void RendererSvg(TSession session, SvgSvgElement element);
		protected abstract void RendererGroup(TSession session, SvgGroupElement element);
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
				var target = element;
				if (target.GetType() == typeof(SvgUseElement))
				{
					target = ((SvgUseElement)target).InstanceRoot;
				}
				if (target.GetType() == typeof(SvgSvgElement))
				{
					this.RendererSvg(session, (SvgSvgElement)target);
				}
				else if (target.GetType() == typeof(SvgGroupElement))
				{
					this.RendererGroup(session, (SvgGroupElement)target);
				}
				else if (target.GetType() == typeof(SvgPathElement))
				{
					this.RendererPath(session, (SvgPathElement)target);
				}
				else if (target.GetType() == typeof(SvgRectElement))
				{
					this.RendererRect(session, (SvgRectElement)target);
				}
				else if (target.GetType() == typeof(SvgCircleElement))
				{
					this.RendererCircle(session, (SvgCircleElement)target);
				}
				else if (target.GetType() == typeof(SvgEllipseElement))
				{
					this.RendererEllipse(session, (SvgEllipseElement)target);
				}
				else if (target.GetType() == typeof(SvgLineElement))
				{
					this.RendererLine(session, (SvgLineElement)target);
				}
				else if (target.GetType() == typeof(SvgPolylineElement))
				{
					this.RendererPolyline(session, (SvgPolylineElement)target);
				}
				else if (target.GetType() == typeof(SvgPolygonElement))
				{
					this.RendererPolygon(session, (SvgPolygonElement)target);
				}
			}
		}
	}
}