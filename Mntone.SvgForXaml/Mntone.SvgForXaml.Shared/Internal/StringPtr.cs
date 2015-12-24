using System.Linq;

namespace Mntone.SvgForXaml.Internal
{
	internal sealed class StringPtr
	{
		public StringPtr(string target) : this(target, 0) { }
		public StringPtr(string target, int index)
		{
			this.Target = target;
			this.Index = index;
		}

		public string Target { get; }
		public int Index { get; set; }
		public char Char => this.Target[this.Index];
		public string String => this.Target.Substring(this.Index);

		public bool IsEnd => this.Index == this.Target.Length;

		private readonly char[] WHITESPACE = { ' ', '\r', '\n', '\t' };
		public void AdvanceWhiteSpace()
		{
			while (!this.IsEnd && WHITESPACE.Any(c => c == this.Char)) ++this.Index;
		}

		public bool IsWhitespace()
		{
			return WHITESPACE.Any(c => c == this.Char);
		}

		public void AdvanceInteger()
		{
			if (!this.IsEnd && (this.Char == '+' || this.Char == '-')) ++this.Index;
			while (!this.IsEnd && (this.Char >= '0' && this.Char <= '9')) ++this.Index;
		}

		public void AdvanceNumber()
		{
			if (!this.IsEnd && (this.Char == '+' || this.Char == '-')) ++this.Index;
			while (!this.IsEnd && (this.Char >= '0' && this.Char <= '9' || this.Char == '.')) ++this.Index;
			if (!this.IsEnd && (this.Char == 'e' || this.Char == 'E'))
			{
				++this.Index;
				if (!this.IsEnd && (this.Char == '+' || this.Char == '-')) ++this.Index;
				while (!this.IsEnd && (this.Char >= '0' && this.Char <= '9')) ++this.Index;
			}
		}

		public void AdvanceNonNegativeNumber()
		{
			while (this.Char >= '0' && this.Char <= '9' || this.Char == '.') ++this.Index;
			if (this.Char == 'e' || this.Char == 'E')
			{
				++this.Index;
				if (this.Char == '+' || this.Char == '-') ++this.Index;
				while (this.Char >= '0' && this.Char <= '9') ++this.Index;
			}
		}

		public static StringPtr operator +(StringPtr left, int right)
		{
			left.Index += right;
			return left;
		}
		public static StringPtr operator -(StringPtr left, int right)
		{
			left.Index -= right;
			return left;
		}
		public static StringPtr operator ++(StringPtr left)
		{
			left.Index += 1;
			return left;
		}
		public static StringPtr operator --(StringPtr left)
		{
			left.Index -= 1;
			return left;
		}
	}
}