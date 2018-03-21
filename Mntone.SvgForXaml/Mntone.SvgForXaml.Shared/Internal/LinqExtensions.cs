using System;
using System.Collections.Generic;
using System.Linq;

namespace Mntone.SvgForXaml.Internal
{
    internal static class LinqExtensions
    {
		public static int FindIndex<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.Select((s, index) => new { Element = s, Index = index })
				.First(e => predicate(e.Element)).Index;
		}

		public static TSource PreviousOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.ElementAtOrDefault(source.FindIndex(predicate) - 1);
		}

		public static TSource NextOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			return source.ElementAtOrDefault(source.FindIndex(predicate) + 1);
		}
	}
}