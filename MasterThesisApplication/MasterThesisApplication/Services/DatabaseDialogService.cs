using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
