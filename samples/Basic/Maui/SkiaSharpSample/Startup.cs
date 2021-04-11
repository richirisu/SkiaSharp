using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Hosting;
using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui.Controls.Compatibility;

namespace SkiaSharpSample
{
	public class Startup : IStartup
	{
		public void Configure(IAppHostBuilder appBuilder)
		{
			appBuilder
				.UseFormsCompatibility()
				//.UseSkiaSharpCompatibilityRenderers() // for the GL views
				.UseSkiaSharpHandlers()
				.UseMauiApp<App>();
		}
	}
}
