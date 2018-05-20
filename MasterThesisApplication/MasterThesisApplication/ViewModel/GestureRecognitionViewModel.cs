using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Model.Utility;
using MasterThesisApplication.Services;
using MasterThesisApplication.Utility;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Accord.Math;
using Messenger = MasterThesisApplication.Utility.Messenger;

namespace MasterThesisApplication.ViewModel
{
    public class GestureRecognitionViewModel : INotifyPropertyChanged
    {
        private Camera _selectedCamera;
        private BitmapImage _cameraImage;
        private Rectangle _rectangle;

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
        public ICommand TakeSnapshotCommand { get; set; }
        public ICommand OpenDatabaseCommand { get; set; }

        public ObservableCollection<Camera> VideoDevicesCollection { get; set; }

        public Camera SelectedCamera
        {
            get
            {
                return _selectedCamera;
            }
            set
            {
                _selectedCamera = value;
                OnPropertyChanged(nameof(VideoDevicesCollection));
                SelectedCamera.PropertyChanged += CameraModel_PropertyChanged;
            }
        }

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
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                Messenger.Default.Register<Rectangle>(this, OnRectangleReceived);
                return _rectangle;
            }
            set
            {
                _rectangle = value;
                OnPropertyChanged(nameof(Rectangle));
            }
        }

        private void OnRectangleReceived(Rectangle rect)
        {
            Rectangle = rect;
        }


        public GestureRecognitionViewModel()
        {
            ICameraDataService cameraDataService = new CameraDataService();
            VideoDevicesCollection = cameraDataService.GetAllCameras();
            SelectedCamera = VideoDevicesCollection[0];
            LoadCommands();
        }

        private void CameraModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CameraImage")
            {
                CameraImage = _selectedCamera.CameraImage.BitmapImage2Bitmap()
                    .DrawRectangle(Rectangle).ToBitmapImage();
            }
        }

        private void LoadCommands()
        {
            StartCameraCommand = new CustomCommand(StartCamera, CanStartCamera);
            StopCameraCommand = new CustomCommand(StopCamera, CanStopCamera);
            SetHslFilterCommand = new CustomCommand(SetHslFilter, CanStopCamera);
            TakeSnapshotCommand = new CustomCommand(TakeSnapshot, CanTakeSnapshot);
            OpenDatabaseCommand = new CustomCommand(OpenDatabase, CanOpenDatabase);
        }

        private bool CanOpenDatabase(object obj)
        {
            return true;
        }

        private void OpenDatabase(object obj)
        {
            DialogService = new DatabaseDialogService();
            DialogService.ShowDialog();
        }

        private bool CanTakeSnapshot(object obj)
        {
            return !Rectangle.Bottom.IsEqual(0);
        }

        private void TakeSnapshot(object obj)
        {
            throw new System.NotImplementedException();
        }

        private void StartCamera(object obj)
        {
            if (SelectedCamera != null)
            {
                SelectedCamera.StartCamera();
                SelectedCamera.IsRunning = true;
            }
        }

        private bool CanStartCamera(object obj)
        {
            return !SelectedCamera.IsRunning;
        }

        private void StopCamera(object obj)
        {
            SelectedCamera.StopCamera();  
            SelectedCamera.IsRunning = false;
        }

        private bool CanStopCamera(object obj)
        {
            return SelectedCamera.IsRunning;
        }

        private void SetHslFilter(object obj)
        {
            Messenger.Default.Send(SelectedCamera);
            DialogService = new HslFilterDialogService();
            DialogService.ShowDialog();
        }
    }
}
