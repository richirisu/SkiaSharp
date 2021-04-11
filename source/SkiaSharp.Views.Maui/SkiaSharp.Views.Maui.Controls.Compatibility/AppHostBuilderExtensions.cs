using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Hosting;

namespace SkiaSharp.Views.Maui.Controls.Compatibility
{
	public static class AppHostBuilderExtensions
	{
		public static IAppHostBuilder UseSkiaSharpCompatibilityRenderers(this IAppHostBuilder builder) =>
			builder.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddCompatibilityRenderer<SKCanvasView, SKCanvasViewRenderer>();
				handlers.AddCompatibilityRenderer<SKGLView, SKGLViewRenderer>();
			});
	}
}
