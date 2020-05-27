using ImageSegmentation.Model.Helpers;
using System.Collections.Generic;

namespace ImageSegmentation.Model.KMeans
{
    static class Clusters
    {
        /// <summary>
        /// Равномерное распределение областей на всём изображении 
        /// </summary>
        /// <param name="image">Исходное изображение</param>
        /// <param name="k">Количество областей</param>
        /// <returns>List centroids</returns>
        public static List<Pixel> Generation(ImageIO image, int k)
        {
            List<Pixel> pixels = new List<Pixel>();
            int step = image.Count / k;

            for(int index = 0; index < k; index++)
            {
                pixels.Add(image.ClonePixel(index * step));
                pixels[index].Claster = index;
            }

            return pixels;
        }
    }
}
