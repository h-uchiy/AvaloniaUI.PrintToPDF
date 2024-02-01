using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Skia;
using Avalonia.Skia.Helpers;
using SkiaSharp;

namespace AvaloniaUI.PrintToPDF
{
    public static class Print
    {
        public static Task ToFile(string fileName, params Visual[] visuals) => ToFile(fileName, visuals.AsEnumerable());
        
        public static async Task ToFile(string fileName, IEnumerable<Visual> visuals)
        {
            using var doc = SKDocument.CreatePdf(fileName);
            foreach (var visual in visuals)
            {
                var bounds = visual.Bounds;
                var page = doc.BeginPage((float)bounds.Width, (float)bounds.Height);
                await DrawingContextHelper.RenderAsync(page, visual, bounds, SkiaPlatform.DefaultDpi);
                doc.EndPage();
            }

            doc.Close();
        }
    }
}