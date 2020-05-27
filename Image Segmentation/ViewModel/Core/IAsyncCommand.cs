using System.Windows.Input;
using System.Threading.Tasks;
namespace ImageSegmentation.ViewModel
{
    interface IAsyncCommand:ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
