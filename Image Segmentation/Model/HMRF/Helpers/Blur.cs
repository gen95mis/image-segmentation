/*
 * Пример горизонтальной Гауссовской маски 5 X 5
 * -2 -2 -2 -2 -2
 * -1 -1 -1 -1 -1
 *  0  0  0  0  0
 *  1  1  1  1  1
 *  2  2  2  2  2
 *  
 * Пример горизонтальной Гауссовской маски 4 X 4
 * -2 -2 -2 -2 -2
 * -1 -1 -1 -1 -1
 *  1  1  1  1  1
 *  2  2  2  2  2
 *  
 * Пример вертикальной Гауссовской маски 5 X 5 
 * -2 -1 0 1 2
 * -2 -1 0 1 2
 * -2 -1 0 1 2
 * -2 -1 0 1 2
 * -2 -1 0 1 2
 * 
 * Пример вертикальной Гауссовской маски 4 X 4 
 * -2 -1 1 2
 * -2 -1 1 2
 * -2 -1 1 2
 * -2 -1 1 2
 * -2 -1 1 2
 * 
 * Крайнее число вычеляется по формуле: (Размкр Ядра - 1) / 2
 */


using System;
using System.Drawing;

namespace ImageSegmentation.Model.HMRF
{
    public static class Blur
    {
        /// <summary>
        /// Размытие по Гауссу
        /// </summary>
        /// <param name="image">Изображение для размытия</param>
        /// <param name="size">Размер ядра</param>
        /// <param name="sigma">Среднеквадратическое отклонение распределения Гаусса</param>
        /// <returns>Размытое изображение</returns>
        public static Bitmap Gaussian(Bitmap image, int size, double sigma)
        {
            Bitmap img = new Bitmap(image.Width, image.Height);

            double[,] kernel = Kernel(size, sigma);
            //kernel = KernelNormalized(kernel, size);
            

            int color = 0; 
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    color = Overlay(image, i, j, kernel, size);
                    img.SetPixel(i, j, Color.FromArgb(color, color, color));
                }
            }

            return img;
        }


        /// <summary>
        /// Наложение Ядра на изображеие
        /// </summary>
        /// <param name="image">Исходное изображение</param>
        /// <param name="x">Координата центральной точки</param>
        /// <param name="y">Координата центральной точки</param>
        /// <param name="kernal">Ядро</param>
        /// <param name="size">Размер ядра</param>
        /// <returns>Цвет пикселя после обработки ядром</returns>
        private static int Overlay(Bitmap image, int x, int y, double[,] kernal, int size)
        {
            double color = 0;
            int bufK = 0, bufL = 0;
            // k и y - устанавливают стартовые координаты на изображении
            for (int i = 0, k = x - (int)(size / 2); i < size; i++, k++)
            {
                for (int j = 0, l = y - (int)(size / 2); j < size; j++, l++)
                {
                    bufK = k;
                    bufL = l;
                    if (k < 0) bufK = 0;
                    else if (k > image.Width - 1) bufK = image.Width - 1;
                    if (l < 0) bufL = 0;
                    else if (l > image.Height - 1) bufL = image.Height - 1;

                    color += kernal[i, j] * image.GetPixel(bufK, bufL).B;
                }
            }

            return (int)color;
        }


        /// <summary>
        /// Создание ядра
        /// </summary>
        /// <param name="size">Размер ядра</param>
        /// <param name="sigma">Среднеквадратическое отклонение распределения Гаусса</param>
        /// <returns>Массив размером NxN созданый по формуле Гаусса</returns>
        private static double[,] Kernel(int size, double sigma)
        {
            double[,] matrix = new double[size, size];


            double[,] horizonte = HorizonteMatrix(size);
            double[,] verticale = VerticaleMatrix(size);

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    matrix[i, j] = GaussianFunc(horizonte[i,j], verticale[i, j], sigma);

            return matrix;
        }

        
        /// <summary>
        /// Функция Гауса
        /// </summary>
        /// <param name="x">Значение за горизонтальной маски Гаусса</param>
        /// <param name="y">Значение за вертекальной маски Гаусса</param>
        /// <param name="sigma">Среднеквадратическое отклонение распределения Гаусса</param>
        //private static double GaussianFunc(double x, double y, double sigma)
        //    => Math.Exp((-1) * (Math.Pow(x, 2) + (Math.Pow(y, 2))) / (2 * Math.Pow(sigma, 2)));
        private static double GaussianFunc(double x, double y, double sigma)
        {
            double normalixe = 1 / (2.0 * Math.PI * Math.Pow(sigma, 2));
            return (Math.Exp((-1) * (Math.Pow(x, 2) + (Math.Pow(y, 2))) / (2 * Math.Pow(sigma, 2)))) * normalixe;
        }


        /// <summary>
        /// Маска Гаусса - горизонтальная
        /// </summary>
        /// <param name="size">Размер ядра</param>
        private static double[,] HorizonteMatrix(int size)
        {
            double[,] matrix = new double[size, size];

            double index = (size - 1) / 2.0;
            for (int i = 0; i < size / 2; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = -index;
                    matrix[size - i - 1, j] = index;
                }
                index--;
            }

            if(size % 2 != 0)
                for (int i = 0; i < size; i++)
                    matrix[size/2, i] = 0;

            return matrix;
        }


        /// <summary>
        /// Маска Гаусса вертикальная
        /// </summary>
        /// <param name="size">Размер ядра</param>
        private static double[,] VerticaleMatrix(int size)
        {
            double[,] matrix = new double[size, size];

            double index = 0;
            for (int i = 0; i < size; i++)
            {
                index = (size - 1) / 2.0;
                for (int j = 0; j < size / 2; j++)
                {
                    matrix[i, j] = -index;
                    matrix[i, size - j - 1] = index;
                    index--;
                }
            }

            return matrix;
        }
    }
}
