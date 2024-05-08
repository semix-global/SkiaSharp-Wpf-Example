using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SkiaWpf.Service
{
    public interface IImageService
    {
        WriteableBitmap CreateImage(int width, int height);
        void UpdateImage(WriteableBitmap writeableBitmap);
    }

    class ImageService : IImageService
    {
        private int Call = 0;
        private Stopwatch Stopwatch = new Stopwatch();

        public ImageService()
        {
        }

        public WriteableBitmap CreateImage(int width, int height)
        {
            return new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
        }

        public void UpdateImage(WriteableBitmap writeableBitmap)
        {
            int width = (int)writeableBitmap.Width,
                height = (int)writeableBitmap.Height;

            writeableBitmap.Lock();

            using (var surface = SKSurface.Create(
                       width: width,
                       height: height,
                       colorType: SKColorType.Bgra_8888,
                       alphaType: SKAlphaType.Premul,
                       pixels: writeableBitmap.BackBuffer,
                       rowBytes: width * 4))
            {
                SKCanvas canvas = surface.Canvas;

                int x = 30;
                var paint = new SKPaint() { Color = new SKColor(0, 0, 0), TextSize = 100 };
                canvas.Clear(new SKColor(130, 130, 130));
                canvas.DrawText("SkiaSharp on Wpf!", x, 200, paint);
                if (this.Call == 0)
                    this.Stopwatch.Start();
                double fps = this.Call / ((this.Stopwatch.Elapsed.TotalSeconds != 0) ? this.Stopwatch.Elapsed.TotalSeconds : 1);
                canvas.DrawText($"FPS: {fps:0}", x, 300, paint);
                canvas.DrawText($"Frames: {this.Call++}", x, 400, paint);

                // 芯片中间的间隔
                const int dX = 15;
                const int dY = 15;

                // 视野显示宽度高度
                const int viewShowWidthPx = 30;
                const int viewShowHeightPx = 30;

                for (var row = 0; row < 1000; row++)
                {
                    for (var col = 0; col < 100; col++)
                    {
                        // 视野中起始坐标
                        var xView = col * viewShowWidthPx;
                        var yView = row * viewShowHeightPx;
                        var xCell1 = xView + dX;
                        var yCell1 = yView + dY;
                        var xCell2 = xView + viewShowWidthPx; // 芯粒之间间隔
                        var yCell2 = yView + viewShowHeightPx; // 芯粒之间间隔

                        canvas.DrawRect(new SKRect(xCell1, yCell1, xCell2, yCell2), new SKPaint { Color = SKColors.LightBlue });
                    }
                }
            }

            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));

            writeableBitmap.Unlock();
        }
    }
}