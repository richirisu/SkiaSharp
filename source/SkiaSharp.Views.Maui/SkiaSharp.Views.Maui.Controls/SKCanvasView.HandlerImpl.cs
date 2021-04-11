#nullable enable

namespace SkiaSharp.Views.Maui.Controls
{
	partial class SKCanvasView : ISKCanvasView
	{
		private SKSize lastCanvasSize;

		void ISKCanvasView.OnCanvasSizeChanged(SKSizeI size)
		{
			lastCanvasSize = size;
		}

		void ISKCanvasView.OnPaintSurface(SKPaintSurfaceEventArgs e)
		{
			OnPaintSurface(e);
		}

		void ISKCanvasView.OnTouch(SKTouchEventArgs e)
		{
			OnTouch(e);
		}
	}
}
