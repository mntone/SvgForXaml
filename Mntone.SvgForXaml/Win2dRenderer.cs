using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Mntone.SvgForXaml.Gradients;
using Mntone.SvgForXaml.Path;
using Mntone.SvgForXaml.Primitives;
using Mntone.SvgForXaml.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Mntone.SvgForXaml
{
	public class Win2dRenderer : SvgRendererBase<CanvasDrawingSession>, IDisposable
	{
		protected ICanvasResourceCreator ResourceCreator { get; }
		protected Collection<IDisposable> DisposableObjects { get; }
		protected Dictionary<SvgGradientElement, ICanvasBrush> ResourceCache { get; }

		private bool _disposed = false;

		public Win2dRenderer(ICanvasResourceCreator resourceCreator, SvgDocument targetDocument)
			: base(targetDocument)
		{
			this.ResourceCreator = resourceCreator;
			this.DisposableObjects = new Collection<IDisposable>();
			this.ResourceCache = new Dictionary<SvgGradientElement, ICanvasBrush>();
		}

		public void Dispose() => this.Dispose(true);
		protected void Dispose(bool disposing)
		{
			if (this._disposed) return;
			if (disposing)
			{
				foreach (var disposableObject in this.DisposableObjects)
				{
					disposableObject.Dispose();
				}
			}
			this._disposed = true;
		}

		protected override void RenderSvg(CanvasDrawingSession session, SvgSvgElement element)
		{
			this.RenderChildren(session, element.ChildNodes);
		}

		protected override void RenderGroup(CanvasDrawingSession session, SvgGroupElement element)
		{
			using (var t = TransformSession.CreateTransformSession(session, element.Transform.Result))
			{
				this.RenderChildren(session, element.ChildNodes);
			}
		}

		protected override void RenderUse(CanvasDrawingSession session, SvgUseElement element)
		{
			using (var t = TransformSession.CreateTransformSession(session, element.Transform.Result))
			{
				this.RenderChild(session, element.InstanceRoot);
			}
		}

		protected override void RenderPath(CanvasDrawingSession session, SvgPathElement element)
		{
			double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;

			var open = false;
			var v = new Vector2(0.0F, 0.0F);
			var builder = new CanvasPathBuilder(this.ResourceCreator);
			foreach (var segment in element.Segments)
			{
				if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.ClosePath)
				{
					builder.EndFigure(CanvasFigureLoop.Closed);
					open = false;
					continue;
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.MoveToAbsolute)
				{
					if (open) builder.EndFigure(CanvasFigureLoop.Open);

					var casted = (SvgPathSegmentMoveToAbsolute)segment;
					v.X = casted.X;
					v.Y = casted.Y;
					builder.BeginFigure(v);
					open = true;

					minX = Math.Min(minX, v.X);
					minY = Math.Min(minY, v.Y);
					maxX = Math.Max(maxX, v.X);
					maxY = Math.Max(maxY, v.Y);
					continue;
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.MoveToRelative)
				{
					if (open) builder.EndFigure(CanvasFigureLoop.Open);

					var casted = (SvgPathSegmentMoveToRelative)segment;
					v.X += casted.X;
					v.Y += casted.Y;
					builder.BeginFigure(v);
					open = true;

					minX = Math.Min(minX, v.X);
					minY = Math.Min(minY, v.Y);
					maxX = Math.Max(maxX, v.X);
					maxY = Math.Max(maxY, v.Y);
					continue;
				}

				if (!open)
				{
					builder.BeginFigure(v);
					open = true;
				}
				if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.LineToAbsolute)
				{
					var casted = (SvgPathSegmentLineToAbsolute)segment;
					v.X = casted.X;
					v.Y = casted.Y;
					builder.AddLine(v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.LineToRelative)
				{
					var casted = (SvgPathSegmentLineToRelative)segment;
					v.X += casted.X;
					v.Y += casted.Y;
					builder.AddLine(v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToCubicAbsolute)
				{
					var casted = (SvgPathSegmentCurveToCubicAbsolute)segment;
					v.X = casted.X;
					v.Y = casted.Y;
					builder.AddCubicBezier(new Vector2(casted.X1, casted.Y1), new Vector2(casted.X2, casted.Y2), v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToCubicRelative)
				{
					var casted = (SvgPathSegmentCurveToCubicRelative)segment;
					var c1 = v;
					c1.X += casted.X1;
					c1.Y += casted.Y1;
					var c2 = v;
					c2.X += casted.X2;
					c2.Y += casted.Y2;
					v.X += casted.X;
					v.Y += casted.Y;
					builder.AddCubicBezier(c1, c2, v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToQuadraticAbsolute)
				{
					var casted = (SvgPathSegmentCurveToQuadraticAbsolute)segment;
					v.X = casted.X;
					v.Y = casted.Y;
					builder.AddQuadraticBezier(new Vector2(casted.X1, casted.Y1), v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToQuadraticRelative)
				{
					var casted = (SvgPathSegmentCurveToQuadraticRelative)segment;
					var c1 = v;
					c1.X += casted.X1;
					c1.Y += casted.Y1;
					v.X += casted.X;
					v.Y += casted.Y;
					builder.AddQuadraticBezier(c1, v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.ArcAbsolute)
				{
					var casted = (SvgPathSegmentArcAbsolute)segment;
					var size = casted.LargeArcFlag ? CanvasArcSize.Large : CanvasArcSize.Small;
					var sweepDirection = casted.SweepFlag ? CanvasSweepDirection.Clockwise : CanvasSweepDirection.CounterClockwise;
					builder.AddArc(new Vector2(casted.X, casted.Y), casted.RadiusX, casted.RadiusY, casted.Angle, sweepDirection, size);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.ArcRelative)
				{
					var casted = (SvgPathSegmentArcRelative)segment;
					v.X += casted.X;
					v.Y += casted.Y;
					var size = casted.LargeArcFlag ? CanvasArcSize.Large : CanvasArcSize.Small;
					var sweepDirection = casted.SweepFlag ? CanvasSweepDirection.Clockwise : CanvasSweepDirection.CounterClockwise;
					builder.AddArc(v, casted.RadiusX, casted.RadiusY, casted.Angle, sweepDirection, size);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.LineToHorizontalAbsolute)
				{
					var casted = (SvgPathSegmentLineToHorizontalAbsolute)segment;
					v.X = casted.X;
					builder.AddLine(v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.LineToHorizontalRelative)
				{
					var casted = (SvgPathSegmentLineToHorizontalRelative)segment;
					v.X += casted.X;
					builder.AddLine(v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.LineToVerticalAbsolute)
				{
					var casted = (SvgPathSegmentLineToVerticalAbsolute)segment;
					v.Y = casted.Y;
					builder.AddLine(v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.LineToVerticalRelative)
				{
					var casted = (SvgPathSegmentLineToVerticalRelative)segment;
					v.Y += casted.Y;
					builder.AddLine(v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToCubicSmoothAbsolute)
				{
					var casted = (SvgPathSegmentCurveToCubicSmoothAbsolute)segment;
					var c1 = v;
					v.X = casted.X;
					v.Y = casted.Y;
					builder.AddCubicBezier(c1, new Vector2(casted.X2, casted.Y2), v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToCubicSmoothRelative)
				{
					var casted = (SvgPathSegmentCurveToCubicSmoothRelative)segment;
					var c1 = v;
					var c2 = v;
					c2.X += casted.X2;
					c2.Y += casted.Y2;
					v.X += casted.X;
					v.Y += casted.Y;
					builder.AddCubicBezier(c1, c2, v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToQuadraticSmoothAbsolute)
				{
					var casted = (SvgPathSegmentCurveToQuadraticSmoothAbsolute)segment;
					var c1 = v;
					v.X = casted.X;
					v.Y = casted.Y;
					builder.AddQuadraticBezier(c1, v);
				}
				else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToQuadraticSmoothRelative)
				{
					var casted = (SvgPathSegmentCurveToQuadraticSmoothRelative)segment;
					var c1 = v;
					v.X += casted.X;
					v.Y += casted.Y;
					builder.AddQuadraticBezier(c1, v);
				}

				minX = Math.Min(minX, v.X);
				minY = Math.Min(minY, v.Y);
				maxX = Math.Max(maxX, v.X);
				maxY = Math.Max(maxY, v.Y);
			}
			if (open)
			{
				builder.EndFigure(CanvasFigureLoop.Open);
			}

			using (var t = TransformSession.CreateTransformSession(session, element.Transform.Result))
			{
				var area = new Rect(minX, minY, maxX - minX, maxY - minY);
				var geometry = CanvasGeometry.CreatePath(builder);
				var fill = element.Style.Fill;
				if (fill == null || fill != null && fill.PaintType != SvgPaintType.None)
				{
					var pen = this.CreatePaint(session, area, fill, element.Style.FillOpacity);
					session.FillGeometry(geometry, pen);
				}
				var stroke = element.Style.Stroke;
				if (stroke != null && fill.PaintType != SvgPaintType.None)
				{
					var pen = this.CreatePaint(session, area, stroke, element.Style.StrokeOpacity);
					var width = element.Style.StrokeWidth;
					session.DrawGeometry(geometry, pen, width.HasValue ? this.LengthConverter.Convert(width.Value) : 1.0F);
				}
			}
		}

		protected override void RenderRect(CanvasDrawingSession session, SvgRectElement element)
		{
			var x = this.LengthConverter.ConvertX(element.X);
			var y = this.LengthConverter.ConvertY(element.Y);
			var width = this.LengthConverter.ConvertX(element.Width);
			var height = this.LengthConverter.ConvertY(element.Height);
			var rx = this.LengthConverter.ConvertX(element.RoundedX);
			var ry = this.LengthConverter.ConvertX(element.RoundedY);

			using (var t = TransformSession.CreateTransformSession(session, element.Transform.Result))
			{
				var area = new Rect(x, y, width, height);
				var fill = element.Style.Fill;
				if (fill == null || fill != null && fill.PaintType != SvgPaintType.None)
				{
					var pen = this.CreatePaint(session, area, fill, element.Style.FillOpacity);
					session.FillRoundedRectangle(x, y, width, height, rx, ry, pen);
				}
				var stroke = element.Style.Stroke;
				if (stroke != null && fill.PaintType != SvgPaintType.None)
				{
					var pen = this.CreatePaint(session, area, stroke, element.Style.StrokeOpacity);
					var strokeWidth = element.Style.StrokeWidth;
					session.DrawRoundedRectangle(x, y, width, height, rx, ry, pen, strokeWidth.HasValue ? this.LengthConverter.Convert(strokeWidth.Value) : 1.0F);
				}
			}
		}

		protected override void RenderCircle(CanvasDrawingSession session, SvgCircleElement element)
		{
			var centerX = this.LengthConverter.ConvertX(element.CenterX);
			var centerY = this.LengthConverter.ConvertY(element.CenterY);
			var radiusX = this.LengthConverter.ConvertX(element.Radius);
			var radiusY = this.LengthConverter.ConvertY(element.Radius);

			using (var t = TransformSession.CreateTransformSession(session, element.Transform.Result))
			{
				var area = new Rect(centerX - radiusX, centerY - radiusY, 2.0F * radiusX, 2.0F * radiusY);
				var fill = element.Style.Fill;
				if (fill == null || fill != null && fill.PaintType != SvgPaintType.None)
				{
					var pen = this.CreatePaint(session, area, fill, element.Style.FillOpacity);
					session.FillEllipse(centerX, centerY, radiusX, radiusY, pen);
				}
				var stroke = element.Style.Stroke;
				if (stroke != null && stroke.PaintType != SvgPaintType.None)
				{
					var pen = this.CreatePaint(session, area, stroke, element.Style.StrokeOpacity);
					var strokeWidth = element.Style.StrokeWidth;
					session.DrawEllipse(centerX, centerY, radiusX, radiusY, pen, strokeWidth.HasValue ? this.LengthConverter.Convert(strokeWidth.Value) : 1.0F);
				}
			}
		}

		protected override void RenderEllipse(CanvasDrawingSession session, SvgEllipseElement element)
		{
			var centerX = this.LengthConverter.ConvertX(element.CenterX);
			var centerY = this.LengthConverter.ConvertY(element.CenterY);
			var radiusX = this.LengthConverter.ConvertX(element.RadiusX);
			var radiusY = this.LengthConverter.ConvertY(element.RadiusY);

			using (var t = TransformSession.CreateTransformSession(session, element.Transform.Result))
			{
				var area = new Rect(centerX - radiusX, centerY - radiusY, 2.0F * radiusX, 2.0F * radiusY);
				var fill = element.Style.Fill;
				if (fill == null || fill != null && fill.PaintType != SvgPaintType.None)
				{
					var pen = this.CreatePaint(session, area, fill, element.Style.FillOpacity);
					session.FillEllipse(centerX, centerY, radiusX, radiusY, pen);
				}
				var stroke = element.Style.Stroke;
				if (stroke != null && stroke.PaintType != SvgPaintType.None)
				{
					var pen = this.CreatePaint(session, area, stroke, element.Style.StrokeOpacity);
					var strokeWidth = element.Style.StrokeWidth;
					session.DrawEllipse(centerX, centerY, radiusX, radiusY, pen, strokeWidth.HasValue ? this.LengthConverter.Convert(strokeWidth.Value) : 1.0F);
				}
			}
		}

		protected override void RenderLine(CanvasDrawingSession session, SvgLineElement element)
		{
			var x1 = this.LengthConverter.ConvertX(element.X1);
			var y1 = this.LengthConverter.ConvertY(element.Y1);
			var x2 = this.LengthConverter.ConvertX(element.X2);
			var y2 = this.LengthConverter.ConvertY(element.Y2);

			using (var t = TransformSession.CreateTransformSession(session, element.Transform.Result))
			{
				var area = new Rect(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(y2 - x1), Math.Abs(y2 - y1));
				var stroke = element.Style.Stroke;
				if (stroke != null && stroke.PaintType != SvgPaintType.None)
				{
					var pen = this.CreatePaint(session, area, stroke, element.Style.StrokeOpacity);
					var width = element.Style.StrokeWidth;
					session.DrawLine(x1, y1, x2, y2, pen, width.HasValue ? this.LengthConverter.Convert(width.Value) : 1.0F);
				}
			}
		}

		protected override void RenderPolyline(CanvasDrawingSession session, SvgPolylineElement element)
		{
			var minX = element.Points.Min(p => p.X);
			var minY = element.Points.Min(p => p.Y);
			var maxX = element.Points.Max(p => p.X);
			var maxY = element.Points.Max(p => p.Y);

			using (var t = TransformSession.CreateTransformSession(session, element.Transform.Result))
			{
				var area = new Rect(minX, minY, maxX - minX, maxY - minY);
				var geometry = CanvasGeometry.CreatePolygon(this.ResourceCreator, element.Points.Select(p => new Vector2((float)p.X, (float)p.Y)).ToArray());
				var pen = this.CreatePaint(session, area, element.Style.Stroke, element.Style.StrokeOpacity);
				session.DrawGeometry(geometry, pen);
			}
		}

		protected override void RenderPolygon(CanvasDrawingSession session, SvgPolygonElement element)
		{
			var minX = element.Points.Min(p => p.X);
			var minY = element.Points.Min(p => p.Y);
			var maxX = element.Points.Max(p => p.X);
			var maxY = element.Points.Max(p => p.Y);

			using (var t = TransformSession.CreateTransformSession(session, element.Transform.Result))
			{
				var geometry = CanvasGeometry.CreatePolygon(this.ResourceCreator, element.Points.Select(p => new Vector2((float)p.X, (float)p.Y)).ToArray());
				var pen = this.CreatePaint(session, new Rect(minX, minY, maxX - minX, maxY - minY), element.Style.Fill, element.Style.FillOpacity);
				session.FillGeometry(geometry, pen);
			}
		}

		private CanvasSolidColorBrush CreateColor(CanvasDrawingSession session, SvgColor color)
		{
			var brush = new CanvasSolidColorBrush(this.ResourceCreator,
				color != null
					? Color.FromArgb(0xff, color.RgbColor.Red, color.RgbColor.Green, color.RgbColor.Blue)
					: Color.FromArgb(0xff, 0, 0, 0));
			this.DisposableObjects.Add(brush);
			return brush;
		}

		private ICanvasBrush CreatePaint(CanvasDrawingSession session, Rect area, SvgPaint paint, SvgNumber? opacity)
		{
			if (paint == null || paint.PaintType == SvgPaintType.RgbColor)
			{
				var alpha = (byte)(255.0F * (opacity?.Value ?? 1.0F));
				var brush = new CanvasSolidColorBrush(
					this.ResourceCreator,
					paint != null
						? Color.FromArgb(alpha, paint.RgbColor.Red, paint.RgbColor.Green, paint.RgbColor.Blue)
						: Color.FromArgb(alpha, 0, 0, 0));
				this.DisposableObjects.Add(brush);
				return brush;
			}
			if (paint.PaintType == SvgPaintType.Uri && paint.Uri[0] == '#')
			{
				var key = paint.Uri.Substring(1);
				var grad = this.TargetDocument.GetElementById(key);
				if (grad.GetType() == typeof(SvgLinearGradientElement))
				{
					return this.CreateLinearGradient(session, area, (SvgLinearGradientElement)grad);
				}
				if (grad.GetType() == typeof(SvgRadialGradientElement))
				{
					return this.CreateRadialGradient(session, area, (SvgRadialGradientElement)grad);
				}
			}
			throw new NotImplementedException();
		}

		private CanvasLinearGradientBrush CreateLinearGradient(CanvasDrawingSession session, Rect area, SvgLinearGradientElement element)
		{
			if (this.ResourceCache.ContainsKey(element)) return (CanvasLinearGradientBrush)this.ResourceCache[element];

			var stops = element.ChildNodes.Cast<SvgStopElement>().Select(s =>
			{
				var alpha = s.Style.StopOpacity.HasValue ? (byte)(255.0F * s.Style.StopOpacity.Value) : (byte)0xff;
				var stop = new CanvasGradientStop()
				{
					Position = s.Offset,
					Color = Color.FromArgb(alpha, s.Style.StopColor.RgbColor.Red, s.Style.StopColor.RgbColor.Green, s.Style.StopColor.RgbColor.Blue)
				};
				return stop;
			}).ToArray();

			var m = element.GradientTransform.Result;
			if (element.GradientUnits == SvgUnitType.ObjectBoundingBox)
			{
				m = new SvgMatrix(area.Width, 0.0, 0.0, area.Height, area.X, area.Y) * m;
			}
			var transform = new Matrix3x2((float)m.A, (float)m.B, (float)m.C, (float)m.D, (float)m.E, (float)m.F);

			var x1 = this.LengthConverter.ConvertX(element.X1);
			var y1 = this.LengthConverter.ConvertY(element.Y1);
			var x2 = this.LengthConverter.ConvertX(element.X2);
			var y2 = this.LengthConverter.ConvertY(element.Y2);
			var spreadMethod = GetSpreadMethod(element.SpreadMethod);
			var brush = new CanvasLinearGradientBrush(this.ResourceCreator, stops, spreadMethod, CanvasAlphaMode.Straight)
			{
				StartPoint = new Vector2(x1, y1),
				EndPoint = new Vector2(x2, y2),
				Transform = transform,
			};
			this.DisposableObjects.Add(brush);
			this.ResourceCache.Add(element, brush);
			return brush;
		}

		private CanvasRadialGradientBrush CreateRadialGradient(CanvasDrawingSession session, Rect area, SvgRadialGradientElement element)
		{
			if (this.ResourceCache.ContainsKey(element)) return (CanvasRadialGradientBrush)this.ResourceCache[element];

			var stops = element.ChildNodes.Cast<SvgStopElement>().Select(s =>
			{
				var alpha = s.Style.StopOpacity.HasValue ? (byte)(255.0F * s.Style.StopOpacity.Value) : (byte)0xff;
				var stop = new CanvasGradientStop()
				{
					Position = s.Offset,
					Color = Color.FromArgb(alpha, s.Style.StopColor.RgbColor.Red, s.Style.StopColor.RgbColor.Green, s.Style.StopColor.RgbColor.Blue)
				};
				return stop;
			}).ToArray();

			var m = element.GradientTransform.Result;
			if (element.GradientUnits == SvgUnitType.ObjectBoundingBox)
			{
				m = new SvgMatrix(area.Width, 0.0, 0.0, area.Height, area.X, area.Y) * m;
			}
			var transform = new Matrix3x2((float)m.A, (float)m.B, (float)m.C, (float)m.D, (float)m.E, (float)m.F);

			var centerX = this.LengthConverter.ConvertX(element.CenterX);
			var centerY = this.LengthConverter.ConvertY(element.CenterY);
			var focusX = this.LengthConverter.ConvertX(element.FocusX);
			var focusY = this.LengthConverter.ConvertY(element.FocusY);
			var radiusX = this.LengthConverter.ConvertX(element.Radius);
			var radiusY = this.LengthConverter.ConvertY(element.Radius);
			var spreadMethod = GetSpreadMethod(element.SpreadMethod);
			var brush = new CanvasRadialGradientBrush(this.ResourceCreator, stops, spreadMethod, CanvasAlphaMode.Straight)
			{
				OriginOffset = new Vector2(focusX - centerX, focusY - centerY),
				Center = new Vector2(centerX, centerY),
				RadiusX = radiusX,
				RadiusY = radiusY,
				Transform = transform,
			};
			this.DisposableObjects.Add(brush);
			this.ResourceCache.Add(element, brush);
			return brush;
		}

		private static CanvasEdgeBehavior GetSpreadMethod(SvgSpreadMethodType spreadMethod)
		{
			switch (spreadMethod)
			{
				case SvgSpreadMethodType.Reflect: return CanvasEdgeBehavior.Mirror;
				case SvgSpreadMethodType.Repeat: return CanvasEdgeBehavior.Wrap;
			}
			return CanvasEdgeBehavior.Clamp;
		}
	}
}