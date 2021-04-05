using Microsoft.Maui.Handlers;
using SkiaSharp.Views.Android;

namespace SkiaSharp.Views.Maui.Handlers
{
	public partial class SKCanvasViewHandler : AbstractViewHandler<ISKCanvasView, SKCanvasView>
	{
		protected override SKCanvasView CreateNativeView()
		{
			var nativeView = new SKCanvasView(Context);

			return nativeView;
		}

		protected override void ConnectHandler(SKCanvasView nativeView)
		{
			nativeView.PaintSurface += OnPaintSurface;

			base.ConnectHandler(nativeView);
		}

		protected override void DisconnectHandler(SKCanvasView nativeView)
		{
			nativeView.PaintSurface -= OnPaintSurface;

			base.DisconnectHandler(nativeView);
		}

		private void OnPaintSurface(object? sender, Android.SKPaintSurfaceEventArgs e)
		{
			VirtualView?.OnCanvasSizeChanged(e.Info.Size);
			VirtualView?.OnPaintSurface(new SKPaintSurfaceEventArgs(e.Surface, e.Info));
		}
	}
}
