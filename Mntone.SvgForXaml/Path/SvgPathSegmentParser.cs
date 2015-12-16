using Mntone.SvgForXaml.Internal;
using System;
using System.Collections.ObjectModel;

namespace Mntone.SvgForXaml.Path
{
	internal sealed class SvgPathSegmentParser
	{
		public static SvgPathSegmentCollection Parse(string pathData) => new SvgPathSegmentCollection(new SvgPathSegmentParser(pathData).Segments);

		public Collection<SvgPathSegment> Segments { get; }

		private SvgPathSegmentParser(string pathData)
		{
			this.Segments = new Collection<SvgPathSegment>();

			var ptr = new StringPtr(pathData);
			ptr.AdvanceWhiteSpace();
			this.ParseMoveToDrawToCommandGroups(ptr);
		}

		private void ParseMoveToDrawToCommandGroups(StringPtr ptr)
		{
			this.ParseMoveToDrawToCommandGroup(ptr);
			ptr.AdvanceWhiteSpace();
			if (!ptr.IsEnd) this.ParseMoveToDrawToCommandGroups(ptr);
		}

		private void ParseMoveToDrawToCommandGroup(StringPtr ptr)
		{
			this.ParseMoveTo(ptr);
			ptr.AdvanceWhiteSpace();
			if (!ptr.IsEnd) this.ParseDrawToCommands(ptr);
		}

		private bool ParseDrawToCommands(StringPtr ptr)
		{
			if (this.ParseDrawToCommand(ptr))
			{
				var current = ptr.Index;
				ptr.AdvanceWhiteSpace();
				if (!ptr.IsEnd)
				{
					if (!this.ParseDrawToCommands(ptr))
					{
						ptr.Index = current;
					}
				}
				return true;
			}
			return false;
		}

		private bool ParseDrawToCommand(StringPtr ptr)
		{
			switch (ptr.Char)
			{
				case 'Z':
				case 'z':
					this.ParseClosePath(ptr);
					break;

				case 'L':
				case 'l':
					this.ParseLineTo(ptr);
					break;

				case 'H':
				case 'h':
					this.ParseHorizontalLineTo(ptr);
					break;

				case 'V':
				case 'v':
					this.ParseVerticalLineTo(ptr);
					break;

				case 'C':
				case 'c':
					this.ParseCurveTo(ptr);
					break;

				case 'S':
				case 's':
					this.ParseSmoothCurveTo(ptr);
					break;

				case 'Q':
				case 'q':
					this.ParseQuadraticBezierCurveTo(ptr);
					break;

				case 'T':
				case 't':
					this.ParseSmoothQuadraticBezierCurveTo(ptr);
					break;

				case 'A':
				case 'a':
					this.ParseQllipticalArc(ptr);
					break;

				default:
					return false;
			}
			return true;
		}

		private void ParseMoveTo(StringPtr ptr)
		{
			if (char.ToLower(ptr.Char) == 'm')
			{
				var abs = ptr.Char == 'M';
				++ptr;
				ptr.AdvanceWhiteSpace();
				this.ParseMoveToArgumentSequence(ptr, abs);
				return;
			}
			throw new ArgumentException("pathData");
		}

		private void ParseMoveToArgumentSequence(StringPtr ptr, bool abs)
		{
			var coordinate = ParseCoordinatePair(ptr);
			if (coordinate == null) return;
			this.Segments.Add(SvgPathSegmentMoveToBase.Create(coordinate.Item1, coordinate.Item2, abs));

			if (this.ParseCommaOrWhitespace(ptr))
			{
				this.ParseLineToArgumentSequence(ptr, abs);
			}
		}

		private void ParseClosePath(StringPtr ptr)
		{
			++ptr;
			this.Segments.Add(new SvgPathSegmentClosePath());
		}

		private void ParseLineTo(StringPtr ptr)
		{
			var abs = ptr.Char == 'L';
			++ptr;
			ptr.AdvanceWhiteSpace();
			this.ParseLineToArgumentSequence(ptr, abs);
		}

		private void ParseLineToArgumentSequence(StringPtr ptr, bool abs)
		{
			var coordinate = ParseCoordinatePair(ptr);
			if (coordinate == null) return;
			this.Segments.Add(SvgPathSegmentLineToBase.Create(coordinate.Item1, coordinate.Item2, abs));

			if (this.ParseCommaOrWhitespace(ptr))
			{
				this.ParseLineToArgumentSequence(ptr, abs);
			}
		}

		private void ParseHorizontalLineTo(StringPtr ptr)
		{
			var abs = ptr.Char == 'H';
			++ptr;
			ptr.AdvanceWhiteSpace();
			this.ParseHorizontalLineToArgumentSequence(ptr, abs);
		}

		private void ParseHorizontalLineToArgumentSequence(StringPtr ptr, bool abs)
		{
			var coordinate = ParseCoordinate(ptr);
			if (!coordinate.HasValue) return;
			this.Segments.Add(SvgPathSegmentLineToHorizontalBase.Create(coordinate.Value, abs));

			if (this.ParseCommaOrWhitespace(ptr))
			{
				this.ParseHorizontalLineToArgumentSequence(ptr, abs);
			}
		}

		private void ParseVerticalLineTo(StringPtr ptr)
		{
			var abs = ptr.Char == 'V';
			++ptr;
			ptr.AdvanceWhiteSpace();
			this.ParseVerticalLineToArgumentSequence(ptr, abs);
		}

		private void ParseVerticalLineToArgumentSequence(StringPtr ptr, bool abs)
		{
			var coordinate = ParseCoordinate(ptr);
			if (!coordinate.HasValue) return;
			this.Segments.Add(SvgPathSegmentLineToVerticalBase.Create(coordinate.Value, abs));

			if (this.ParseCommaOrWhitespace(ptr))
			{
				this.ParseVerticalLineToArgumentSequence(ptr, abs);
			}
		}

		private void ParseCurveTo(StringPtr ptr)
		{
			var abs = ptr.Char == 'C';
			++ptr;
			ptr.AdvanceWhiteSpace();
			this.ParseCurveToArgumentSequence(ptr, abs);
		}

		private void ParseCurveToArgumentSequence(StringPtr ptr, bool abs)
		{
			var coordinate = ParseCoordinatePair(ptr);
			if (coordinate == null) return;
			this.ParseCommaOrWhitespace(ptr);

			var coordinate2 = ParseCoordinatePair(ptr);
			this.ParseCommaOrWhitespace(ptr);

			var coordinate3 = ParseCoordinatePair(ptr);
			this.Segments.Add(SvgPathSegmentCurveToCubicBase.Create(coordinate.Item1, coordinate.Item2, coordinate2.Item1, coordinate2.Item2, coordinate3.Item1, coordinate3.Item2, abs));

			if (this.ParseCommaOrWhitespace(ptr))
			{
				this.ParseCurveToArgumentSequence(ptr, abs);
			}
		}

		private void ParseSmoothCurveTo(StringPtr ptr)
		{
			var abs = ptr.Char == 'S';
			++ptr;
			ptr.AdvanceWhiteSpace();
			this.ParseSmoothCurveToArgumentSequence(ptr, abs);
		}

		private void ParseSmoothCurveToArgumentSequence(StringPtr ptr, bool abs)
		{
			var coordinate = ParseCoordinatePair(ptr);
			if (coordinate == null) return;
			this.ParseCommaOrWhitespace(ptr);

			var coordinate2 = ParseCoordinatePair(ptr);
			this.Segments.Add(SvgPathSegmentCurveToCubicSmoothBase.Create(coordinate.Item1, coordinate.Item2, coordinate2.Item1, coordinate2.Item2, abs));

			if (this.ParseCommaOrWhitespace(ptr))
			{
				this.ParseSmoothCurveToArgumentSequence(ptr, abs);
			}
		}

		private void ParseQuadraticBezierCurveTo(StringPtr ptr)
		{
			var abs = ptr.Char == 'Q';
			++ptr;
			ptr.AdvanceWhiteSpace();
			this.ParseQuadraticBezierCurveToArgumentSequence(ptr, abs);
		}

		private void ParseQuadraticBezierCurveToArgumentSequence(StringPtr ptr, bool abs)
		{
			var coordinate = ParseCoordinatePair(ptr);
			if (coordinate == null) return;
			this.ParseCommaOrWhitespace(ptr);

			var coordinate2 = ParseCoordinatePair(ptr);
			this.Segments.Add(SvgPathSegmentCurveToQuadraticBase.Create(coordinate.Item1, coordinate.Item2, coordinate2.Item1, coordinate2.Item2, abs));

			if (this.ParseCommaOrWhitespace(ptr))
			{
				this.ParseQuadraticBezierCurveToArgumentSequence(ptr, abs);
			}
		}

		private void ParseSmoothQuadraticBezierCurveTo(StringPtr ptr)
		{
			var abs = ptr.Char == 'T';
			++ptr;
			ptr.AdvanceWhiteSpace();
			this.ParseSmoothQuadraticBezierCurveToArgumentSequence(ptr, abs);
		}

		private void ParseSmoothQuadraticBezierCurveToArgumentSequence(StringPtr ptr, bool abs)
		{
			var coordinate = ParseCoordinatePair(ptr);
			this.Segments.Add(SvgPathSegmentCurveToQuadraticSmoothBase.Create(coordinate.Item1, coordinate.Item2, abs));

			if (this.ParseCommaOrWhitespace(ptr))
			{
				this.ParseSmoothQuadraticBezierCurveToArgumentSequence(ptr, abs);
			}
		}

		private void ParseQllipticalArc(StringPtr ptr)
		{
			var abs = ptr.Char == 'A';
			++ptr;
			ptr.AdvanceWhiteSpace();
			this.ParseQllipticalArcArgumentSequence(ptr, abs);
		}

		private void ParseQllipticalArcArgumentSequence(StringPtr ptr, bool abs)
		{
			var nonNegativeNumber = ParseNonNegativeNumber(ptr);
			if (nonNegativeNumber == null) return;
			this.ParseCommaOrWhitespace(ptr);

			var nonNegativeNumber2 = ParseNonNegativeNumber(ptr).Value;
			this.ParseCommaOrWhitespace(ptr);

			var number = ParseNumber(ptr).Value;
			if (this.ParseCommaOrWhitespace(ptr))
			{
				var flag = ParseFlag(ptr);
				this.ParseCommaOrWhitespace(ptr);

				var flag2 = ParseFlag(ptr);
				this.ParseCommaOrWhitespace(ptr);

				var coordinate = ParseCoordinatePair(ptr);

				this.Segments.Add(SvgPathSegmentArcBase.Create(coordinate.Item1, coordinate.Item2, nonNegativeNumber.Value, nonNegativeNumber2, number, flag, flag2, abs));

				if (this.ParseCommaOrWhitespace(ptr))
				{
					this.ParseQllipticalArcArgumentSequence(ptr, abs);
				}
				return;
			}
			throw new ArgumentException("pathData");
		}

		private Tuple<float, float> ParseCoordinatePair(StringPtr ptr)
		{
			var x = ParseCoordinate(ptr);
			if (!x.HasValue) return null;

			this.ParseCommaOrWhitespace(ptr);
			var y = ParseCoordinate(ptr);
			if (!y.HasValue) return null;

			return Tuple.Create(x.Value, y.Value);
		}

		private float? ParseCoordinate(StringPtr ptr) => ParseNumber(ptr);

		private float? ParseNonNegativeNumber(StringPtr ptr)
		{
			var begin = ptr.Index;
			ptr.AdvanceNonNegativeNumber();
			if (begin == ptr.Index) return null;

			var numberText = ptr.Target.Substring(begin, ptr.Index - begin);
			return float.Parse(numberText);
		}

		private float? ParseNumber(StringPtr ptr)
		{
			var begin = ptr.Index;
			ptr.AdvanceNumber();
			if (begin == ptr.Index) return null;

			var numberText = ptr.Target.Substring(begin, ptr.Index - begin);
			return float.Parse(numberText);
		}

		private bool ParseFlag(StringPtr ptr)
		{
			var flag = ptr.Char == '1';
			if (!flag && ptr.Char != '0') throw new ArgumentException("pathData");
			++ptr;
			return flag;
		}

		private bool ParseCommaOrWhitespace(StringPtr ptr)
		{
			if (ptr.IsEnd) return false;
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