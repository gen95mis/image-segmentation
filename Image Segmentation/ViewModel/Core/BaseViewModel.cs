using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageSegmentation.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnProperyChanged([CallerMemberName] string prop="")
        {
            if(PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
