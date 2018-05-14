using MasterThesisApplication.Model.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using MasterThesisApplication;
using MasterThesisApplication.Model.Utility;

namespace MasterThesisApplication.Model
{
    public class Camera : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)

        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isRunning;
        private BitmapImage _cameraImage;
        public string Name { get; set; }
        public string MonikerString { get; set; }

        public BitmapImage CameraImage
        {
            get
            {
                return _cameraImage;
            }
            set
            {
                _cameraImage = value;
                OnPropertyChanged(nameof(CameraImage));
                Messenger.Default.Send<BitmapImage>(CameraImage);
            }
        }
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }
    }
}
