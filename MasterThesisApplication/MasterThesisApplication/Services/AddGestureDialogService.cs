using MasterThesisApplication.View;

namespace MasterThesisApplication.Services
{
    class AddGestureDialogService :IDialogService
    {
        private AddGestureView _window;

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
