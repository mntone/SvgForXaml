using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mntone.SvgForXaml.Primitives
{
	public sealed class SvgLengthCollection : IReadOnlyCollection<SvgLength>
	{
		private readonly Collection<SvgLength> _lengths;

		internal SvgLengthCollection(string attributeValue)
		{
			var lengths = attributeValue.Split(new[] { ' ', '\n', '\r', '\t' }).Where(s => !string.IsNullOrWhiteSpace(s)).Select(p => SvgLength.Parse(p));

			this._lengths = new Collection<SvgLength>();
			foreach (var length in lengths) this._lengths.Add(length);
		}

		public int Count => this._lengths.Count;
		public IEnumerator<SvgLength> GetEnumerator() => this._lengths.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this._lengths).GetEnumerator();
	}
}