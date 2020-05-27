using System;
using System.Drawing;
using System.Collections.Generic;

namespace ImageSegmentation.Model.Helpers
{
    static class ImageMath
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static double EuclideanDistance(Color c1, Color c2)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(c1.R - c2.R), 2) +
                Math.Pow(Math.Abs(c1.G - c2.G), 2) + Math.Pow(Math.Abs(c1.B - c2.B), 2));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public static List<Pixel> PixelDeepCopy(List<Pixel> pixels)
        {
            List<Pixel> pxls = new List<Pixel>();
            foreach (Pixel p in pixels)
                pxls.Add(new Pixel(p.X, p.Y, p.Color, p.Claster));

            return pxls;
        }


    }
}
