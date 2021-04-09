using Microsoft.Maui.Handlers;
using SkiaSharp.Views.Android;

namespace SkiaSharp.Views.Maui.Handlers
{
	public partial class SKCanvasViewHandler : ViewHandler<ISKCanvasView, SKCanvasView>
	{
		private SKSizeI lastSize;

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
			var newSize = e.Info.Size;
			if (lastSize != newSize)
			{
				lastSize = newSize;
				VirtualView?.OnCanvasSizeChanged(newSize);
			}

			VirtualView?.OnPaintSurface(new SKPaintSurfaceEventArgs(e.Surface, e.Info));
		}
	}
}
