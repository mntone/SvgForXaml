using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Mntone.SvgForXaml.Primitives
{
	public class SvgColor : ICssValue
	{
		private static readonly Dictionary<string, RgbColor> _colors;
		static SvgColor()
		{
			_colors = new Dictionary<string, RgbColor>()
			{
				["aliceblue"] = new RgbColor(240, 248, 255),
				["antiquewhite"] = new RgbColor(250, 235, 215),
				["aqua"] = new RgbColor(0, 255, 255),
				["aquamarine"] = new RgbColor(127, 255, 212),
				["azure"] = new RgbColor(240, 255, 255),
				["beige"] = new RgbColor(245, 245, 220),
				["bisque"] = new RgbColor(255, 228, 196),
				["black"] = new RgbColor(0, 0, 0),
				["blanchedalmond"] = new RgbColor(255, 235, 205),
				["blue"] = new RgbColor(0, 0, 255),
				["blueviolet"] = new RgbColor(138, 43, 226),
				["brown"] = new RgbColor(165, 42, 42),
				["burlywood"] = new RgbColor(222, 184, 135),
				["cadetblue"] = new RgbColor(95, 158, 160),
				["chartreuse"] = new RgbColor(127, 255, 0),
				["chocolate"] = new RgbColor(210, 105, 30),
				["coral"] = new RgbColor(255, 127, 80),
				["cornflowerblue"] = new RgbColor(100, 149, 237),
				["cornsilk"] = new RgbColor(255, 248, 220),
				["crimson"] = new RgbColor(220, 20, 60),
				["cyan"] = new RgbColor(0, 255, 255),
				["darkblue"] = new RgbColor(0, 0, 139),
				["darkcyan"] = new RgbColor(0, 139, 139),
				["darkgoldenrod"] = new RgbColor(184, 134, 11),
				["darkgray"] = new RgbColor(169, 169, 169),
				["darkgreen"] = new RgbColor(0, 100, 0),
				["darkgrey"] = new RgbColor(169, 169, 169),
				["darkkhaki"] = new RgbColor(189, 183, 107),
				["darkmagenta"] = new RgbColor(139, 0, 139),
				["darkolivegreen"] = new RgbColor(85, 107, 47),
				["darkorange"] = new RgbColor(255, 140, 0),
				["darkorchild"] = new RgbColor(255, 140, 0),
				["darkorchid"] = new RgbColor(153, 50, 204),
				["darkred"] = new RgbColor(139, 0, 0),
				["darksalmon"] = new RgbColor(233, 150, 122),
				["darkseagreen"] = new RgbColor(143, 188, 143),
				["darkslateblue"] = new RgbColor(72, 61, 139),
				["darkslategray"] = new RgbColor(47, 79, 79),
				["darkslategrey"] = new RgbColor(47, 79, 79),
				["darkturquoise"] = new RgbColor(0, 206, 209),
				["darkviolet"] = new RgbColor(148, 0, 211),
				["deeppink"] = new RgbColor(255, 20, 147),
				["deepskyblue"] = new RgbColor(0, 191, 255),
				["dimgray"] = new RgbColor(105, 105, 105),
				["dimgrey"] = new RgbColor(105, 105, 105),
				["dodgerblue"] = new RgbColor(30, 144, 255),
				["firebrick"] = new RgbColor(178, 34, 34),
				["floralwhite"] = new RgbColor(255, 250, 240),
				["forestgreen"] = new RgbColor(34, 139, 34),
				["fuchsia"] = new RgbColor(255, 0, 255),
				["gainsboro"] = new RgbColor(220, 220, 220),
				["ghostwhite"] = new RgbColor(248, 248, 255),
				["gold"] = new RgbColor(255, 215, 0),
				["goldenrod"] = new RgbColor(218, 165, 32),
				["gray"] = new RgbColor(128, 128, 128),
				["grey"] = new RgbColor(128, 128, 128),
				["green"] = new RgbColor(0, 128, 0),
				["greenyellow"] = new RgbColor(173, 255, 47),
				["honeydew"] = new RgbColor(240, 255, 240),
				["hotpink"] = new RgbColor(255, 105, 180),
				["indianred"] = new RgbColor(205, 92, 92),
				["indigo"] = new RgbColor(75, 0, 130),
				["ivory"] = new RgbColor(255, 255, 240),
				["khaki"] = new RgbColor(240, 230, 140),
				["lavender"] = new RgbColor(230, 230, 250),
				["lavenderblush"] = new RgbColor(255, 240, 245),
				["lawngreen"] = new RgbColor(124, 252, 0),
				["lemonchiffon"] = new RgbColor(255, 250, 205),
				["lightblue"] = new RgbColor(173, 216, 230),
				["lightcoral"] = new RgbColor(240, 128, 128),
				["lightcyan"] = new RgbColor(224, 255, 255),
				["lightgoldenrodyellow"] = new RgbColor(250, 250, 210),
				["lightgray"] = new RgbColor(211, 211, 211),
				["lightgreen"] = new RgbColor(144, 238, 144),
				["lightgrey"] = new RgbColor(211, 211, 211),
				["lightpink"] = new RgbColor(255, 182, 193),
				["lightsalmon"] = new RgbColor(255, 160, 122),
				["lightseagreen"] = new RgbColor(32, 178, 170),
				["lightskyblue"] = new RgbColor(135, 206, 250),
				["lightslategray"] = new RgbColor(119, 136, 153),
				["lightslategrey"] = new RgbColor(119, 136, 153),
				["lightsteelblue"] = new RgbColor(176, 196, 222),
				["lightyellow"] = new RgbColor(255, 255, 224),
				["lime"] = new RgbColor(0, 255, 0),
				["limegreen"] = new RgbColor(50, 205, 50),
				["linen"] = new RgbColor(250, 240, 230),
				["magenta"] = new RgbColor(255, 0, 255),
				["maroon"] = new RgbColor(128, 0, 0),
				["mediumaquamarine"] = new RgbColor(102, 205, 170),
				["mediumblue"] = new RgbColor(0, 0, 205),
				["mediumorchid"] = new RgbColor(186, 85, 211),
				["mediumpurple"] = new RgbColor(147, 112, 219),
				["mediumseagreen"] = new RgbColor(60, 179, 113),
				["mediumslateblue"] = new RgbColor(13, 104, 238),
				["mediumspringgreen"] = new RgbColor(0, 250, 154),
				["mediumturquoise"] = new RgbColor(72, 209, 204),
				["mediumvioletred"] = new RgbColor(199, 21, 133),
				["midnightblue"] = new RgbColor(25, 25, 112),
				["mintcream"] = new RgbColor(245, 255, 250),
				["mistyrose"] = new RgbColor(255, 228, 225),
				["moccasin"] = new RgbColor(255, 228, 181),
				["navajowhite"] = new RgbColor(255, 222, 173),
				["navy"] = new RgbColor(0, 0, 128),
				["oldlace"] = new RgbColor(253, 245, 230),
				["olive"] = new RgbColor(128, 128, 0),
				["olivedrab"] = new RgbColor(107, 142, 35),
				["orange"] = new RgbColor(255, 165, 0),
				["orangered"] = new RgbColor(255, 69, 0),
				["orchid"] = new RgbColor(128, 112, 214),
				["palegoldenrod"] = new RgbColor(238, 232, 170),
				["palegreen"] = new RgbColor(152, 251, 152),
				["paleturquoise"] = new RgbColor(175, 238, 238),
				["palevioletred"] = new RgbColor(219, 112, 147),
				["papayawhip"] = new RgbColor(255, 239, 213),
				["peachpuff"] = new RgbColor(255, 218, 185),
				["peru"] = new RgbColor(205, 133, 63),
				["pink"] = new RgbColor(255, 192, 203),
				["plum"] = new RgbColor(221, 160, 221),
				["powderblue"] = new RgbColor(176, 224, 230),
				["purple"] = new RgbColor(128, 0, 128),
				["rebeccapurple"] = new RgbColor(102, 51, 153),
				["red"] = new RgbColor(255, 0, 0),
				["rosybrown"] = new RgbColor(188, 143, 143),
				["royalblue"] = new RgbColor(65, 105, 225),
				["saddlebrown"] = new RgbColor(139, 69, 19),
				["salmon"] = new RgbColor(250, 128, 114),
				["sandybrown"] = new RgbColor(244, 164, 96),
				["seagreen"] = new RgbColor(46, 139, 87),
				["seashell"] = new RgbColor(255, 245, 238),
				["sienna"] = new RgbColor(160, 82, 45),
				["silver"] = new RgbColor(192, 192, 192),
				["skyblue"] = new RgbColor(135, 206, 235),
				["slateblue"] = new RgbColor(106, 90, 205),
				["slategray"] = new RgbColor(112, 128, 144),
				["slategrey"] = new RgbColor(112, 128, 144),
				["snow"] = new RgbColor(255, 250, 250),
				["springgreen"] = new RgbColor(0, 255, 127),
				["steelblue"] = new RgbColor(70, 130, 180),
				["tan"] = new RgbColor(210, 180, 140),
				["teal"] = new RgbColor(0, 128, 128),
				["thistle"] = new RgbColor(216, 191, 216),
				["tomato"] = new RgbColor(255, 99, 71),
				["turquoise"] = new RgbColor(64, 224, 208),
				["violet"] = new RgbColor(238, 130, 238),
				["wheat"] = new RgbColor(245, 222, 179),
				["white"] = new RgbColor(255, 255, 255),
				["whitesmoke"] = new RgbColor(245, 245, 245),
				["yellow"] = new RgbColor(255, 255, 0),
				["yellowgreen"] = new RgbColor(154, 205, 50),
			};
		}

		internal protected SvgColor() { }
		internal SvgColor(string color)
		{
			this.RgbColor = Parse(color);
		}

		internal SvgColor Clone() => (SvgColor)this.MemberwiseClone();

		public RgbColor RgbColor { get; protected set; }

		protected static RgbColor Parse(string color)
		{
			if (color[0] == '#') return ParseColorCode(color.Substring(1));
			if (color.StartsWith("rgb(")) return ParseColorFunc(color.Substring(4));
			if (_colors.ContainsKey(color)) return _colors[color];
			throw new ArgumentException(nameof(color));
		}

		private static RgbColor ParseColorCode(string color)
		{
			if (color.Length == 3)
			{
				var f = new Func<char, byte>((char bc) =>
				{
					var c = char.ToUpper(bc);
					var b = c >= 'A' ? c - '8' : c - '0';
					return (byte)((b << 4) | b);
				});
				return new RgbColor(f(color[0]), f(color[1]), f(color[2]));
			}
			if (color.Length == 6)
			{
				var r = byte.Parse(color.Substring(0, 2), NumberStyles.HexNumber);
				var g = byte.Parse(color.Substring(2, 2), NumberStyles.HexNumber);
				var b = byte.Parse(color.Substring(4, 2), NumberStyles.HexNumber);
				return new RgbColor(r, g, b);
			}
			throw new ArgumentException(nameof(color));
		}

		private static RgbColor ParseColorFunc(string color)
		{
			var ptr = new StringPtr(color);
			ptr.AdvanceWhiteSpace();

			var s1 = ptr.Index;
			ptr.AdvanceInteger();
			if (ptr.Index == s1) throw new ArgumentException(nameof(color));
			var r = byte.Parse(color.Substring(s1, ptr.Index - s1));

			var percentage = false;
			if (ptr.Char == '%')
			{
				percentage = true;
				++ptr;
			}

			ptr.AdvanceWhiteSpace();
			if (ptr.Char != ',') throw new ArgumentException(nameof(color));
			++ptr;
			ptr.AdvanceWhiteSpace();

			var s2 = ptr.Index;
			ptr.AdvanceInteger();
			if (ptr.Index == s2) throw new ArgumentException(nameof(color));
			var g = byte.Parse(color.Substring(s2, ptr.Index - s2));

			if (percentage)
			{
				if (ptr.Char != '%') throw new ArgumentException(nameof(color));
				++ptr;
			}

			ptr.AdvanceWhiteSpace();
			if (ptr.Char != ',') throw new ArgumentException(nameof(color));
			++ptr;
			ptr.AdvanceWhiteSpace();

			var s3 = ptr.Index;
			ptr.AdvanceInteger();
			if (ptr.Index == s3) throw new ArgumentException(nameof(color));
			var b = byte.Parse(color.Substring(s3, ptr.Index - s3));

			if (percentage)
			{
				if (ptr.Char != '%') throw new ArgumentException(nameof(color));
				++ptr;
			}

			ptr.AdvanceWhiteSpace();

			if (ptr.Char != ')') throw new ArgumentException(nameof(color));

			return percentage
				? new RgbColor((byte)(255.0F * r / 100.0F), (byte)(255.0F * g / 100.0F), (byte)(255.0F * b / 100.0F))
				: new RgbColor(r, g, b);
		}

		private static RgbColor ParseOther(string color)
		{
			throw new NotImplementedException();
		}

		internal Windows.UI.Color ToPlatformColor(byte alpha)
		{
			return Windows.UI.Color.FromArgb(alpha, this.RgbColor.Red, this.RgbColor.Green, this.RgbColor.Blue);
		}
	}
}