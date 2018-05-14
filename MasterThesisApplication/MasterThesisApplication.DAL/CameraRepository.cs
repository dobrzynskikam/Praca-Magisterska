using Accord.Video;
using Accord.Video.DirectShow;
using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Utility;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MasterThesisApplication.DAL
{
    public class CameraRepository : ICameraRepository
    {
        private ObservableCollection<Camera> _cameras;
        private VideoCaptureDevice _videoCaptureDevice;
        //private readonly Camera _camera= new Camera();

        public ObservableCollection<Camera> GetCameras()
        {
            if (_cameras == null)
            {
                GetVideoDevices();
            }

            return _cameras;
        }

        private void GetVideoDevices()
        {
            _cameras = new ObservableCollection<Camera>();
            foreach (var device in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                _cameras.Add(new Camera() { Name = device.Name, MonikerString = device.MonikerString});
            }
        }

        //private void Video_NewFrame(object sender, NewFrameEventArgs eventargs)
        //{
        //    try
        //    {
        //        BitmapImage bitmapImage;
        //        using (var bitmap = (Bitmap)eventargs.Frame.Clone())
        //        {
        //            bitmapImage = bitmap.ToBitmapImage();
        //            bitmapImage.Freeze();
        //        }

        //        _camera.CameraImage = bitmapImage;
        //    }
        //    catch (Exception exc)
        //    {
        //        MessageBox.Show("Error on Video_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //public void StopCamera(Camera camera)
        //{
        //    _videoCaptureDevice.SignalToStop();
        //    _videoCaptureDevice.WaitForStop();
        //    _videoCaptureDevice.Stop();
        //    _videoCaptureDevice.NewFrame -= Video_NewFrame;
        //}

        //public void StartCamera(Camera camera)
        //{
        //    _videoCaptureDevice = new VideoCaptureDevice(camera.MonikerString);
        //    _videoCaptureDevice.NewFrame += Video_NewFrame;
        //    _videoCaptureDevice.Start();
        //}
    }
}
