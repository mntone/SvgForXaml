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

		public virtual void Render(float width, float height, TSession session)
		{
			this.LengthConverter.CanvasSize = new SvgPoint(width, height);

			var root = this.TargetDocument.RootElement;
			this.RenderSvg(session, root);
		}

		protected abstract void RenderSvg(TSession session, SvgSvgElement element);
		protected abstract void RenderGroup(TSession session, SvgGroupElement element);
		protected abstract void RenderUse(TSession session, SvgUseElement element);
		protected abstract void RenderPath(TSession session, SvgPathElement element);
		protected abstract void RenderRect(TSession session, SvgRectElement element);
		protected abstract void RenderCircle(TSession session, SvgCircleElement element);
		protected abstract void RenderEllipse(TSession session, SvgEllipseElement element);
		protected abstract void RenderLine(TSession session, SvgLineElement element);
		protected abstract void RenderPolyline(TSession session, SvgPolylineElement element);
		protected abstract void RenderPolygon(TSession session, SvgPolygonElement element);

		protected virtual void RenderChildren(TSession session, IReadOnlyCollection<SvgElement> elements)
		{
			foreach (var element in elements) this.RenderChild(session, element);
		}

		protected virtual void RenderChild(TSession session, SvgElement element)
		{
			if (element.GetType() == typeof(SvgSvgElement))
			{
				this.RenderSvg(session, (SvgSvgElement)element);
			}
			else if (element.GetType() == typeof(SvgGroupElement))
			{
				this.RenderGroup(session, (SvgGroupElement)element);
			}
			else if (element.GetType() == typeof(SvgUseElement))
			{
				this.RenderUse(session, (SvgUseElement)element);
			}
			else if (element.GetType() == typeof(SvgPathElement))
			{
				this.RenderPath(session, (SvgPathElement)element);
			}
			else if (element.GetType() == typeof(SvgRectElement))
			{
				this.RenderRect(session, (SvgRectElement)element);
			}
			else if (element.GetType() == typeof(SvgCircleElement))
			{
				this.RenderCircle(session, (SvgCircleElement)element);
			}
			else if (element.GetType() == typeof(SvgEllipseElement))
			{
				this.RenderEllipse(session, (SvgEllipseElement)element);
			}
			else if (element.GetType() == typeof(SvgLineElement))
			{
				this.RenderLine(session, (SvgLineElement)element);
			}
			else if (element.GetType() == typeof(SvgPolylineElement))
			{
				this.RenderPolyline(session, (SvgPolylineElement)element);
			}
			else if (element.GetType() == typeof(SvgPolygonElement))
			{
				this.RenderPolygon(session, (SvgPolygonElement)element);
			}
		}
	}
}