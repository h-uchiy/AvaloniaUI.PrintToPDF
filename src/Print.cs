using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Skia;
using Avalonia.Skia.Helpers;
using SkiaSharp;

namespace AvaloniaUI.PrintToPDF
{
    public static class Print
    {
        // SkiaPDF treats PDF pages sized in point units. 1 pt == 1/72 inch == 127/360 mm.
        // https://api.skia.org/namespaceSkPDF.html#ac4e43ae2897f44e2fbd27fd1c2b45ff3
        public const float SkPdfDpi = 72.0f;

        public static readonly Vector InchPerMm = new Vector(0.0393701, 0.0393701);
        public static readonly Size PaperSizeA4 = new Size(210, 297) * InchPerMm;
        public const double PageMargin = 0.25; // inch

        public static Task ToFile(string fileName, params Visual[] visuals) => ToFile(fileName, visuals.AsEnumerable());

        public static async Task ToFile(string fileName, IEnumerable<Visual> visuals)
        {
            // Calc page contents area
            var pageContentArea = new Rect(PageMargin, PageMargin,
                PaperSizeA4.Width - PageMargin * 2,
                PaperSizeA4.Height - PageMargin * 2) * SkPdfDpi;

            using var doc = SKDocument.CreatePdf(fileName);
            foreach (var visual in visuals)
            {
                // Save & Restore RenderTransform
                using (Restorer.Create(visual.RenderTransformOrigin, x => visual.RenderTransformOrigin = x))
                using (Restorer.Create(visual.RenderTransform, x => visual.RenderTransform = x))
                {
                    // Fit visual to pageContentArea
                    var scale = pageContentArea.Width / visual.Bounds.Width;
                    visual.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);
                    visual.RenderTransform = new ScaleTransform(scale, scale);

                    // Create PDF page with A4 paper size
                    var paperSize = PaperSizeA4 * SkPdfDpi;
                    var page = doc.BeginPage((float)paperSize.Width, (float)paperSize.Height, pageContentArea.ToSKRect());
                    await DrawingContextHelper.RenderAsync(page, visual, visual.Bounds, SkiaPlatform.DefaultDpi);
                    doc.EndPage();
                }
            }

            doc.Close();
        }
    }
}