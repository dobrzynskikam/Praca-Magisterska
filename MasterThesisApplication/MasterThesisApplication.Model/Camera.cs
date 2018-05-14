using System;
using MasterThesisApplication.Model.Annotations;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using Accord.Video;
using Accord.Video.DirectShow;
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
        private VideoCaptureDevice _videoCaptureDevice;
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
                //Messenger.CameraStream.Send<BitmapImage>(CameraImage);
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

        private void Video_NewFrame(object sender, NewFrameEventArgs eventargs)
        {
            try
            {
                BitmapImage bitmapImage;
                using (var bitmap = (Bitmap)eventargs.Frame.Clone())
                {
                    bitmapImage = bitmap.ToBitmapImage();
                    bitmapImage.Freeze();
                }

                CameraImage = bitmapImage;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on Video_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void StopCamera()
        {
            _videoCaptureDevice.SignalToStop();
            _videoCaptureDevice.WaitForStop();
            _videoCaptureDevice.Stop();
            _videoCaptureDevice.NewFrame -= Video_NewFrame;
        }

        public void StartCamera()
        {
            _videoCaptureDevice = new VideoCaptureDevice(MonikerString);
            _videoCaptureDevice.NewFrame += Video_NewFrame;
            _videoCaptureDevice.Start();
        }
    }
}
