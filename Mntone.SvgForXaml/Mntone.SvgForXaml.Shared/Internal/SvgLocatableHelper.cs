using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Primitives;

namespace Mntone.SvgForXaml.Internal
{
	internal static class SvgLocatableHelper
	{
		public static SvgRect GetUnionBBox(SvgElement element)
		{
			float left = float.MaxValue, top = float.MinValue, right = float.MaxValue, bottom = float.MinValue;
			foreach (var child in element.ChildNodes)
			{
				var locatable = child as ISvgLocatable;
				if (locatable == null) continue;

				var bbox = locatable.GetBBox();
				if (bbox.X < left) left = bbox.X;

				var bboxRight = bbox.X + bbox.Width;
				if (bboxRight > right) right = bboxRight;

				if (bbox.Y < top) top = bbox.Y;

				var bboxBottom = bbox.Y + bbox.Height;
				if (bboxBottom > bottom) bottom = bboxBottom;
			}
			return new SvgRect(left, top, right - left, bottom - top);
		}
	}
}
