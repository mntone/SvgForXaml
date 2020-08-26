using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Mntone.SvgForXaml.Filters;
using Mntone.SvgForXaml.Gradients;
using Mntone.SvgForXaml.Maskings;
using Mntone.SvgForXaml.Path;
using Mntone.SvgForXaml.Primitives;
using Mntone.SvgForXaml.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.UI;

#if WINDOWS_UWP
using System.Numerics;
using Windows.Graphics.Effects;
#else
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Numerics;
#endif

namespace Mntone.SvgForXaml
{
	public class Win2dRenderer : SvgRendererBase<CanvasDrawingSession>, IDisposable
	{
		protected ICanvasResourceCreator ResourceCreator { get; }
		protected Collection<IDisposable> DisposableObjects { get; }
		protected Dictionary<int, CanvasSolidColorBrush> SolidResourceCache { get; }
		protected Dictionary<SvgGradientElement, ICanvasBrush> ResourceCache { get; }
		protected Dictionary<SvgPathElement, CanvasGeometry> PathCache { get; }
		protected Dictionary<SvgFilterElement, IGraphicsEffectSource> FilterCache { get; }

		private bool _disposed = false;

		public Win2dRenderer(ICanvasResourceCreator resourceCreator, SvgDocument targetDocument)
			: base(targetDocument)
		{
			this.ResourceCreator = resourceCreator;
			this.DisposableObjects = new Collection<IDisposable>();
			this.SolidResourceCache = new Dictionary<int, CanvasSolidColorBrush>();
			this.ResourceCache = new Dictionary<SvgGradientElement, ICanvasBrush>();
			this.PathCache = new Dictionary<SvgPathElement, CanvasGeometry>();
			this.FilterCache = new Dictionary<SvgFilterElement, IGraphicsEffectSource>();
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

		private void RenderGeometory(CanvasDrawingSession session, CanvasGeometry geometry, SvgMatrix transform, CssStyleDeclaration style)
		{
			bool change = false;
			var geometry2 = geometry;
			CanvasSolidColorBrush opacityBrush = null;
			try
			{
				using (var t = TransformSession.CreateTransformSession(session, transform))
				{
					var clipPath = style.ClipPath;
					if (clipPath != null)
					{
						if (clipPath.Uri[0] != '#') throw new ArgumentException();
						var clipPathElement = (SvgClipPathElement)this.TargetDocument.GetElementById(clipPath.Uri.Substring(1));
						var clipGeometory = this.CreateClipPath(session, clipPathElement);
						geometry2 = geometry.CombineWith(
							clipGeometory,
							new Matrix3x2 { M11 = 1.0F, M12 = 0.0F, M21 = 0.0F, M22 = 1.0F, M31 = 0.0F, M32 = 0.0F },
							CanvasGeometryCombine.Intersect,
							CanvasGeometry.ComputeFlatteningTolerance(session.Dpi, 1.0F, session.Transform));
						change = true;
					}

					var area = geometry2.ComputeBounds();

					var opacity = style.Opacity;
					if (opacity != null)
					{
						opacityBrush = this.CreateOpacity(session, opacity.Value);
					}

					var fill = style.Fill;
					if (fill == null || fill != null && fill.PaintType != SvgPaintType.None)
					{
						var pen = this.CreatePaint(session, area, fill, style.FillOpacity, style);
						if (opacityBrush == null)
						{
							session.FillGeometry(geometry2, pen);
						}
						else
						{
							session.FillGeometry(geometry2, pen, opacityBrush);
						}
					}

					var stroke = style.Stroke;
					if (stroke != null && stroke.PaintType != SvgPaintType.None)
					{
						var pen = this.CreatePaint(session, area, stroke, style.StrokeOpacity, style);
						var strokeWidth = this.LengthConverter.Convert(style.StrokeWidth, 1.0F);
						using (var strokeStyle = this.CreateStrokeStyle(style))
						{
							session.DrawGeometry(geometry2, pen, strokeWidth, strokeStyle);
						}
					}
				}
			}
			finally
			{
				if (change) geometry2.Dispose();
				if (opacityBrush != null) opacityBrush.Dispose();
			}
		}

		protected override void RenderPath(CanvasDrawingSession session, SvgPathElement element)
		{
			var geometry = this.CreatePath(session, element); // NO Dispose!
			this.RenderGeometory(session, geometry, element.Transform.Result, element.Style);
		}

		protected override void RenderRect(CanvasDrawingSession session, SvgRectElement element)
		{
			var x = this.LengthConverter.ConvertX(element.X);
			var y = this.LengthConverter.ConvertY(element.Y);
			var width = this.LengthConverter.ConvertX(element.Width);
			var height = this.LengthConverter.ConvertY(element.Height);
			var rx = this.LengthConverter.ConvertX(element.RoundedX);
			var ry = this.LengthConverter.ConvertY(element.RoundedY);
			using (var geometry = CanvasGeometry.CreateRoundedRectangle(this.ResourceCreator, x, y, width, height, rx, ry))
			{
				this.RenderGeometory(session, geometry, element.Transform.Result, element.Style);
			}
		}

		protected override void RenderCircle(CanvasDrawingSession session, SvgCircleElement element)
		{
			var centerX = this.LengthConverter.ConvertX(element.CenterX);
			var centerY = this.LengthConverter.ConvertY(element.CenterY);
			var radiusX = this.LengthConverter.ConvertX(element.Radius);
			var radiusY = this.LengthConverter.ConvertY(element.Radius);
			using (var geometry = CanvasGeometry.CreateEllipse(this.ResourceCreator, centerX, centerY, radiusX, radiusY))
			{
				this.RenderGeometory(session, geometry, element.Transform.Result, element.Style);
			}
		}

		protected override void RenderEllipse(CanvasDrawingSession session, SvgEllipseElement element)
		{
			var centerX = this.LengthConverter.ConvertX(element.CenterX);
			var centerY = this.LengthConverter.ConvertY(element.CenterY);
			var radiusX = this.LengthConverter.ConvertX(element.RadiusX);
			var radiusY = this.LengthConverter.ConvertY(element.RadiusY);
			using (var geometry = CanvasGeometry.CreateEllipse(this.ResourceCreator, centerX, centerY, radiusX, radiusY))
			{
				this.RenderGeometory(session, geometry, element.Transform.Result, element.Style);
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
				var area = new Rect(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x2 - x1), Math.Abs(y2 - y1));
				var stroke = element.Style.Stroke;
				if (stroke != null && stroke.PaintType != SvgPaintType.None)
				{
					var pen = this.CreatePaint(session, area, stroke, element.Style.StrokeOpacity, element.Style);
					var strokeWidth = this.LengthConverter.Convert(element.Style.StrokeWidth, 1.0F);
					using (var strokeStyle = this.CreateStrokeStyle(element.Style))
					{
						session.DrawLine(x1, y1, x2, y2, pen, strokeWidth, strokeStyle);
					}
				}
			}
		}

		protected override void RenderPolyline(CanvasDrawingSession session, SvgPolylineElement element)
		{
			using (var builder = new CanvasPathBuilder(this.ResourceCreator))
			{
				var begin = element.Points.First();
				builder.BeginFigure(begin.X, begin.Y);
				foreach (var point in element.Points.Skip(1))
				{
					builder.AddLine(point.X, point.Y);
				}
				builder.EndFigure(CanvasFigureLoop.Open);

				using (var geometry = CanvasGeometry.CreatePath(builder))
				{
					this.RenderGeometory(session, geometry, element.Transform.Result, element.Style);
				}
			}
		}

		protected override void RenderPolygon(CanvasDrawingSession session, SvgPolygonElement element)
		{
			using (var geometry = CanvasGeometry.CreatePolygon(this.ResourceCreator, element.Points.Select(p => new Vector2 { X = p.X, Y = p.Y }).ToArray()))
			{
				this.RenderGeometory(session, geometry, element.Transform.Result, element.Style);
			}
		}

		private CanvasGeometry CreatePath(CanvasDrawingSession session, SvgPathElement element, bool isClip = false)
		{
			if (this.PathCache.ContainsKey(element)) return this.PathCache[element];

			var open = false;
			var startPoint = new Vector2 { X = 0.0F, Y = 0.0F };
			var v = new Vector2 { X = 0.0F, Y = 0.0F };
			var prevC1 = v;
			var prevC2 = v;

			CanvasGeometry geometry;
			using (var builder = new CanvasPathBuilder(this.ResourceCreator))
			{
				var fillRule = isClip ? element.Style.ClipRule : element.Style.FillRule;
				if (fillRule.HasValue && fillRule.Value.Value != SvgFillRuleType.EvenOdd)
				{
					builder.SetFilledRegionDetermination(CanvasFilledRegionDetermination.Winding);
				}
				foreach (var segment in element.Segments)
				{
					if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.ClosePath)
					{
						builder.EndFigure(CanvasFigureLoop.Closed);
						v = startPoint;
						open = false;
						continue;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.MoveToAbsolute)
					{
						if (open) builder.EndFigure(CanvasFigureLoop.Open);

						var casted = (SvgPathSegmentMoveToAbsolute)segment;
						v.X = casted.X;
						v.Y = casted.Y;
						startPoint = v;
						builder.BeginFigure(v);
						prevC1 = v;
						prevC2 = v;
						open = true;
						continue;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.MoveToRelative)
					{
						if (open) builder.EndFigure(CanvasFigureLoop.Open);

						var casted = (SvgPathSegmentMoveToRelative)segment;
						v.X += casted.X;
						v.Y += casted.Y;
						startPoint = v;
						builder.BeginFigure(v);
						prevC1 = v;
						prevC2 = v;
						open = true;
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
						prevC1 = v;
						prevC2 = v;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.LineToRelative)
					{
						var casted = (SvgPathSegmentLineToRelative)segment;
						v.X += casted.X;
						v.Y += casted.Y;
						builder.AddLine(v);
						prevC1 = v;
						prevC2 = v;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToCubicAbsolute)
					{
						var casted = (SvgPathSegmentCurveToCubicAbsolute)segment;
						var c2 = new Vector2 { X = casted.X2, Y = casted.Y2 };
						v.X = casted.X;
						v.Y = casted.Y;
						builder.AddCubicBezier(new Vector2 { X = casted.X1, Y = casted.Y1 }, c2, v);
						prevC1 = v;
						prevC2 = c2;
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
						prevC1 = v;
						prevC2 = c2;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToQuadraticAbsolute)
					{
						var casted = (SvgPathSegmentCurveToQuadraticAbsolute)segment;
						var c1 = new Vector2 { X = casted.X1, Y = casted.Y1 };
						v.X = casted.X;
						v.Y = casted.Y;
						builder.AddQuadraticBezier(c1, v);
						prevC1 = c1;
						prevC2 = v;
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
						prevC1 = c1;
						prevC2 = v;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.ArcAbsolute)
					{
						var casted = (SvgPathSegmentArcAbsolute)segment;
						v.X = casted.X;
						v.Y = casted.Y;
						var size = casted.LargeArcFlag ? CanvasArcSize.Large : CanvasArcSize.Small;
						var sweepDirection = casted.SweepFlag ? CanvasSweepDirection.Clockwise : CanvasSweepDirection.CounterClockwise;
						builder.AddArc(v, casted.RadiusX, casted.RadiusY, 180.0F * casted.Angle / (float)Math.PI, sweepDirection, size);
						prevC1 = v;
						prevC2 = v;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.ArcRelative)
					{
						var casted = (SvgPathSegmentArcRelative)segment;
						v.X += casted.X;
						v.Y += casted.Y;
						var size = casted.LargeArcFlag ? CanvasArcSize.Large : CanvasArcSize.Small;
						var sweepDirection = casted.SweepFlag ? CanvasSweepDirection.Clockwise : CanvasSweepDirection.CounterClockwise;
						builder.AddArc(v, casted.RadiusX, casted.RadiusY, 180.0F * casted.Angle / (float)Math.PI, sweepDirection, size);
						prevC1 = v;
						prevC2 = v;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.LineToHorizontalAbsolute)
					{
						var casted = (SvgPathSegmentLineToHorizontalAbsolute)segment;
						v.X = casted.X;
						builder.AddLine(v);
						prevC1 = v;
						prevC2 = v;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.LineToHorizontalRelative)
					{
						var casted = (SvgPathSegmentLineToHorizontalRelative)segment;
						v.X += casted.X;
						builder.AddLine(v);
						prevC1 = v;
						prevC2 = v;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.LineToVerticalAbsolute)
					{
						var casted = (SvgPathSegmentLineToVerticalAbsolute)segment;
						v.Y = casted.Y;
						builder.AddLine(v);
						prevC1 = v;
						prevC2 = v;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.LineToVerticalRelative)
					{
						var casted = (SvgPathSegmentLineToVerticalRelative)segment;
						v.Y += casted.Y;
						builder.AddLine(v);
						prevC1 = v;
						prevC2 = v;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToCubicSmoothAbsolute)
					{
						var casted = (SvgPathSegmentCurveToCubicSmoothAbsolute)segment;
#if WINDOWS_UWP
						var c1 = 2 * v - prevC2;
#else
						var c1 = new Vector2 { X = 2 * v.X - prevC2.X, Y = 2 * v.Y - prevC2.Y };
#endif
						var c2 = new Vector2 { X = casted.X2, Y = casted.Y2 };
						v.X = casted.X;
						v.Y = casted.Y;
						builder.AddCubicBezier(c1, c2, v);
						prevC1 = v;
						prevC2 = c2;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToCubicSmoothRelative)
					{
						var casted = (SvgPathSegmentCurveToCubicSmoothRelative)segment;
#if WINDOWS_UWP
						var c1 = 2 * v - prevC2;
#else
						var c1 = new Vector2 { X = 2 * v.X - prevC2.X, Y = 2 * v.Y - prevC2.Y };
#endif
						var c2 = v;
						c2.X += casted.X2;
						c2.Y += casted.Y2;
						v.X += casted.X;
						v.Y += casted.Y;
						builder.AddCubicBezier(c1, c2, v);
						prevC1 = v;
						prevC2 = c2;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToQuadraticSmoothAbsolute)
					{
						var casted = (SvgPathSegmentCurveToQuadraticSmoothAbsolute)segment;
#if WINDOWS_UWP
						var c1 = 2 * v - prevC1;
#else
						var c1 = new Vector2 { X = 2 * v.X - prevC1.X, Y = 2 * v.Y - prevC1.Y };
#endif
						v.X = casted.X;
						v.Y = casted.Y;
						builder.AddQuadraticBezier(c1, v);
						prevC1 = c1;
						prevC2 = v;
					}
					else if (segment.PathSegmentType == SvgPathSegment.SvgPathSegmentType.CurveToQuadraticSmoothRelative)
					{
						var casted = (SvgPathSegmentCurveToQuadraticSmoothRelative)segment;
#if WINDOWS_UWP
						var c1 = 2 * v - prevC1;
#else
						var c1 = new Vector2 { X = 2 * v.X - prevC1.X, Y = 2 * v.Y - prevC1.Y };
#endif
						v.X += casted.X;
						v.Y += casted.Y;
						builder.AddQuadraticBezier(c1, v);
						prevC1 = c1;
						prevC2 = v;
					}
				}
				if (open)
				{
					builder.EndFigure(CanvasFigureLoop.Open);
				}

				geometry = CanvasGeometry.CreatePath(builder);
			}
			this.DisposableObjects.Add(geometry);
			this.PathCache.Add(element, geometry);
			return geometry;
		}

		private CanvasGeometry CreateClipPath(CanvasDrawingSession session, SvgClipPathElement element)
		{
			var child = element.FirstChild;
			if (child.GetType() == typeof(SvgUseElement))
			{
				child = ((SvgUseElement)child).InstanceRoot;
			}
			if (child.GetType() != typeof(SvgPathElement)) throw new ArgumentException();

			return this.CreatePath(session, (SvgPathElement)child);
		}

		private CanvasSolidColorBrush CreateColor(CanvasDrawingSession session, SvgColor color)
		{
			var code = (int)(0xff000000 | (uint)(color.RgbColor.Red << 16 | color.RgbColor.Green << 8 | color.RgbColor.Blue));
			if (this.SolidResourceCache.ContainsKey(code)) return this.SolidResourceCache[code];

			var brush = new CanvasSolidColorBrush(this.ResourceCreator, color?.ToPlatformColor(0xff) ?? Color.FromArgb(0xff, 0, 0, 0));
			this.DisposableObjects.Add(brush);
			this.SolidResourceCache.Add(code, brush);
			return brush;
		}

		private CanvasSolidColorBrush CreateOpacity(CanvasDrawingSession session, SvgNumber opacity)
		{
			return new CanvasSolidColorBrush(this.ResourceCreator, Color.FromArgb((byte)(255.0F * opacity.Value), 0, 0, 0));
		}

		private ICanvasBrush CreatePaint(CanvasDrawingSession session, Rect area, SvgPaint paint, SvgNumber? opacity, CssStyleDeclaration style)
		{
			if (paint == null || paint.PaintType == SvgPaintType.RgbColor)
			{
				var alpha = (byte)(255.0F * (opacity?.Value ?? 1.0F));
				var code = paint == null ? alpha << 24 : alpha << 24 | paint.RgbColor.Red << 16 | paint.RgbColor.Green << 8 | paint.RgbColor.Blue;
				if (this.SolidResourceCache.ContainsKey(code)) return this.SolidResourceCache[code];

				var brush = new CanvasSolidColorBrush(this.ResourceCreator, paint?.ToPlatformColor(alpha) ?? Color.FromArgb(alpha, 0, 0, 0));
				this.DisposableObjects.Add(brush);
				this.SolidResourceCache.Add(code, brush);
				return brush;
			}
			if (paint.PaintType == SvgPaintType.CurrentColor)
			{
				var color = style.Color;
				var alpha = (byte)(255.0F * (opacity?.Value ?? 1.0F));
				var code = alpha << 24 | color.RgbColor.Red << 16 | color.RgbColor.Green << 8 | color.RgbColor.Blue;
				if (this.SolidResourceCache.ContainsKey(code)) return this.SolidResourceCache[code];

				var brush = new CanvasSolidColorBrush(this.ResourceCreator, color.ToPlatformColor(alpha));
				this.DisposableObjects.Add(brush);
				this.SolidResourceCache.Add(code, brush);
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
					Color = s.Style.StopColor.StopColorType == SvgStopColorType.CurrentColor
						? s.Style.Color.ToPlatformColor(alpha)
						: s.Style.StopColor?.ToPlatformColor(alpha) ?? Color.FromArgb(alpha, 0, 0, 0)
				};
				return stop;
			}).ToArray();

			var m = element.GradientTransform.Result;
			var transform = new Matrix3x2 { M11 = (float)m.A, M12 = (float)m.B, M21 = (float)m.C, M22 = (float)m.D, M31 = (float)m.E, M32 = (float)m.F };

			float x1, y1, x2, y2;
			if (element.GradientUnits != SvgUnitType.UserSpaceOnUse)
			{
				x1 = this.LengthConverter.ConvertXForOBBU(element.X1, (float)area.X, (float)area.Width);
				y1 = this.LengthConverter.ConvertYForOBBU(element.Y1, (float)area.Y, (float)area.Height);
				x2 = this.LengthConverter.ConvertXForOBBU(element.X2, (float)area.X, (float)area.Width);
				y2 = this.LengthConverter.ConvertYForOBBU(element.Y2, (float)area.Y, (float)area.Height);
			}
			else
			{
				x1 = this.LengthConverter.ConvertX(element.X1);
				y1 = this.LengthConverter.ConvertY(element.Y1);
				x2 = this.LengthConverter.ConvertX(element.X2);
				y2 = this.LengthConverter.ConvertY(element.Y2);
			}
			var spreadMethod = GetSpreadMethod(element.SpreadMethod);
			var brush = new CanvasLinearGradientBrush(this.ResourceCreator, stops, spreadMethod, CanvasAlphaMode.Straight)
			{
				StartPoint = new Vector2 { X = x1, Y = y1 },
				EndPoint = new Vector2 { X = x2, Y = y2 },
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
					Color = s.Style.StopColor.StopColorType == SvgStopColorType.CurrentColor
						? s.Style.Color.ToPlatformColor(alpha)
						: s.Style.StopColor?.ToPlatformColor(alpha) ?? Color.FromArgb(alpha, 0, 0, 0)
				};
				return stop;
			}).ToArray();

			var m = element.GradientTransform.Result;
			var transform = new Matrix3x2 { M11 = (float)m.A, M12 = (float)m.B, M21 = (float)m.C, M22 = (float)m.D, M31 = (float)m.E, M32 = (float)m.F };

			float centerX, centerY, focusX, focusY, radiusX, radiusY;
			if (element.GradientUnits != SvgUnitType.UserSpaceOnUse)
			{
				centerX = this.LengthConverter.ConvertXForOBBU(element.CenterX, (float)area.X, (float)area.Width);
				centerY = this.LengthConverter.ConvertYForOBBU(element.CenterY, (float)area.Y, (float)area.Height);
				focusX  = this.LengthConverter.ConvertXForOBBU(element.FocusX, (float)area.X, (float)area.Width);
				focusY  = this.LengthConverter.ConvertYForOBBU(element.FocusY, (float)area.Y, (float)area.Height);
				radiusX = this.LengthConverter.ConvertXForOBBU(element.Radius, (float)area.X, (float)area.Width);
				radiusY = this.LengthConverter.ConvertYForOBBU(element.Radius, (float)area.Y, (float)area.Height);
			}
			else
			{
				centerX = this.LengthConverter.ConvertX(element.CenterX);
				centerY = this.LengthConverter.ConvertY(element.CenterY);
				focusX = this.LengthConverter.ConvertX(element.FocusX);
				focusY = this.LengthConverter.ConvertY(element.FocusY);
				radiusX = this.LengthConverter.ConvertX(element.Radius);
				radiusY = this.LengthConverter.ConvertY(element.Radius);
			}
			var spreadMethod = GetSpreadMethod(element.SpreadMethod);
			var brush = new CanvasRadialGradientBrush(this.ResourceCreator, stops, spreadMethod, CanvasAlphaMode.Straight)
			{
				OriginOffset = new Vector2 { X = focusX - centerX, Y = focusY - centerY },
				Center = new Vector2 { X = centerX, Y = centerY },
				RadiusX = radiusX,
				RadiusY = radiusY,
				Transform = transform,
			};
			this.DisposableObjects.Add(brush);
			this.ResourceCache.Add(element, brush);
			return brush;
		}

		private CanvasStrokeStyle CreateStrokeStyle(CssStyleDeclaration style)
		{
			var lineCap = GetStrokeLineCap(style.StrokeLineCap);
			return new CanvasStrokeStyle()
			{
				StartCap = lineCap,
				EndCap = lineCap,
				LineJoin = GetStrokeLineJoin(style.StrokeLineJoin),
				MiterLimit = style.StrokeMiterLimit?.Value ?? 4.0F,
			};
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

		private static CanvasCapStyle GetStrokeLineCap(SvgStrokeLineCap? strokeLineCap)
			=> strokeLineCap.HasValue ? GetStrokeLineCap(strokeLineCap.Value.Value) : CanvasCapStyle.Flat;
		private static CanvasCapStyle GetStrokeLineCap(SvgStrokeLineCapType strokeLineCap)
		{
			switch (strokeLineCap)
			{
				case SvgStrokeLineCapType.Round: return CanvasCapStyle.Round;
				case SvgStrokeLineCapType.Square: return CanvasCapStyle.Square;
			}
			return CanvasCapStyle.Flat;
		}

		private static CanvasLineJoin GetStrokeLineJoin(SvgStrokeLineJoin? strokeLineJoin)
			=> strokeLineJoin.HasValue ? GetStrokeLineJoin(strokeLineJoin.Value.Value) : CanvasLineJoin.MiterOrBevel;
		private static CanvasLineJoin GetStrokeLineJoin(SvgStrokeLineJoinType strokeLineCap)
		{
			switch (strokeLineCap)
			{
				case SvgStrokeLineJoinType.Miter: return CanvasLineJoin.MiterOrBevel;
				case SvgStrokeLineJoinType.Round: return CanvasLineJoin.Round;
				case SvgStrokeLineJoinType.Bevel: return CanvasLineJoin.Bevel;
			}
			return CanvasLineJoin.MiterOrBevel;
		}
	}
}
