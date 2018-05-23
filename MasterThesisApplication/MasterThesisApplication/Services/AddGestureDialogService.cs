using System.Windows;
using MasterThesisApplication.View;

namespace MasterThesisApplication.Services
{
    public class AddGestureDialogService :IDialogService
    {
        private Window _window;

        public void ShowDialog()
        {
            _window = new AddGestureView();
            _window.ShowDialog();
        }

        public void CloseDialog()
        {
            _window?.Close();
        }
    }
}
