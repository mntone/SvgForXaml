using Mntone.SvgForXaml.Interfaces;
using System;

namespace Mntone.SvgForXaml.Primitives
{
	public sealed class SvgIri : ICssValue
	{
		public SvgIri(string iri)
		{
			if (iri.StartsWith("url(") && iri[iri.Length - 1] == ')')
			{
				this.Uri = iri.Substring(4, iri.Length - 5);
				return;
			}
			throw new ArgumentException(nameof(iri));
		}

		public string Uri { get; }
	}
}