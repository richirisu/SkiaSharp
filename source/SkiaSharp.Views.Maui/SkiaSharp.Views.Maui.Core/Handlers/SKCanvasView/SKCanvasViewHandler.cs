using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace SkiaSharp.Views.Maui.Handlers
{
	public partial class SKCanvasViewHandler
	{
		public static PropertyMapper<ISKCanvasView, SKCanvasViewHandler> SKCanvasViewMapper =
			new PropertyMapper<ISKCanvasView, SKCanvasViewHandler>(ViewHandler.ViewMapper)
			{
			};

		public SKCanvasViewHandler()
			: base(SKCanvasViewMapper)
		{

		}

		public SKCanvasViewHandler(PropertyMapper? mapper = null)
			: base(mapper ?? SKCanvasViewMapper)
		{
		}
	}
}
