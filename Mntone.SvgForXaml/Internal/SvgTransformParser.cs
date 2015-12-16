using Mntone.SvgForXaml.Primitives;
using System;
using System.Collections.ObjectModel;

namespace Mntone.SvgForXaml.Internal
{
	internal sealed class SvgTransformParser
	{
		public static SvgTransformCollection Parse(string transformData) => new SvgTransformCollection(new SvgTransformParser(transformData).Transforms);

		public Collection<SvgTransform> Transforms { get; }

		private SvgTransformParser(string pathData)
		{
			this.Transforms = new Collection<SvgTransform>();

			var ptr = new StringPtr(pathData);
			ptr.AdvanceWhiteSpace();
			this.ParseTransforms(ptr);
		}

		private void ParseTransforms(StringPtr ptr)
		{
			this.ParseTransform(ptr);
			if (!ptr.IsEnd && this.ParseCommaOrWhitespace(ptr))
			{
				this.ParseTransforms(ptr);
			}
		}

		private void ParseTransform(StringPtr ptr)
		{
			this.ParseMatrix(ptr);
			this.ParseTranslate(ptr);
			this.ParseScale(ptr);
			this.ParseRotate(ptr);
			this.ParseSkewX(ptr);
			this.ParseSkewY(ptr);
		}

		private void ParseMatrix(StringPtr ptr)
		{
			if (ptr.String.StartsWith("matrix"))
			{
				ptr += 6;

				ptr.AdvanceWhiteSpace();
				if (ptr.Char == '(')
				{
					++ptr;
					ptr.AdvanceWhiteSpace();

					var a = ParseNumber(ptr).Value;
					this.ParseCommaOrWhitespace(ptr);

					var b = ParseNumber(ptr).Value;
					this.ParseCommaOrWhitespace(ptr);

					var c = ParseNumber(ptr).Value;
					this.ParseCommaOrWhitespace(ptr);

					var d = ParseNumber(ptr).Value;
					this.ParseCommaOrWhitespace(ptr);

					var e = ParseNumber(ptr).Value;
					this.ParseCommaOrWhitespace(ptr);

					var f = ParseNumber(ptr).Value;
					this.ParseCommaOrWhitespace(ptr);

					ptr.AdvanceWhiteSpace();
					if (ptr.Char == ')')
					{
						++ptr;
						this.Transforms.Add(SvgTransform.CreateMatrix(new SvgMatrix(a, b, c, d, e, f)));
						return;
					}
				}
				throw new ArgumentException("transformData");
			}
		}

		private void ParseTranslate(StringPtr ptr)
		{
			if (ptr.String.StartsWith("translate"))
			{
				ptr += 9;

				ptr.AdvanceWhiteSpace();
				if (ptr.Char == '(')
				{
					++ptr;
					ptr.AdvanceWhiteSpace();

					var tx = ParseNumber(ptr).Value;

					var current = ptr.Index;
					this.ParseCommaOrWhitespace(ptr);

					var ty = ParseNumber(ptr);
					if (ty.HasValue)
					{
						this.ParseCommaOrWhitespace(ptr);

						ptr.AdvanceWhiteSpace();
						if (ptr.Char == ')')
						{
							this.Transforms.Add(SvgTransform.CreateTranslate(tx, ty.Value));
							return;
						}
					}
					else
					{
						ptr.Index = current;

						ptr.AdvanceWhiteSpace();
						if (ptr.Char == ')')
						{
							++ptr;
							this.Transforms.Add(SvgTransform.CreateTranslate(tx, 0.0F));
							return;
						}
					}
				}
				throw new ArgumentException("transformData");
			}
		}

		private void ParseScale(StringPtr ptr)
		{
			if (ptr.String.StartsWith("scale"))
			{
				ptr += 5;

				ptr.AdvanceWhiteSpace();
				if (ptr.Char == '(')
				{
					++ptr;
					ptr.AdvanceWhiteSpace();

					var sx = ParseNumber(ptr).Value;

					var current = ptr.Index;
					this.ParseCommaOrWhitespace(ptr);

					var sy = ParseNumber(ptr);
					if (sy.HasValue)
					{
						this.ParseCommaOrWhitespace(ptr);

						ptr.AdvanceWhiteSpace();
						if (ptr.Char == ')')
						{
							++ptr;
							this.Transforms.Add(SvgTransform.CreateScale(sx, sy.Value));
							return;
						}
					}
					else
					{
						ptr.Index = current;

						ptr.AdvanceWhiteSpace();
						if (ptr.Char == ')')
						{
							++ptr;
							this.Transforms.Add(SvgTransform.CreateScale(sx, sx));
							return;
						}
					}
				}
				throw new ArgumentException("transformData");
			}
		}

		private void ParseRotate(StringPtr ptr)
		{
			if (ptr.String.StartsWith("rotate"))
			{
				ptr += 6;

				ptr.AdvanceWhiteSpace();
				if (ptr.Char == '(')
				{
					++ptr;
					ptr.AdvanceWhiteSpace();

					var r = ParseNumber(ptr).Value;

					var current = ptr.Index;
					this.ParseCommaOrWhitespace(ptr);

					var cx = ParseNumber(ptr);
					if (cx.HasValue)
					{
						this.ParseCommaOrWhitespace(ptr);

						var cy = ParseNumber(ptr).Value;
						this.ParseCommaOrWhitespace(ptr);

						ptr.AdvanceWhiteSpace();
						if (ptr.Char == ')')
						{
							++ptr;
							this.Transforms.Add(SvgTransform.CreateRotate(r, cx.Value, cy));
							return;
						}
					}
					else
					{
						ptr.Index = current;

						ptr.AdvanceWhiteSpace();
						if (ptr.Char == ')')
						{
							++ptr;
							this.Transforms.Add(SvgTransform.CreateRotate(r));
							return;
						}
					}
				}
				throw new ArgumentException("transformData");
			}
		}

		private void ParseSkewX(StringPtr ptr)
		{
			if (ptr.String.StartsWith("skewX"))
			{
				ptr += 5;

				ptr.AdvanceWhiteSpace();
				if (ptr.Char == '(')
				{
					++ptr;
					ptr.AdvanceWhiteSpace();

					var angle = ParseNumber(ptr).Value;
					this.ParseCommaOrWhitespace(ptr);

					ptr.AdvanceWhiteSpace();
					if (ptr.Char == ')')
					{
						++ptr;
						this.Transforms.Add(SvgTransform.CreateSkewX(angle));
						return;
					}
				}
				throw new ArgumentException("transformData");
			}
		}

		private void ParseSkewY(StringPtr ptr)
		{
			if (ptr.String.StartsWith("skewY"))
			{
				ptr += 5;

				ptr.AdvanceWhiteSpace();
				if (ptr.Char == '(')
				{
					++ptr;
					ptr.AdvanceWhiteSpace();

					var angle = ParseNumber(ptr).Value;
					this.ParseCommaOrWhitespace(ptr);

					ptr.AdvanceWhiteSpace();
					if (ptr.Char == ')')
					{
						++ptr;
						this.Transforms.Add(SvgTransform.CreateSkewY(angle));
						return;
					}
				}
				throw new ArgumentException("transformData");
			}
		}

		private float? ParseNumber(StringPtr ptr)
		{
			var begin = ptr.Index;
			ptr.AdvanceNumber();
			if (begin == ptr.Index) return null;

			var numberText = ptr.Target.Substring(begin, ptr.Index - begin);
			return float.Parse(numberText);
		}

		private bool ParseCommaOrWhitespace(StringPtr ptr)
		{
			if (ptr.IsWhitespace())
			{
				++ptr;
				ptr.AdvanceWhiteSpace();
				if (ptr.Char == ',') ++ptr;
				ptr.AdvanceWhiteSpace();
				return true;
			}
			else if (ptr.Char == ',')
			{
				++ptr;
				ptr.AdvanceWhiteSpace();
				return true;
			}
			return false;
		}
	}
}
