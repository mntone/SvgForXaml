using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mntone.SvgForXaml.Path
{
	public sealed class SvgPathSegmentCollection : IReadOnlyCollection<SvgPathSegment>
	{
		private readonly Collection<SvgPathSegment> _segments;

		internal SvgPathSegmentCollection(Collection<SvgPathSegment> segments)
		{
			this._segments = segments;
		}

		public int Count => this._segments.Count;
		public IEnumerator<SvgPathSegment> GetEnumerator() => this._segments.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this._segments).GetEnumerator();
	}
}