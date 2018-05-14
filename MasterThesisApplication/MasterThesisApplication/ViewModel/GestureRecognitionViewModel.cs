using System;
using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Services;
using MasterThesisApplication.Utility;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Accord.Imaging;
using Accord.Imaging.Filters;
using MasterThesisApplication.Model.Utility;

namespace MasterThesisApplication.ViewModel
{
    public class GestureRecognitionViewModel : INotifyPropertyChanged
    {
        //private Camera modelCamera;
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
                SelectedCamera.PropertyChanged += cameraModel_PropertyChanged;
            }
        }

        public BitmapImage CameraImage
        {
            get
            {
                //Messenger.CameraStream.Register<BitmapImage>(this, OnImageReceived);
                return SelectedCamera.CameraImage;
            }
            set
            {
                _cameraImage = value;
                OnPropertyChanged(nameof(CameraImage));
                Messenger.Default.Register<Rectangle>(this, OnRectangleReceived);
                //Messenger.DefaultStream.Register<Rectangle>(this, OnRectangleReceived);
            }
        }

        //public Rectangle Rectangle
        //{
        //    get
        //    {
                
        //        return _rectangle;
        //    }
        //    set
        //    {
        //        _rectangle = value;
        //        OnPropertyChanged(nameof(Rectangle));
        //    }
        //}

        private void OnImageReceived(BitmapImage cameraImage)
        {
            CameraImage = cameraImage;
        }

        private void OnRectangleReceived(Rectangle rect)
        {
            _rectangle = rect;
            //RectanglesMarker marker = new RectanglesMarker(Color.DarkRed);
            //marker.SingleRectangle = rect;
            //var rectangleBitmap = marker.Apply(CameraImage.BitmapImage2Bitmap());
            ////var bitmapasd = rectanImage.ToManagedImage();
            ////_filter.Apply(bitmap).UnlockBits(imgData);
            //var rectangleBitmapImage = rectangleBitmap.ToBitmapImage();
            //rectangleBitmapImage.Freeze();
            //CameraImage = rectangleBitmapImage;
        }


        public GestureRecognitionViewModel()
        {
            ICameraDataService cameraDataService = new CameraDataService();
            VideoDevicesCollection = cameraDataService.GetAllCameras();
            SelectedCamera = VideoDevicesCollection[0];
            Messenger.Default.Send<Camera>(SelectedCamera);
            LoadCommands();
            //this.modelCamera = camera;
        }

        private void cameraModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CameraImage")
            {
                RectanglesMarker marker = new RectanglesMarker(Color.DarkRed);
                marker.SingleRectangle = _rectangle;

                BitmapData imgData = SelectedCamera.CameraImage.BitmapImage2Bitmap().LockBits(ImageLockMode.ReadWrite);

                UnmanagedImage img = new UnmanagedImage(imgData);

                var rectangleBitmap = marker.Apply(img);
                //var bitmapasd = rectanImage.ToManagedImage();
                //_filter.Apply(bitmap).UnlockBits(imgData);
                var rectangleBitmapImage = rectangleBitmap.ToManagedImage().ToBitmapImage();
                rectangleBitmapImage.Freeze();
                CameraImage = rectangleBitmapImage;
                //CameraImage = SelectedCamera.CameraImage;
            }
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
                SelectedCamera.StartCamera();
                //CameraService.StartCamera(SelectedCamera);
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
            //CameraService.StopCamera(SelectedCamera);  
            SelectedCamera.IsRunning = false;
        }

        private bool CanStopCamera(object obj)
        {
            return SelectedCamera.IsRunning;
        }

        private void SetHslFilter(object obj)
        {
            Messenger.Default.Send<Camera>(SelectedCamera);
            DialogService = new HslFilterDialogService();
            DialogService.ShowDialog();
        }
    }
}
