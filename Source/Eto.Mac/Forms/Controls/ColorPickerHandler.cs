using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using System.Text.RegularExpressions;
using System.Linq;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms.Controls
{
	public class ColorPickerHandler : MacControl<NSColorWell, ColorPicker, ColorPicker.ICallback>, ColorPicker.IHandler
	{
		public class EtoColorWell : NSColorWell, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public override void Activate(bool exclusive)
			{
				base.Activate(exclusive);
				var handler = GetHandler(this) as ColorPickerHandler;
				if (handler != null)
				{
					handler.TriggerMouseCallback();
				}
			}

			public override void Deactivate()
			{
				base.Deactivate();
				var handler = GetHandler(this) as ColorPickerHandler;
				if (handler != null)
				{
					handler.TriggerMouseCallback();
				}
			}
		}

		public ColorPickerHandler()
		{
			Control = new EtoColorWell { WeakHandler = new WeakReference(this) };
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			return new SizeF(44, 23);
		}

		static NSString keyColor = new NSString("color");

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case ColorPicker.ColorChangedEvent:
					AddControlObserver(keyColor, args =>
					{
						var handler = (ColorPickerHandler)args.Handler;
						handler.Callback.OnColorChanged(handler.Widget, EventArgs.Empty);
					});
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public Color Color
		{
			get { return Control.Color.ToEto(); }
			set { Control.Color = value.ToNSUI(); }
		}
	}
}
