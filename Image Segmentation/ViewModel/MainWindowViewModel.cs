using System;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;

using ImageSegmentation.Model;
using ImageSegmentation.Model.DialogService;
using ImageSegmentation.Model.FileService;

namespace ImageSegmentation.ViewModel
{
    class MainWindowViewModel : BaseViewModel
    {
        private BitmapSource original;
        private BitmapSource kmeans;
        private BitmapSource hmrf;

        private string path;
        private int countClasters;
        private int sizeKernalGauss;
        private double sigmaKernalGauss;

        private RelayCommand openCommand;
        private RelayCommand saveCommand;
        private RelayCommand processCommand;
        private IDialogService dialogService;
       

        public BitmapSource Original
        {
            get => original;

            set
            {
                original = value;
                OnProperyChanged(nameof(Original));
            }
        }

        public BitmapSource KMeans
        {
            get => kmeans;
            set
            {
                kmeans = value;
                OnProperyChanged(nameof(KMeans));
            }
        }

        public BitmapSource HMRF
        {
            get => hmrf;
            set
            {
                hmrf = value;
                OnProperyChanged(nameof(HMRF));
            }
        }

        public string Path
        {
            get => path;
            set
            {
                path = value;
                OnProperyChanged(nameof(Path));
            }
        }

        public int CountClasters
        {
            get => countClasters;
            set
            {
                countClasters = value;
                OnProperyChanged(nameof(CountClasters));
            }
        }

        public int SizeKernalGauss
        {
            get => sizeKernalGauss;
            set
            {
                sizeKernalGauss = value;
                OnProperyChanged(nameof(SizeKernalGauss));
            }
        }

        public double SigmaKrnalGauss
        {
            get => sigmaKernalGauss;
            set
            {
                sigmaKernalGauss = value;
                OnProperyChanged(nameof(SigmaKrnalGauss));
            }
        }

        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ?? (openCommand = new RelayCommand(obj =>
                {
                    try
                    {
                        if(dialogService.OpenFileDialog() == true)
                        {
                            Bitmap img = ImageFileService.Openfile(dialogService.FilePath);
                            Original = BitmapConversion.BitmapToBitmapSource(img);
                            Path = dialogService.FilePath;
                        }
                    }
                    catch(Exception ex)
                    {
                        dialogService.ShowMessage(ex.Message);
                    }
                }));
            }
        }

        //public RelayCommand SaveCommand
        //{
        //    get
        //    {
        //        return saveCommand ?? (saveCommand = new RelayCommand(obj =>
        //        {
        //            try
        //            {

        //                if (dialogService.SaveFileDialog(hmrf, "hmrf.png") == true) dialogService.ShowMessage("Данные сохранениы.");
        //            }
        //            catch(Exception ex)
        //            {
        //                dialogService.ShowMessage(ex.Message);
        //            }
        //        }));
        //    }
        //}



        public RelayCommand ProcessCommand
        {
            get
            {
                return processCommand ?? (processCommand = new RelayCommand(obj =>
                {
                    try
                    {
                        var mwm = new MainWindowModel(Original, CountClasters, SizeKernalGauss, SigmaKrnalGauss);
                        mwm.Processing();
                        KMeans = mwm.KMeansImage;
                        HMRF = mwm.HMRFImage;
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage(ex.Message);
                    }
                }));
            }
        }


                                                                                                                                                                                                                                                                     
        public MainWindowViewModel()
        {
            dialogService = new DefaultDialogService();

            Path = @"C:\\";
            CountClasters = 5;
            SizeKernalGauss = 9;
            SigmaKrnalGauss = 1.45;
        }
    }
}
