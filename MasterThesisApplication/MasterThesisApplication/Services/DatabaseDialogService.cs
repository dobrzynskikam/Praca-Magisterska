using MasterThesisApplication.View;

namespace MasterThesisApplication.Services
{
    class DatabaseDialogService : IDialogService
    {
        private GestureDatabaseView _window;

        public void ShowDialog()
        {
            _window = new GestureDatabaseView();
            _window.ShowDialog();
        }

        public void CloseDialog()
        {
            _window?.Close();
        }
    }
}
