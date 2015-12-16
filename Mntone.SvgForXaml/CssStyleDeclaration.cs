using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mntone.SvgForXaml
{
	public sealed class CssStyleDeclaration
	{
		private readonly ISvgStylable _parent;
		private readonly List<string> _items;
		private readonly Dictionary<string, Tuple<string, ICssValue>> _cache;

		private CssStyleDeclaration(ISvgStylable parent, List<string> items, Dictionary<string, Tuple<string, ICssValue>> cache)
		{
			this._parent = parent;
			this._items = items;
			this._cache = cache;
		}

		internal CssStyleDeclaration(ISvgStylable parent, string css)
		{
			this._parent = parent;
			this._items = new List<string>();
			this._cache = new Dictionary<string, Tuple<string, ICssValue>>();
			this.ParseText(css);
		}

		internal CssStyleDeclaration DeepCopy(ISvgStylable parent)
		{
			var item = new List<string>(this._items);
			var cache = new Dictionary<string, Tuple<string, ICssValue>>();
			foreach (var c in this._cache)
			{
				ICssValue value;
				var target = c.Value.Item2;
				if (target.GetType() == typeof(SvgPaint) || target.GetType() == typeof(SvgColor))
				{
					value = ((SvgColor)c.Value.Item2).Clone();
				}
				else if (target.GetType() == typeof(SvgNumber) || target.GetType() == typeof(SvgLength))
				{
					value = c.Value.Item2;
				}
				else if (target.GetType() == typeof(SvgIri))
				{
					value = new SvgIri(((SvgIri)c.Value.Item2).Uri);
				}
				else
				{
					throw new InvalidOperationException();
				}
				cache.Add(c.Key, Tuple.Create(c.Value.Item1, value));
			}
			return new CssStyleDeclaration(parent, item, cache);
		}

		public string GetPropertyValue(string propertyName) => this.GetPropertyValuePrivate(propertyName)?.Item1;
		public ICssValue GetPropertyCssValue(string propertyName) => this.GetPropertyValuePrivate(propertyName)?.Item2;

		internal void SetProperty(string propertyName, string value, string priority, bool presentation)
		{
			this.ParseValue(propertyName, value, priority, presentation);
		}

		private Tuple<string, ICssValue> GetPropertyValuePrivate(string propertyName)
		{
			if (!this._cache.ContainsKey(propertyName))
			{
				var target = ((INode)this._parent)?.ParentNode as ISvgStylable;
				if (target != null)
				{
					return target.Style.GetPropertyValuePrivate(propertyName);
				}
				return null;
			}
			return this._cache[propertyName];
		}

		public string this[ulong index] => this._items[(int)index];

		public SvgPaint Fill => this.GetPropertyCssValue("fill") as SvgPaint;
		public SvgNumber? FillOpacity => this.GetPropertyCssValue("fill-opacity") as SvgNumber?;
		public SvgFillRule? FillRule => this.GetPropertyCssValue("fill-rule") as SvgFillRule?;
		public SvgPaint Stroke => this.GetPropertyCssValue("stroke") as SvgPaint;
		public SvgLength? StrokeWidth => this.GetPropertyCssValue("stroke-width") as SvgLength?;
		public SvgNumber? StrokeOpacity => this.GetPropertyCssValue("stroke-opacity") as SvgNumber?;
		public SvgColor StopColor => this.GetPropertyCssValue("stop-color") as SvgColor;
		public SvgNumber? StopOpacity => this.GetPropertyCssValue("stop-opacity") as SvgNumber?;
		public SvgIri ClipPath => this.GetPropertyCssValue("clip-path") as SvgIri;

		private void ParseText(string css)
		{
			if (string.IsNullOrWhiteSpace(css)) return;

			var props = css.Split(new[] { ';' }).Where(p => !string.IsNullOrEmpty(p)).Select(prop =>
			{
				var kv = prop.Split(new[] { ':' }).ToArray();
				if (kv.Length != 2) throw new Exception();
				return new KeyValuePair<string, string>(kv[0].Trim(), kv[1].Trim());
			});
			foreach (var prop in props)
			{
				var result = this.ParseValue(prop.Key, prop.Value, string.Empty, false);
				this._items.Add(result.Item1);
			}
		}

		private Tuple<string, ICssValue> ParseValue(string name, string value, string priority, bool presentation)
		{
			var important = priority == "important";
			if (!presentation) name = name.ToLower();

			ICssValue parsedValue = null;
			switch (name)
			{
				case "fill":
				case "stroke":
					parsedValue = new SvgPaint(value);
					break;

				case "stroke-width":
					parsedValue = SvgLength.Parse(value, presentation);
					break;

				case "stop-color":
					parsedValue = new SvgColor(value);
					break;

				case "fill-opacity":
				case "stroke-opacity":
				case "stop-opacity":
					parsedValue = SvgNumber.Parse(value);
					break;

				case "clip-path":
					parsedValue = new SvgIri(value);
					break;

				case "fill-rule":
				case "clip-rule":
					parsedValue = new SvgFillRule(presentation ? value : value.ToLower());
					break;
			}

			if (!this._cache.ContainsKey(name))
			{
				var result = Tuple.Create(value, parsedValue);
				this._cache.Add(name, result);
				return result;
			}
			else if (important)
			{
				var result = Tuple.Create(value, parsedValue);
				this._cache[name] = result;
				return result;
			}

			return null;
		}
	}
}