using System.Drawing;

namespace ImageSegmentation.Model.HMRF
{
    public static class Grayscale
    {
        public static Bitmap Convert(Bitmap image)
        {
            Bitmap img = new Bitmap(image.Width, image.Height);
            for (int i = 0; i < image.Width; i++)
            { 
                for (int j = 0; j < image.Height; j++)
                {
                    byte gray = image.GetPixel(i, j).R;
                    img.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                    //byte c = (byte)(0.3 * gray + 0.59 * gray + 0.11 * gray);
                    //img.SetPixel(i, j, Color.FromArgb(c, c, c));
                }
            }
            return img;
        }
    }
}
