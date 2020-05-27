using System;
using System.Collections.Generic;
using System.Drawing;
using ImageSegmentation.Model.Helpers;
using ImageSegmentation.Model.KMeans;

namespace ImageSegmentation.Model.KMeans
{
    class KMeans
    {        
        private int k;
        private ImageIO image;
        private List<Pixel> pixels;
        private List<Pixel> clusters;
        private List<Pixel> clusterHist;

        private int[] count;

        public double[] STD
        {
            get
            {
                double[] std = new double[k];
                var average = AVG;
                for (int i = 0; i < image.Count; i++)
                    std[pixels[i].Claster] +=
                        Math.Pow(pixels[i].Color.R - average[pixels[i].Claster], 2);
                for (int i = 0; i < k; i++)
                    std[i] = Math.Sqrt(std[i] / count[i]);
                return std;
            }
        }

        public double[] AVG
        {
            get
            {
                double[] avg = new double[k];
                for (int i = 0; i < image.Count; i++)
                    avg[pixels[i].Claster] += pixels[i].Color.R;
                for (int i = 0; i < k; i++)
                    avg[i]  /= count[i];
                return avg;
            }
        }

        public double[,] Field
        {
            get
            {
                double[,] field = new double[image.Width, image.Height];

                for (int i = 0; i < image.Count; i++)
                    field[image[i].X, image[i].Y] = image[i].Claster;

                return field;
            }
        }

        public Bitmap Image
        {
            get => image.ToBitmap(clusters);
        }


        public List<Pixel> LastCluster
        {
            get => clusters;
        }

        /// <param name="image">Изобрежение</param>
        /// <param name="k">Количество кластеров</param>
        public KMeans(ref ImageIO image, int k)
        {
            this.pixels = image.Pixels;
            this.k = k;
            this.image = image;

            clusters = Clusters.Generation(image, k);
            clusterHist = new List<Pixel>();
        }


        /// <param name="bitmap">Изобрежение</param>
        /// <param name="k">Количество кластеров</param>
        public KMeans(Bitmap bitmap, int k)
        {
            ImageIO image = new ImageIO(bitmap);

            this.pixels = image.Pixels;
            this.k = k;
            this.image = image;

            clusters = Clusters.Generation(image, k);
            clusterHist = new List<Pixel>();
        }


        /// <summary>
        /// Сегментация
        /// </summary>
        /// <returns></returns>
        public void Sample()
        {
            do
            {
                BatchUpdate();
                OnlineUpdate();
            } while (!IsUntilCluster()) ;

            image.Pixels = pixels;
            Count();
        }


        /// <summary>
        /// Назначние каждому пикселю ближайший кластер
        /// </summary>
        private void BatchUpdate()
        {
            double min = 0.0;
            double buf = 0.0;
            int indexCluster = 0;

            for (int i = 0; i < image.Count; i++)
            {
                min = ImageMath.EuclideanDistance(pixels[i].Color, clusters[0].Color);
                indexCluster = 0;
                for(int j  = 0; j < k; j++)
                {
                    buf = ImageMath.EuclideanDistance(pixels[i].Color, clusters[j].Color);
                    if(min > buf)
                    {
                        min = buf;
                        indexCluster = j;
                    }
                }
                pixels[i].Claster = indexCluster;
            }
        }


        /// <summary>
        /// Перерасчет позиции кластеров
        /// </summary>
        private void OnlineUpdate()
        {
            clusterHist = ImageMath.PixelDeepCopy(clusters);
            ulong[] clustersX = new ulong[k];
            ulong[] clustersY = new ulong[k];
            ulong[] countPixle = new ulong[k];

            Array.Clear(clustersX, 0, k);
            Array.Clear(clustersY, 0, k);
            Array.Clear(countPixle, 0, k);

            for(int i = 0; i < image.Count; i++)
            {
                clustersX[pixels[i].Claster] += (ulong)pixels[i].X;
                clustersY[pixels[i].Claster] += (ulong)pixels[i].Y;
                countPixle[pixels[i].Claster]++;
            }

            for(int i = 0; i < k; i++)
            {
                clusters[i].X = Convert.ToInt32(clustersX[i] / countPixle[i]);
                clusters[i].Y = Convert.ToInt32(clustersY[i] / countPixle[i]);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsUntilCluster()
        {
            int count = 0;
            for (int i = 0; i < k; i++)
                if ((clusterHist[i].X == clusters[i].X) &&
                    (clusterHist[i].Y == clusters[i].Y))
                    count++;

            if (count == k) return true;
            return false;
        }


        /// <summary>
        /// Количество пикселей в области
        /// </summary>
        private void Count()
        {
            count = new int[k];
            for (int i = 0; i < image.Count; i++)
                count[pixels[i].Claster]++;
        }
    }
}
