using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Services;
using MasterThesisApplication.Utility;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MasterThesisApplication.Model.Utility;

namespace MasterThesisApplication.ViewModel
{
    public class GestureRecognitionViewModel : INotifyPropertyChanged
    {
        private Camera _selectedCamera;
        private BitmapImage _cameraImage;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICameraDataService CameraService = new CameraDataService();
        public IDialogService DialogService;
        public ICommand StartCameraCommand { get; set; }
        public ICommand StopCameraCommand { get; set; }

        public ICommand SetHslFilterCommand { get; set; }

        public ObservableCollection<Camera> VideoDevicesCollection { get; set; }

        public Camera SelectedCamera
        {
            get => _selectedCamera;
            set
            {
                _selectedCamera = value;
                OnPropertyChanged(nameof(VideoDevicesCollection));
            }
        }

        public BitmapImage CameraImage
        {
            get
            {
                Messenger.Default.Register<BitmapImage>(this, OnImageReceived);
                return _cameraImage;
            }
            set
            {
                _cameraImage = value;
                OnPropertyChanged(nameof(CameraImage));
            }
        }

        private void OnImageReceived(BitmapImage cameraImage)
        {
            CameraImage = cameraImage;
        }


        public GestureRecognitionViewModel()
        {
            ICameraDataService cameraDataService = new CameraDataService();
            VideoDevicesCollection = cameraDataService.GetAllCameras();
            SelectedCamera = VideoDevicesCollection[0];
            LoadCommands();
        }

        private void LoadCommands()
        {
            StartCameraCommand = new CustomCommand(StartCamera, CanStartCamera);
            StopCameraCommand = new CustomCommand(StopCamera, CanStopCamera);
            SetHslFilterCommand = new CustomCommand(SetHslFilter, CanStopCamera);
        }

        private void StartCamera(object obj)
        {
            if (SelectedCamera != null)
            {
                CameraService.StartCamera(SelectedCamera);
                SelectedCamera.IsRunning = true;
            }
        }

        private bool CanStartCamera(object obj)
        {
            return !SelectedCamera.IsRunning;
        }

        private void StopCamera(object obj)
        {
            CameraService.StopCamera(SelectedCamera);  
            SelectedCamera.IsRunning = false;
        }

        private bool CanStopCamera(object obj)
        {
            return SelectedCamera.IsRunning;
        }

        private void SetHslFilter(object obj)
        {
            DialogService = new HslFilterDialogService();
            DialogService.ShowDialog();
        }
    }
}
