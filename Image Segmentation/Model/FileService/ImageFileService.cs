using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageSegmentation.Model.FileService
{
    static class ImageFileService
    {
        public static Bitmap Openfile(string fileName) => new Bitmap(fileName);


        public static void SaveFile(string path, string nameFolder, BitmapSource original, BitmapSource kmeans, BitmapSource kmeansPP, BitmapSource hmrf)
        {
            throw new NotImplementedException();
        }
    }
}
