using System;
using ImageSegmentation.Model.Helpers;


namespace ImageSegmentation.Model.HMRF
{
    struct Energy
    {
        public double[] U;
        public double[] Claster;
    }



    class MRF_MAP
    {
        private ImageIO image;
        private double[,] edge;
        private double[,] field;

        private double[] std;
        private double[] avg;

        private int k;
        private int iterMAP;


        public double[,] Field
        {
            get => DeepCopy(field);
        }
        

        public MRF_MAP(ImageIO image, double[,] field, double[,] edge, 
            double[] std, double[] avg,int k, int iterMAP)
        {
            this.field = field;
            this.image = image;
            this.edge = edge;

            this.std = std;
            this.avg = avg;

            this.k = k;
            this.iterMAP = iterMAP;
        }


        public decimal Classification()
        {
            decimal sumU = 0;

            // U - Энергия (обозначение из термодинамики)
            double[,] U = new double[image.Count, k];

            for (int iter = 0; iter < iterMAP; iter++)
            {
                // TODO: Поменть парметры местами
                double[,] U1 = new double[image.Count, k];
                double[,] U2 = new double[image.Count, k];

                for (int l = 0; l < k; l++)
                {
                    // Вычесление первой энергии
                    double[] y = new double[image.Count];
                    for (int i = 0; i < image.Count; i++)
                    {
                        y[i] = (double)(image[i].Color.R - avg[l]);
                        U1[i, l] = Math.Pow(y[i], 2) / (Math.Pow(std[l], 2) * 2);
                        U1[i, l] += Math.Log(std[l]);
                        
                    }

                    // Вычисление второй энергии
                    // TODO: Ускорить работу участка
                    double u2;
                    for (int i = 0; i < image.Count; i++)
                    {
                        u2 = 0.0;

                        if (image[i].X - 1 > 0)
                            if (edge[image[i].X - 1, image[i].Y] == 0 &&
                                l != field[image[i].X - 1, image[i].Y])
                                u2 += 0.5;

                        if (image[i].X + 1 < image.Width)
                            if (edge[image[i].X + 1, image[i].Y] == 0 &&
                                l != field[image[i].X + 1, image[i].Y])
                                u2 += 0.5;

                        if (image[i].Y - 1 > 0)
                            if (edge[image[i].X, image[i].Y - 1] == 0 &&
                                l != field[image[i].X, image[i].Y - 1])
                                u2 += 0.5;

                        if (image[i].Y + 1 < image.Height)
                            if (edge[image[i].X, image[i].Y + 1] == 0 &&
                                l != field[image[i].X, image[i].Y + 1])
                                u2 += 0.5;

                        U2[i, l] = u2;
                    }
                }

                U = Sum(U1, U2);
                Energy energy = MinEnergy(U);
                sumU = Convert.ToDecimal(Sum(energy.U));
                field = ArrayToMatrix(energy.Claster);
            }

            return sumU;
        }


        /// <summary>
        /// Сумма двух матриц
        /// </summary>
        /// <param name="m1">Матрица 1</param>
        /// <param name="m2">Матрица 2</param>
        private double[,] Sum(double[,] matrix1, double[,] matrix2)
        {
            double[,] result = new double[image.Count, k];

            for(int l = 0; l < k; l++)
                for(int i = 0; i < image.Count; i++)
                    result[i, l] = matrix1[i, l] + matrix2[i, l];

            return result;
        }


        /// <summary>
        /// Сумма массива
        /// </summary>
        private double Sum(double[] array)
        {
            double sum =  0.0;

            for (int i = 0; i < image.Count; i++)
                sum += array[i];

            return sum;
        }


        /// <summary>
        /// Минимальная энегрия (в формете структура Energy)
        /// </summary>
        private Energy MinEnergy(double[,] matrix)
        {
            Energy energy = new Energy();

            energy.U = new double[image.Count];
            energy.Claster = new double[image.Count];

            for (int i = 0; i < image.Count; i++)
            {
                double min = matrix[i, 0];
                double label = 0.0;

                for (int l = 0; l < k; l++)
                {
                    if(matrix[i, l] < min)
                    {
                        min = matrix[i, l];
                        label = l;
                    }
                }

                energy.U[i] = min;
                energy.Claster[i] = label;
            }

            return energy;
        }


        private double[,] ArrayToMatrix(double[] array)
        {
            double[,] result = new double[image.Width, image.Height];

            for (int i = 0, count = 0; i < image.Width; i++)
                for (int j = 0; j < image.Height; j++, count++)
                    result[i, j] = array[count];

            return result;
        }


        private double[,] DeepCopy(double[,] matrix)
        {
            double[,] result = new double[image.Width, image.Height];

            for (int i = 0; i < image.Width; i++)
                for (int j = 0; j < image.Height; j++)
                    result[i, j] = matrix[i, j];

            return result;
        }
    }
}
