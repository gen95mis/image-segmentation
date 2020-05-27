using System.Collections.Generic;
using System.Drawing;

namespace ImageSegmentation.Model.Helpers
{
    class ImageIO
    {
        private List<Pixel> pixels;
        private Bitmap image;

        public int Width
        {
            get => image.Width; 
        }

        public int Height
        {
            get => image.Height; 
        }

        public int Count
        {
            get => image.Width * image.Height;
        }

        public List<Pixel> Pixels
        {
            get => ImageMath.PixelDeepCopy(pixels);
            set => pixels = ImageMath.PixelDeepCopy(value);
        }

        public Pixel this [int index]
        {
            get => pixels[index];
            set => pixels[index] = value;
        }

        public Bitmap Image
        {
            get => image;
        }


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="fileName">Пусть к файлу</param>
        public ImageIO(string fileName) 
        {
            pixels = new List<Pixel>();

            image = (Bitmap)System.Drawing.Image.FromFile(fileName, true);
            for (int i = 0; i < image.Width; i++)
                for (int j = 0; j < image.Height; j++)
                    pixels.Add(new Pixel(i, j, image.GetPixel(i, j)));
        }


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="fileName">Изображение</param>
        public ImageIO(Bitmap image)
        {
            pixels = new List<Pixel>();

            this.image = image;
            for (int i = 0; i < image.Width; i++)
                for (int j = 0; j < image.Height; j++)
                    pixels.Add(new Pixel(i, j, image.GetPixel(i, j)));
        }


        /// <summary>
        /// Клонировать пиксель
        /// </summary>
        /// <param name="index">Номер пикселя</param>
        public Pixel ClonePixel(int index) =>
            new Pixel(pixels[index].X, pixels[index].Y, pixels[index].Color);


        /// <summary>
        /// Получить пиксель
        /// </summary>
        /// <param name="x">координата по X</param>
        /// <param name="y">координата по Y</param>
        /// <returns></returns>
        public Pixel GetPixel(int x, int y)
        {
            int index = x * Width + y;
            return pixels[index];
        }


        /// <summary>
        /// Сохранение изображений
        /// </summary>
        /// <param name="cent"> Центройды </param>
        public void Save(List<Pixel> cent, string name)
        {
            Bitmap img = new Bitmap(image.Width, image.Height);
            
            for (int i = 0; i < Count; i++)
                img.SetPixel(pixels[i].X, pixels[i].Y, cent[pixels[i].Claster].Color);

            img.Save(name);
        }


        /// <summary>
        /// Конвертация в Bitmap
        /// </summary>
        /// <param name="cent"> Центройды </param>
        public Bitmap ToBitmap(List<Pixel> cent)
        {
            Bitmap img = new Bitmap(image.Width, image.Height);

            for (int i = 0; i < Count; i++)
            {
                byte R = cent[pixels[i].Claster].Color.R;
                img.SetPixel(pixels[i].X, pixels[i].Y, Color.FromArgb(R, R, R));
            }

            return img;
        }
    }
}
