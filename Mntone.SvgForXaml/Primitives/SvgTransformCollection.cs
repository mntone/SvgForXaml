using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mntone.SvgForXaml.Primitives
{
	public sealed class SvgTransformCollection : IReadOnlyCollection<SvgTransform>
	{
		private readonly ICollection<SvgTransform> _transforms;

		internal SvgTransformCollection(ICollection<SvgTransform> transforms)
		{
			this._transforms = transforms;
		}

		internal void Add(SvgTransform transfrom) => this._transforms.Add(transfrom);

		internal SvgTransformCollection DeepCopy()
		{
			return new SvgTransformCollection(this._transforms.Select(t => new SvgTransform(t)).ToList());
		}

		internal SvgMatrix Result
		{
			get
			{
				if (this._transforms.Count == 0) return SvgMatrix.Indentity;

				var m = this._transforms.First().Matrix;
				foreach (var m2 in this._transforms.Skip(1)) m = m * m2.Matrix;
				return m;
			}
		}

		public int Count => this._transforms.Count;
		public IEnumerator<SvgTransform> GetEnumerator() => this._transforms.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this._transforms).GetEnumerator();
	}
}