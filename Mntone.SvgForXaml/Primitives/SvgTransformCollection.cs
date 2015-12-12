using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mntone.SvgForXaml.Primitives
{
	public sealed class SvgTransformCollection : IReadOnlyCollection<SvgTransform>
	{
		private readonly Collection<SvgTransform> _transforms;

		internal SvgTransformCollection(Collection<SvgTransform> transforms)
		{
			this._transforms = transforms;
		}

		internal SvgMatrix Result
		{
			get
			{
				if (this._transforms.Count == 0) return SvgMatrix.Indentity;

				var m = this._transforms[0].Matrix;
				foreach (var m2 in this._transforms.Skip(1)) m = m2.Matrix * m;
				return m;
			}
		}

		public int Count => this._transforms.Count;
		public IEnumerator<SvgTransform> GetEnumerator() => this._transforms.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this._transforms).GetEnumerator();
	}
}