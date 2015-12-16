using Mntone.SvgForXaml.Interfaces;
using System;

namespace Mntone.SvgForXaml.Primitives
{
	public sealed class SvgIri : ICssValue
	{
		public SvgIri(string iri)
		{
			if (iri.StartsWith("url("))
			{
				this.Uri = iri.Substring(4, iri.Length - 5);
				return;
			}
			throw new ArgumentException();
		}

		public string Uri { get; }
	}
}