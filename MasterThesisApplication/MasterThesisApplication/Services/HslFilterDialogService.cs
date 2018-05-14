using MasterThesisApplication.View;
using System.Windows;
using System.Windows.Controls;
using MasterThesisApplication.Model;

namespace MasterThesisApplication.Services
{
    public class HslFilterDialogService : IDialogService
    {
        private Window _window;
        public void ShowDialog()
        {
            _window = new HslFilterView();
            _window.ShowDialog();
        }

        public void CloseDialog()
        {
            _window?.Close();
        }
    }
}
