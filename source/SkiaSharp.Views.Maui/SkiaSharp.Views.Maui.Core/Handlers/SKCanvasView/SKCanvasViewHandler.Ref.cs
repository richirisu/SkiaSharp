using System;
using Microsoft.Maui.Handlers;

namespace SkiaSharp.Views.Maui.Handlers
{
	public partial class SKCanvasViewHandler : AbstractViewHandler<ISKCanvasView, object>
	{
		protected override object CreateNativeView() => throw new NotImplementedException();
	}
}
