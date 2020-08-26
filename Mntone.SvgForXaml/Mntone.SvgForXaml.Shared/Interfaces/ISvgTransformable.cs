using Mntone.SvgForXaml.Primitives;

namespace Mntone.SvgForXaml.Interfaces
{
	public interface ISvgTransformable : ISvgLocatable
	{
		SvgTransformCollection Transform { get; }
	}
}
