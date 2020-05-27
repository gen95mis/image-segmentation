using System.Drawing;
using System.Windows.Media.Imaging;

using ImageSegmentation.Model.FileService;

namespace ImageSegmentation.Model
{
    class MainWindowModel
    {
        private Bitmap original;
        private BitmapSource kmeansImage;
        private BitmapSource kmeansPPImage;
        private BitmapSource hmrfImage;

        private int countClasters;
        private int sizeKernalGauss;
        private double sigmaKernalGauss;

        public BitmapSource KMeansImage
        {
            get => kmeansImage;
        }

        public BitmapSource KMeansPPImage
        {
            get => kmeansPPImage;
        }

        public BitmapSource HMRFImage
        {
            get => hmrfImage;
        }


        public MainWindowModel(BitmapSource image, int countClasters, int sizeKernalGauss, double sigmaKernalGauss)
        {
            original = BitmapConversion.BitmapSourceToBitmap(image);
            this.countClasters = countClasters;
            this.sizeKernalGauss = sizeKernalGauss;
            this.sigmaKernalGauss = sigmaKernalGauss;
        }


        public void Processing()
        {
            KMeans.KMeans kmeans = new KMeans.KMeans(original, countClasters);
            kmeans.Sample();
            kmeansImage = BitmapConversion.BitmapToBitmapSource(kmeans.Image);

            HMRF.HMRF hmrf = new HMRF.HMRF(original, countClasters, 5, 5, sizeKernalGauss, sigmaKernalGauss);
            var img = hmrf.Classification();
            img.Save("hmrf.png");
            hmrfImage = BitmapConversion.BitmapToBitmapSource(img);
        }
    }
}
