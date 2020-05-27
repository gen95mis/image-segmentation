using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageSegmentation.Model.DialogService
{
    interface IDialogService
    {
        string FilePath { get; set; }

        void ShowMessage(string message);
        bool OpenFileDialog();
        bool SaveFileDialog(BitmapSource image, string name);
    }
}
