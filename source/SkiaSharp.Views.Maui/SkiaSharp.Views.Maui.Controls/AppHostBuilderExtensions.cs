using Microsoft.Maui.Hosting;
using SkiaSharp.Views.Maui.Handlers;

namespace SkiaSharp.Views.Maui.Controls
{
	public static class AppHostBuilderExtensions
	{
		public static IAppHostBuilder UseSkiaSharpHandlers(this IAppHostBuilder builder) =>
			builder.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler<ISKCanvasView, SKCanvasViewHandler>();
			});
	}
}
