using System.Collections.Generic;
using System.Drawing;
using ImageSegmentation.Model.Helpers;



namespace ImageSegmentation.Model.HMRF
{
    public class HMRF
    {
        int k;
        int iterEM;
        int iterMAP;


        private ImageIO image;
        private KMeans.KMeans kMeans;
        private Canny canny;


        public HMRF(Bitmap image, int k, int iterEM, int iterMAP, int sizeKernalGass, double sigmaKernalGauss)
        {
            this.k = k;
            this.iterEM = iterEM;
            this.iterMAP = iterMAP;

            this.image = new ImageIO(image);

            canny = new Canny(image, sizeKernalGass, sigmaKernalGauss);
            canny.EdgeDetection();
            
            //KMeans
            ImageIO img = new ImageIO(image);
            kMeans = new KMeans.KMeans(ref img, k);
            kMeans.Sample();
            img.Save(kMeans.LastCluster, "kmeans.png");
        }


        public Bitmap Classification()
        {
            double[] std = kMeans.STD;
            double[] avg = kMeans.AVG;

            HMRF_EM em = new HMRF_EM(image, kMeans.Field, canny.Mask, k, std, avg, iterEM, iterMAP);
            em.Classification();

            return GetImage(em.Field);
        }


        private Bitmap GetImage(double[,] field)
        {
            Bitmap img = new Bitmap(image.Width, image.Height);
            List<Pixel> cluster = kMeans.LastCluster;
            for(int i = 0; i < image.Width; i++)
            {
                for(int j = 0; j < image.Height; j++)
                {
                    byte R = cluster[(int)field[i, j]].Color.R;
                    img.SetPixel(i, j, Color.FromArgb(R, R, R));
                }
            }

            return img;
        }

    }
}
