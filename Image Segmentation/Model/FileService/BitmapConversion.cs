using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageSegmentation.Model.FileService
{
    public static class BitmapConversion
    {
        /// <summary>
        /// Конвертация Bitmap в BitmapSource
        /// </summary>
        /// <param name="source">Bitmap изображение</param>
        public static BitmapSource BitmapToBitmapSource(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          System.Windows.Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }


        /// <summary>
        /// Конвертация BitmapSource в Bitmap
        /// </summary>
        /// <param name="source">BitmapSource изображение</param>
        public static Bitmap BitmapSourceToBitmap(BitmapSource source)
        {
            Bitmap bitmap = new Bitmap(source.PixelWidth, source.PixelHeight, PixelFormat.Format32bppPArgb);
            BitmapData data = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bitmap.UnlockBits(data);
            return bitmap;
        }
    }
}
