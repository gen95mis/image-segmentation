using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ImageSegmentation.ViewModel
{
    class AsyncCommand : AsyncCommandBase
    {
        private readonly Func<Task> command;

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override Task ExecuteAsync(object parameter)
        {
            
            return command();
        }
    }
}
