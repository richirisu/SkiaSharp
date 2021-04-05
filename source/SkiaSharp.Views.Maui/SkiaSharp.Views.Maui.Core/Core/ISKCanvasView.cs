using Microsoft.Maui;

namespace SkiaSharp.Views.Maui
{
	public interface ISKCanvasView : IView
	{
		SKSize CanvasSize { get; }

		bool IgnorePixelScaling { get; set; }

		bool EnableTouchEvents { get; set; }

		void InvalidateSurface();

		void OnCanvasSizeChanged(SKSizeI size);

		void OnPaintSurface(SKPaintSurfaceEventArgs e);

		void OnTouch(SKTouchEventArgs e);
	}
}
