using Microsoft.Maui.Hosting;
using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui.Controls.Compatibility;

namespace Microsoft.Maui.Controls.Compatibility
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
