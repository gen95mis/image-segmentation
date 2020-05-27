using System;
using System.Collections.Generic;
using System.Drawing;


namespace ImageSegmentation.Model.HMRF
{
    public class Canny
    {
        private Bitmap image;
        private int width;
        private int height;

        private double[,] gradient;
        private double[,] theta;

        private double[,] nonMaxSuppression;

        private int weak = 50;
        private int strong = 255;
        private double[,] threshold;
        private double[,] hysteresis;

        /// <summary>
        /// Маска состояит из 1 на границах и 0 в остальных облатях
        /// </summary>
        public double[,] Mask
        {
            get
            {
                double[,] mask = new double[width, height];

                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        mask[i, j] = hysteresis[i, j] / 255;

                return mask;
            }
        }


        public Canny(Bitmap origenal, int size, double sigma)
        {
            Bitmap grayscale = Grayscale.Convert(origenal); 
            grayscale.Save("grayscale.png");

            image = Blur.Gaussian(grayscale, size , sigma);
            image.Save("blur.png");

            width = image.Width;
            height = image.Height;
        }
        

        /// <summary>
        /// 
        /// </summary>
        public Bitmap EdgeDetection()
        {
            Bitmap edge = new Bitmap(width, height);

            SobleFilters();
            NonMaxSuppression();
            Threshold();
            Hysteresis();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    byte color = (byte)hysteresis[i, j];
                    edge.SetPixel(i, j, Color.FromArgb(color, color, color));
                }
            }


            SaveSoble();
            SaveNonMaxSuppression();
            SaveThreshold();
            SaveEdge();

            return edge;
        }

        #region Soble

        /// <summary>
        /// Выделение контуров алгоритмом Собеля
        /// </summary>
        private void SobleFilters()
        {
            int[,] kernalX = new int[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] kernalY = new int[3, 3] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };

            gradient = new double[width, height];
            theta = new double[width, height];

            double Ix = 0.0;
            double Iy = 0.0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Ix = SobleKernalOverlay(kernalX, i , j);
                    Iy = SobleKernalOverlay(kernalY, i, j);

                    gradient[i, j] = Math.Sqrt(Math.Pow(Ix, 2) + Math.Pow(Iy, 2));
                    theta[i, j] = Math.Atan2(Iy, Ix);
                }
            }

            // Нормализация градиента
            double max = Max(gradient);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    gradient[i, j] = gradient[i, j] / max  * 255;
        }


        /// <summary>
        /// Наложение ядра
        /// </summary>
        private double SobleKernalOverlay(int[,] kernal, int x, int y)
        {
            double sum = 0.0;

            for (int i = 0, k = x - 1; i < 3; i++, k++)
            {
                for (int j = 0, l = y - 1; j < 3; j++, l++)
                {
                    if (k < 0 || l < 0 || l > height - 1 || k > width - 1) sum += 0;
                    else sum += kernal[i, j] * image.GetPixel(k, l).R;
                }
            }

            return sum;
        }

        #endregion


        /// <summary>
        /// Подавление не-максимумов
        /// </summary>
        private void NonMaxSuppression()
        {
            nonMaxSuppression = new double[width, height];
            double[,] angle = new double[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    angle[i, j] = theta[i, j] * 180 / Math.PI;
                    if (angle[i, j] < 0) angle[i, j] += 180;
                }
            }

            double left, right;

            for(int i = 1; i < width - 1; i++)
            {
                for(int j = 1; j < height - 1; j++)
                {
                    left = 255;
                    right = 255;

                    // angle 0
                    if ((0 < angle[i, j] && angle[i, j] < 22.5) ||
                        (157.5 <= angle[i, j] && angle[i, j] <= 180))
                    {
                        left = gradient[i, j - 1];
                        right = gradient[i, j + 1];
                    }
                    // angle 45
                    if(22.5 < angle[i, j] && angle[i, j] < 67.5)
                    {
                        left = gradient[i - 1, j + 1];
                        right = gradient[i + 1, j - 1];
                    }
                    // angle 90
                    if(67.5 < angle[i,j] && angle[i, j] < 112.5)
                    {
                        left = gradient[i - 1, j];
                        right = gradient[i + 1, j];
                    }
                    // angle 135
                    if(112.5 < angle[i, j] && angle[i, j] < 157.5)
                    {
                        left = gradient[i - 1, j - 1];
                        right = gradient[i + 1, j + 1];
                    }


                    if (gradient[i, j] >= left && gradient[i, j] >= right)
                        nonMaxSuppression[i, j] = gradient[i, j];
                    else nonMaxSuppression[i, j] = 0;
                }
            }
        }


        /// <summary>
        /// Двойной порог
        /// </summary>
        private void Threshold()
        {
            threshold = new double[width, height];
            double lowTreshold = 0.5;
            double highTrshold = 0.5;

            highTrshold = Max(nonMaxSuppression) * highTrshold;
            lowTreshold = highTrshold * lowTreshold;

            for (int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    if (nonMaxSuppression[i,j] >= highTrshold) threshold[i, j] = strong;
                    else if (nonMaxSuppression[i, j] <= highTrshold &&
                            nonMaxSuppression[i, j] > lowTreshold)
                        threshold[i, j] = weak;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void Hysteresis()
        {
            hysteresis = threshold.Clone() as double[,];


            for (int i = 1; i < width - 1 ; i++)
            {
                for(int j = 1; j < height - 1; j++)
                {
                    if (hysteresis[i, j] == weak)
                    {
                        if ((hysteresis[i + 1, j - 1] == strong) || (hysteresis[i + 1, j] == strong)
                            || (hysteresis[i + 1, j + 1] == strong) || (hysteresis[i, j - 1] == strong)
                            || (hysteresis[i, j + 1] == strong) || (hysteresis[i - 1, j - 1] == strong)
                            || (hysteresis[i - 1, j] == strong) || (hysteresis[i - 1, j + 1] == strong))
                            hysteresis[i, j] = strong;
                        else hysteresis[i, j] = 0;
                    }
                    else hysteresis[i, j] = hysteresis[i, j];
                }
            }
        }


        /// <summary>
        /// Максимальный элемент матрицы
        /// </summary>
        private double Max(double[,] matrix)
        {
            double max = matrix[0, 0];

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (max < matrix[i, j]) max = matrix[i, j];

            return max;
        }



        /****************************************************
         *  Функции для тестирования этапов алгоритма Canny *
         ***************************************************/


        private void SaveSoble()
        {
            Bitmap img = new Bitmap(width, height);

            byte color = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    color = (byte)gradient[i, j];
                    img.SetPixel(i, j, Color.FromArgb(color, color, color));
                }
            }

            img.Save("sobel.png");
        }


        private void SaveNonMaxSuppression()
        {
            Bitmap img = new Bitmap(width, height);
            byte color = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    color = (byte)nonMaxSuppression[i, j];
                    img.SetPixel(i, j, Color.FromArgb(color, color, color));
                }
            }

            img.Save("non_max_suppression.png");
        }


        private void SaveThreshold()
        {
            Bitmap img = new Bitmap(width, height);

            byte color = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    color = (byte)threshold[i, j];
                    img.SetPixel(i, j, Color.FromArgb(color, color, color));
                }
            }

            img.Save("threshold.png");
        }


        private void SaveEdge()
        {
            Bitmap img = new Bitmap(width, height);

            byte color = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    color = (byte)hysteresis[i, j];
                    img.SetPixel(i, j, Color.FromArgb(color, color, color));
                }
            }

            img.Save("hysteresis.png");
        }
    }
}
