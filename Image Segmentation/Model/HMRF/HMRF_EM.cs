using System;
using ImageSegmentation.Model.Helpers;


namespace ImageSegmentation.Model.HMRF
{
    class HMRF_EM
    {
        private ImageIO image;
        private double[,] field;
        private double[,] edge;
        private double[] std;
        private double[] avg;
        private int k;
        private int iterEM;
        private int iterMAP;

        private int width;
        private int height;
        private int count;


        public double[,] Field
        {
            get => field;
        }

        public HMRF_EM(ImageIO image, double[,] field,  double[,] edge, int k,
            double[] std, double[] avg, int iterEM, int iterMAP)
        {
            this.image = image;
            this.field = field;
            this.edge = edge;
            this.std = std;
            this.avg = avg;
            this.k = k;
            this.iterEM = iterEM;
            this.iterMAP = iterMAP;

            width = image.Width;
            height = image.Height;
            count = width * height;
        }


        public void Classification()
        {

            double[,] PLY = new double[k, count];  // P(L|Yi) Y - иссходное изображение (image)
            double[] gauss = new double[count];     // G(Yi;{AVG, STD})
            double[] PLX = new double[count];      // P(L|X) L - Edge; X - Field
            double[] PY = new double[count];       // p(Y)

            decimal[] sumU = new decimal[iterEM];

            for (int iter = 0; iter < iterMAP; iter++)
            {
                MRF_MAP mrf = new MRF_MAP(image, field, edge, std, avg, k, iterMAP);
                sumU[iter] = mrf.Classification();
                field = mrf.Field;

                for (int l = 0; l < k; l++)
                {
                  
                    for (int i = 0; i < image.Count; i++)
                    {
                        double u = 0.0;
                        if (image[i].X - 1 > 0)
                            if (edge[image[i].X - 1, image[i].Y] == 0 &&
                                l != field[image[i].X - 1, image[i].Y])
                                u += 0.5;

                        if (image[i].X + 1 < image.Width)
                            if (edge[image[i].X + 1, image[i].Y] == 0 &&
                                l != field[image[i].X + 1, image[i].Y])
                                u += 0.5;

                        if (image[i].Y - 1 > 0)
                            if (edge[image[i].X, image[i].Y - 1] == 0 &&
                                l != field[image[i].X, image[i].Y - 1])
                                u += 0.5;

                        if (image[i].Y + 1 < image.Height)
                            if (edge[image[i].X, image[i].Y + 1] == 0 &&
                                l != field[image[i].X, image[i].Y + 1])
                                u += 0.5;

                        PLX[i] = u;
                    }


                    //TODO: Все три цыкла можно будет объеденить в один
                    // Gauss(Y[i], AVR[l], STD[l])
                    for (int i = 0; i < count; i++)
                    {
                        double temp1 = 1 / Math.Sqrt(2 * Math.Pow(std[l], 2) * Math.PI);
                        double temp2 = Math.Exp( -Math.Pow((image[i].Color.R - avg[l]), 2) / (2 * Math.Pow(std[l], 2)));
                        PY[i] += temp1 * temp2 * Math.Exp(PLX[i]);
                        PLY[l, i] = temp1 * temp2 * PLX[i];
                    }
                }


                for (int l = 0; l < k; l++)
                {
                    for (int i = 0; i < count; i++)
                    {
                        double temp1 = 1 / Math.Sqrt(2 * Math.Pow(std[l], 2) * Math.PI);
                        double temp2 = Math.Exp(-Math.Pow((image[i].Color.R - avg[l]), 2) / (2 * Math.Pow(std[l], 2)));
                        gauss[i] = temp1 * temp2 * Math.Exp(PLX[i]); ;
                        PLY[l, i] = gauss[i] / PY[i];
                    }
                }

                AVG(PLY);
                STD(PLY);
            }
        }


        private void STD(double[,] PLY)
        {
            double sum;
            double[] sigma = new double[k];

            for (int l = 0; l < k; l++)
            {
                sum = 0.0;
                for(int i = 0; i < count; i++)
                {
                    sigma[l] += PLY[l, i] * Math.Pow(image[i].Color.R - avg[l], 2);
                    sum += PLY[l, i];
                }

                std[l] = Math.Sqrt(sigma[l] / sum);
            }
        }


        private void AVG(double[,] PLY)
        {
            double sum;
            double[] mu = new double[k];

            for (int l = 0; l < k; l++)
            {
                sum = 0.0;
                for (int i = 0; i < count; i++)
                {
                    mu[l] += PLY[l, i] * image[i].Color.R;
                    sum += PLY[l, i];
                }

                avg[l] = mu[l] / sum;
            }
        }

   
        private double[] Sum(double[,] matrix)
        {
            int size = matrix.GetLength(1);
            double[] result = new double[size];

            for (int l = 0; l < k; l++)
                for (int i = 0; i < size; i++)
                    result[i] += matrix[l, i];

            return result;
        }
    }
}