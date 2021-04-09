using System;
using Microsoft.Maui.Handlers;
using SkiaSharp.Views.iOS;

namespace SkiaSharp.Views.Maui.Handlers
{
	public partial class SKCanvasViewHandler : ViewHandler<ISKCanvasView, SKCanvasView>
	{
		protected override SKCanvasView CreateNativeView() => throw new NotImplementedException();
	}
}
