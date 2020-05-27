using Microsoft.Win32;
//using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageSegmentation.Model.DialogService
{
    class DefaultDialogService : IDialogService
    {
        public string FilePath { get; set; }

        /// <summary>
        /// Проверка на возможность обращения к файлу
        /// </summary>
        /// <returns></returns>
        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }

            return false;
        }


        /// <summary>
        /// Проверка на возможность сохранения файла
        /// </summary>
        /// <returns></returns>
        public bool SaveFileDialog(BitmapSource image, string name)
        {


            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Вывод сообщения на экран
        /// </summary>
        /// <param name="message">Сообщение</param>
        public void ShowMessage(string message) => MessageBox.Show(message);
    }
}
