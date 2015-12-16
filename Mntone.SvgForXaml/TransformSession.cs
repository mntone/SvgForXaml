using Microsoft.Graphics.Canvas;
using Mntone.SvgForXaml.Primitives;
using System;
using System.Numerics;

namespace Mntone.SvgForXaml
{
	internal sealed class TransformSession : IDisposable
	{
		private bool _disposed = false;

		public CanvasDrawingSession Session { get; }
		public Matrix3x2 OldTransform { get; }

		private TransformSession(CanvasDrawingSession session, Matrix3x2 oldTransform, SvgMatrix multiply)
		{
			this.Session = session;
			this.OldTransform = oldTransform;

			var transform = new Matrix3x2((float)multiply.A, (float)multiply.B, (float)multiply.C, (float)multiply.D, (float)multiply.E, (float)multiply.F);
			session.Transform = transform * session.Transform;
		}

		public void Dispose() => this.Dispose(true);
		private void Dispose(bool disposing)
		{
			if (this._disposed) return;
			if (disposing)
			{
				this.Session.Transform = this.OldTransform;
			}
			this._disposed = true;
		}

		public static TransformSession CreateTransformSession(CanvasDrawingSession session, SvgMatrix multiply)
		{
			return new TransformSession(session, session.Transform, multiply);
		}
	}
}
