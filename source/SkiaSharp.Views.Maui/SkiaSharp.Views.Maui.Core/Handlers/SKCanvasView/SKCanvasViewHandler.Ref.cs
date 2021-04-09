using System;
using Microsoft.Maui.Handlers;

namespace SkiaSharp.Views.Maui.Handlers
{
	public partial class SKCanvasViewHandler : ViewHandler<ISKCanvasView, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();
	}
}
