using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mntone.SvgForXaml.Primitives
{
	public sealed class SvgPointCollection : IReadOnlyCollection<SvgPoint>
	{
		private readonly Collection<SvgPoint> _points;

		internal SvgPointCollection(string attributeValue)
		{
			var points = attributeValue.Split(new[] { ' ', '\n', '\r', '\t' }).Where(s => !string.IsNullOrWhiteSpace(s)).Select(p => SvgPoint.Parse(p));

			this._points = new Collection<SvgPoint>();
			foreach (var point in points) this._points.Add(point);
		}

		public int Count => this._points.Count;
		public IEnumerator<SvgPoint> GetEnumerator() => this._points.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this._points).GetEnumerator();
	}
}