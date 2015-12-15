using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
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
	public class WinRtWin2dRenderer : SvgRendererBase<CanvasControl, CanvasDrawEventArgs>, IDisposable
	{
		protected SvgLengthConverter LengthConverter { get; }
		protected Collection<IDisposable> DisposableObjects { get; }
		protected Dictionary<SvgGradientElement, ICanvasBrush> ResourceCache { get; }

		private bool _disposed = false;

		public WinRtWin2dRenderer(CanvasControl rendererTarget, SvgDocument targetDocument)
			: base(rendererTarget, targetDocument)
		{
			this.LengthConverter = new SvgLengthConverter();
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

		public override void Renderer(CanvasDrawEventArgs session)
		{
			if (this.RendererTarget == null) return;
			this.LengthConverter.CanvasSize = new Size(this.RendererTarget.ActualWidth, this.RendererTarget.ActualHeight);

			var root = this.TargetDocument.RootElement;
			this.RendererSvg(session, root);
		}

		protected override void RendererSvg(CanvasDrawEventArgs session, SvgSvgElement element)
		{
			this.RendererChildren(session, element.ChildNodes);
		}

		protected override void RendererPath(CanvasDrawEventArgs session, SvgPathElement element)
		{
			double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;

			var open = false;
			var v = new Vector2(0.0F, 0.0F);
			var builder = new CanvasPathBuilder(this.RendererTarget);
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

			var area = new Rect(minX, minY, maxX - minX, maxY - minY);
			var geometry = CanvasGeometry.CreatePath(builder);
			var fill = element.Style.Fill;
			if (fill == null || fill != null && fill.PaintType != SvgPaintType.None)
			{
				var pen = this.CreatePaint(session, area, fill, element.Style.FillOpacity);
				session.DrawingSession.FillGeometry(geometry, pen);
			}
			var stroke = element.Style.Stroke;
			if (stroke != null && fill.PaintType != SvgPaintType.None)
			{
				var pen = this.CreatePaint(session, area, stroke, element.Style.StrokeOpacity);
				var width = element.Style.StrokeWidth;
				session.DrawingSession.DrawGeometry(geometry, pen, width.HasValue ? this.LengthConverter.Convert(width.Value) : 1.0F);
			}
		}

		protected override void RendererRect(CanvasDrawEventArgs session, SvgRectElement element)
		{
			var x = this.LengthConverter.ConvertX(element.X);
			var y = this.LengthConverter.ConvertY(element.Y);
			var width = this.LengthConverter.ConvertX(element.Width);
			var height = this.LengthConverter.ConvertY(element.Height);

			var area = new Rect(x, y, width, height);
			var fill = element.Style.Fill;
			if (fill == null || fill != null && fill.PaintType != SvgPaintType.None)
			{
				var pen = this.CreatePaint(session, area, fill, element.Style.FillOpacity);
				session.DrawingSession.FillRectangle(x, y, width, height, pen);
			}
			var stroke = element.Style.Stroke;
			if (stroke != null && fill.PaintType != SvgPaintType.None)
			{
				var pen = this.CreatePaint(session, area, stroke, element.Style.StrokeOpacity);
				var strokeWidth = element.Style.StrokeWidth;
				session.DrawingSession.DrawRectangle(x, y, width, height, pen, strokeWidth.HasValue ? this.LengthConverter.Convert(strokeWidth.Value) : 1.0F);
			}
		}

		protected override void RendererCircle(CanvasDrawEventArgs session, SvgCircleElement element)
		{
			var centerX = this.LengthConverter.ConvertX(element.CenterX);
			var centerY = this.LengthConverter.ConvertY(element.CenterY);
			var radiusX = this.LengthConverter.ConvertX(element.Radius);
			var radiusY = this.LengthConverter.ConvertY(element.Radius);

			var area = new Rect(centerX - radiusX, centerY - radiusY, 2.0F * radiusX, 2.0F * radiusY);
			var fill = element.Style.Fill;
			if (fill == null || fill != null && fill.PaintType != SvgPaintType.None)
			{
				var pen = this.CreatePaint(session, area, fill, element.Style.FillOpacity);
				session.DrawingSession.FillEllipse(centerX, centerY, radiusX, radiusY, pen);
			}
			var stroke = element.Style.Stroke;
			if (stroke != null && stroke.PaintType != SvgPaintType.None)
			{
				var pen = this.CreatePaint(session, area, stroke, element.Style.StrokeOpacity);
				var strokeWidth = element.Style.StrokeWidth;
				session.DrawingSession.DrawEllipse(centerX, centerY, radiusX, radiusY, pen, strokeWidth.HasValue ? this.LengthConverter.Convert(strokeWidth.Value) : 1.0F);
			}
		}

		protected override void RendererEllipse(CanvasDrawEventArgs session, SvgEllipseElement element)
		{
			var centerX = this.LengthConverter.ConvertX(element.CenterX);
			var centerY = this.LengthConverter.ConvertY(element.CenterY);
			var radiusX = this.LengthConverter.ConvertX(element.RadiusX);
			var radiusY = this.LengthConverter.ConvertY(element.RadiusY);

			var area = new Rect(centerX - radiusX, centerY - radiusY, 2.0F * radiusX, 2.0F * radiusY);
			var fill = element.Style.Fill;
			if (fill == null || fill != null && fill.PaintType != SvgPaintType.None)
			{
				var pen = this.CreatePaint(session, area, fill, element.Style.FillOpacity);
				session.DrawingSession.FillEllipse(centerX, centerY, radiusX, radiusY, pen);
			}
			var stroke = element.Style.Stroke;
			if (stroke != null && stroke.PaintType != SvgPaintType.None)
			{
				var pen = this.CreatePaint(session, area, stroke, element.Style.StrokeOpacity);
				var strokeWidth = element.Style.StrokeWidth;
				session.DrawingSession.DrawEllipse(centerX, centerY, radiusX, radiusY, pen, strokeWidth.HasValue ? this.LengthConverter.Convert(strokeWidth.Value) : 1.0F);
			}
		}

		protected override void RendererLine(CanvasDrawEventArgs session, SvgLineElement element)
		{
			var x1 = this.LengthConverter.ConvertX(element.X1);
			var y1 = this.LengthConverter.ConvertY(element.Y1);
			var x2 = this.LengthConverter.ConvertX(element.X2);
			var y2 = this.LengthConverter.ConvertY(element.Y2);

			var area = new Rect(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(y2 - x1), Math.Abs(y2 - y1));
			var stroke = element.Style.Stroke;
			if (stroke != null && stroke.PaintType != SvgPaintType.None)
			{
				var pen = this.CreatePaint(session, area, stroke, element.Style.StrokeOpacity);
				var width = element.Style.StrokeWidth;
				session.DrawingSession.DrawLine(x1, y1, x2, y2, pen, width.HasValue ? this.LengthConverter.Convert(width.Value) : 1.0F);
			}
		}

		protected override void RendererPolyline(CanvasDrawEventArgs session, SvgPolylineElement element)
		{
			var minX = element.Points.Min(p => p.X);
			var minY = element.Points.Min(p => p.Y);
			var maxX = element.Points.Max(p => p.X);
			var maxY = element.Points.Max(p => p.Y);

			var area = new Rect(minX, minY, maxX - minX, maxY - minY);
			var geometry = CanvasGeometry.CreatePolygon(this.RendererTarget, element.Points.Select(p => new Vector2((float)p.X, (float)p.Y)).ToArray());
			var pen = this.CreatePaint(session, area, element.Style.Stroke, element.Style.StrokeOpacity);
			session.DrawingSession.DrawGeometry(geometry, pen);
		}

		protected override void RendererPolygon(CanvasDrawEventArgs session, SvgPolygonElement element)
		{
			var minX = element.Points.Min(p => p.X);
			var minY = element.Points.Min(p => p.Y);
			var maxX = element.Points.Max(p => p.X);
			var maxY = element.Points.Max(p => p.Y);

			var geometry = CanvasGeometry.CreatePolygon(this.RendererTarget, element.Points.Select(p => new Vector2((float)p.X, (float)p.Y)).ToArray());
			var pen = this.CreatePaint(session, new Rect(minX, minY, maxX - minX, maxY - minY), element.Style.Fill, element.Style.FillOpacity);
			session.DrawingSession.FillGeometry(geometry, pen);
		}

		private CanvasSolidColorBrush CreateColor(CanvasDrawEventArgs session, SvgColor color)
		{
			var brush = new CanvasSolidColorBrush(this.RendererTarget,
				color != null
					? Color.FromArgb(0xff, color.RgbColor.Red, color.RgbColor.Green, color.RgbColor.Blue)
					: Color.FromArgb(0xff, 0, 0, 0));
			this.DisposableObjects.Add(brush);
			return brush;
		}

		private ICanvasBrush CreatePaint(CanvasDrawEventArgs session, Rect area, SvgPaint paint, SvgNumber? opacity)
		{
			if (paint == null || paint.PaintType == SvgPaintType.RgbColor)
			{
				var alpha = (byte)(255.0F * (opacity?.Value ?? 1.0F));
				var brush = new CanvasSolidColorBrush(
					this.RendererTarget,
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

		private CanvasLinearGradientBrush CreateLinearGradient(CanvasDrawEventArgs session, Rect area, SvgLinearGradientElement element)
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
			var brush = new CanvasLinearGradientBrush(this.RendererTarget, stops, spreadMethod, CanvasAlphaMode.Straight)
			{
				StartPoint = new Vector2(x1, y1),
				EndPoint = new Vector2(x2, y2),
				Transform = transform,
			};
			this.DisposableObjects.Add(brush);
			this.ResourceCache.Add(element, brush);
			return brush;
		}

		private CanvasRadialGradientBrush CreateRadialGradient(CanvasDrawEventArgs session, Rect area, SvgRadialGradientElement element)
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
			var brush = new CanvasRadialGradientBrush(this.RendererTarget, stops, spreadMethod, CanvasAlphaMode.Straight)
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