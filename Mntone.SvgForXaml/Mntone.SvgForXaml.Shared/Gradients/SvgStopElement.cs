﻿using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Gradients
{
	[System.Diagnostics.DebuggerDisplay("Stop: Offset = {this.Offset}")]
	public sealed class SvgStopElement : SvgElement, ISvgStylable
	{
		internal SvgStopElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(this, element);

			this.Offset = element.ParseNumberOrPercentage("offset");
		}

		protected override void DeepCopy(SvgElement element)
		{
			var casted = (SvgStopElement)element;
			casted._stylableHelper = this._stylableHelper.DeepCopy(casted);
		}

		public override string TagName => "stop";
		public float Offset { get; }

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion
	}
}