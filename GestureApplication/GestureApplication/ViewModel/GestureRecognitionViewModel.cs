using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using GestureApplication.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GestureApplication.ViewModel
{
    public class GestureRecognitionViewModel : INotifyPropertyChanged
    {
        
        public ObservableCollection<FilterInfo> VideoDevices { get; set; }

        private int threshold;
        public int Threshold
        {
            get
            {
                return threshold;
            }
            set
            {
                threshold = value;
                RaisePropertyChanged("Threshold");
            }
        }
        public FilterInfo CurrentDevice
        {
            get
            {
                return currentDevice;
            }
            set
            {
                currentDevice = value;
                RaisePropertyChanged("CurrentDevice");
            }
        }
        private BitmapImage bitmap;
        public BitmapImage Bitmap
        {
            get
            {
                return bitmap;
            }
            set
            {
                bitmap = value;
                RaisePropertyChanged("Bitmap");
            }
        }

        public BitmapImage BinaryBitmapImage
        {
            get
            {
                return binaryBitmapImage;
            }
            set
            {
                binaryBitmapImage = value;
                RaisePropertyChanged("BinaryBitmapImage");
            }
        }

        public BitmapImage ThresholdBitmapImage
        {
            get
            {
                return thresholdBitmapImage;
            }
            set
            {
                thresholdBitmapImage = value;
                RaisePropertyChanged("ThresholdBitmapImage");
            }
        }

        public BitmapImage CannyEdgeBitmapImage
        {
            get
            {
                return cannyEdgeBitmapImage;
            }
            set
            {
                cannyEdgeBitmapImage = value;
                RaisePropertyChanged("CannyEdgeBitmapImage");
            }
        }

        public BitmapImage ColorFilterBitmapImage
        {
            get
            {
                return colorBitmapFilterImage;
            }
            set
            {
                colorBitmapFilterImage = value;
                RaisePropertyChanged("ColorFilterBitmapImage");
            }
        }

        private int text;
        private FilterInfo currentDevice;

        public int Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                RaisePropertyChanged("Text");
            }
        }

        private IVideoSource _videoSource;
        private BitmapImage binaryBitmapImage;
        private BitmapImage thresholdBitmapImage;
        private BitmapImage cannyEdgeBitmapImage;
        private BitmapImage colorBitmapFilterImage;

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand StartCameraCommand { get; set; }
        public ICommand StopCameraCommand { get; set; }

        public GestureRecognitionViewModel()
        {
            LoadCommands();
            GetVideoDevices();
        }

        private void GetVideoDevices()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            foreach (FilterInfo filterInfo in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                VideoDevices.Add(filterInfo);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
            else
            {
                MessageBox.Show("No video sources found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void LoadCommands()
        {
            StartCameraCommand = new CustomCommand(StartCamera, CanStartCamera);
            StopCameraCommand = new CustomCommand(StopCamera, CanStopCamera);
        }

        private bool CanStopCamera(object obj)
        {
            return true;
        }

        private void StopCamera(object obj)
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= new NewFrameEventHandler(video_NewFrame);
            }
        }

        private bool CanStartCamera(object obj)
        {
            return true;
        }

        private void StartCamera(object obj)
        {

            if (CurrentDevice != null)
            {
                _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                _videoSource.NewFrame += video_NewFrame;
                //add counter and then comparer w
                //Text = _videoSource.FramesReceived.ToString();
                _videoSource.Start();
            }
        }

        private async void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                VideoCaptureDevice device = sender as VideoCaptureDevice;
                Text +=1;
                BitmapImage bi, binary, threshold, cannyEdge, colorFilter;
                using (var bit = (Bitmap)eventArgs.Frame.Clone())
                {
                    //binary = bit.ToBinaryBitmap().ToBitmapImage();
                    bi = bit.ToBitmapImage();
                    bi.Freeze();
                    colorFilter = bit.ColorFilter().BiggestBlob().ToBitmapImage();
                    colorFilter.Freeze();
                    //threshold = bit.ThresholdBitmap(Threshold);
                    //if (Text % 10 == 0)
                    //{
                    //    colorFilter = bit.ColorFilter().ToBitmapImage();
                    //    colorFilter.Freeze();
                    //    await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.ColorFilterBitmapImage = colorFilter));
                    //}
                    //colorFilter = bit.ToBinaryBitmap().ThresholdBitmap(Threshold).UseCannyEdgeDetector().ToBitmapImage();
                    //cannyEdge = bit.UseCannyEdgeDetector(Threshold);
                }
                
                 // avoid cross thread operations and prevents leaks
                //binary.Freeze();
                //colorFilter.Freeze();
                //colorFilter.Freeze();
                //cannyEdge.Freeze();
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Bitmap = bi));
                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.BinaryBitmapImage = binary));
                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.ThresholdBitmapImage = threshold));
                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.CannyEdgeBitmapImage = cannyEdge));
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.ColorFilterBitmapImage = colorFilter));
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCamera(null);
            }
        }
    }
}
