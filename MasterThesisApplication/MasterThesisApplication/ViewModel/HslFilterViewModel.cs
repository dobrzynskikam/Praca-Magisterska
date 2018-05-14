using Accord;
using Accord.Imaging.Filters;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Utility;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using Accord.Vision.Tracking;
using MasterThesisApplication.Model.Utility;
using Accord.Imaging;
using MasterThesisApplication.Model;

namespace MasterThesisApplication.ViewModel
{
    public class HslFilterViewModel : INotifyPropertyChanged
    {
        //private BitmapImage _cameraImage;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //private Camera _selectedCamera;
        //public Camera SelectedCamera
        //{
        //    get
        //    {
        //        return _selectedCamera;
        //    }
        //    set
        //    {
        //        _selectedCamera = value;
        //        OnPropertyChanged(nameof(SelectedCamera));
        //    }
        //}

        private BitmapImage _cameraImage;
        private readonly HSLFiltering _filter = new HSLFiltering();
        private HslBlobTracker _hslBlobTracker;

        private int _minHue;
        private int _maxHue;
        private float _minSaturation;
        private float _maxSaturation;
        private float _minLuminance;
        private float _maxLuminance;
        private Rectangle _rectangle;
        private Camera _selectedCamera;

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
            get { return _rectangle; }
            set
            {
                _rectangle = value;
                OnPropertyChanged(nameof(Rectangle));
                //Messenger.DefaultStream.Send<Rectangle>(Rectangle);
            }
        }

        //private void OnImageReceived(BitmapImage image)
        //{
        //    var bitmap = BitmapImage2Bitmap(image);
        //    var filteredBitmap = _filter.Apply(bitmap);
            
        //    _hslBlobTracker = new HslBlobTracker(_filter);
        //    _hslBlobTracker.MinHeight = 100;
        //    _hslBlobTracker.MinWidth = 150;
        //    //_hslBlobTracker.MaxHeight = 200;
        //    //_hslBlobTracker.MaxWidth = 200;
        //    _hslBlobTracker.Extract = true;
        //    BitmapData imgData = filteredBitmap.LockBits(ImageLockMode.ReadWrite);

        //    UnmanagedImage img = new UnmanagedImage(imgData);

        //    _hslBlobTracker.ProcessFrame(img);
        //    Rectangle rect = _hslBlobTracker.TrackingObject.Rectangle;
        //    Rectangle = rect;
        //    //Messenger.DefaultStream.Send<Rectangle>(rect);

        //    RectanglesMarker marker = new RectanglesMarker(rect);
        //    var rectangleInRealBitmap = marker.Apply(imgData);
        //    var rectanImage = marker.Apply(img);
        //    var bitmapasd = rectanImage.ToManagedImage();
        //    //_filter.Apply(bitmap).UnlockBits(imgData);
        //    var rectangleBitmapImage = bitmapasd.ToBitmapImage();
        //    var rectangleInRealBitmapImage = rectangleInRealBitmap.ToBitmapImage();
        //    rectangleInRealBitmapImage.Freeze();
        //    rectangleBitmapImage.Freeze();
        //    CameraImage = rectangleBitmapImage;
        //}

        public int MinHue
        {
            get
            {
                return _minHue;
            }
            set
            {
                _minHue = value;
                OnPropertyChanged(nameof(MinHue));
                UpdateFilter();
            }

        }

        public int MaxHue
        {
            get
            {
                return _maxHue;
            }
            set
            {
                _maxHue = value;
                OnPropertyChanged(nameof(MaxHue));
                UpdateFilter();
            }

        }

        public float MinSaturation
        {
            get
            {
                return _minSaturation;
            }
            set
            {
                _minSaturation = value;
                OnPropertyChanged(nameof(MinSaturation));
                UpdateFilter();
            }

        }

        public float MaxSaturation
        {
            get
            {
                return _maxSaturation;
            }
            set
            {
                _maxSaturation = value;
                OnPropertyChanged(nameof(MaxSaturation));
                UpdateFilter();
            }

        }

        public float MinLuminance
        {
            get
            {
                return _minLuminance;
            }
            set
            {
                _minLuminance = value;
                OnPropertyChanged(nameof(MinLuminance));
                UpdateFilter();
            }

        }

        public float MaxLuminance
        {
            get
            {
                return _maxLuminance;
            } 
            set
            {
                _maxLuminance = value;
                OnPropertyChanged(nameof(MaxLuminance));
                UpdateFilter();
            }

        }

        public HslFilterViewModel(int minHue, int maxHue, float minSaturation, float maxSaturation, float minLuminance, float maxLuminance)
        {
            Messenger.Default.Register<Camera>(this, OnCameraReceived);
            MinHue = minHue;
            MaxHue = maxHue;
            MinSaturation = minSaturation;
            MaxSaturation= maxSaturation;
            MinLuminance = minLuminance;
            MaxLuminance = maxLuminance;
            UpdateFilter();
        }

        private void OnCameraReceived(Camera camera)
        {
            _selectedCamera = camera;
            _selectedCamera.PropertyChanged += cameraModel_PropertyChanged;
        }

        private void cameraModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CameraImage")
            {
                var bitmap = _selectedCamera.CameraImage.BitmapImage2Bitmap();
                var filteredBitmap = _filter.Apply(bitmap);
                //var filteredBitmapImage = filteredBitmap.ToBitmapImage();
                //filteredBitmapImage.Freeze();

                _hslBlobTracker = new HslBlobTracker(_filter);
                _hslBlobTracker.MinHeight = 100;
                _hslBlobTracker.MinWidth = 150;
                //_hslBlobTracker.MaxHeight = 200;
                //_hslBlobTracker.MaxWidth = 200;
                _hslBlobTracker.Extract = true;
                BitmapData imgData = filteredBitmap.LockBits(ImageLockMode.ReadWrite);

                UnmanagedImage img = new UnmanagedImage(imgData);

                _hslBlobTracker.ProcessFrame(img);
                Rectangle rect = _hslBlobTracker.TrackingObject.Rectangle;
                Rectangle = rect;
                Messenger.Default.Send<Rectangle>(rect);

                RectanglesMarker marker = new RectanglesMarker(rect);
                //var rectangleInRealBitmap = marker.Apply(imgData);
                var rectanImage = marker.Apply(img);
                var bitmapasd = rectanImage.ToManagedImage();
                //_filter.Apply(bitmap).UnlockBits(imgData);
                var rectangleBitmapImage = bitmapasd.ToBitmapImage();
                //var rectangleInRealBitmapImage = rectangleInRealBitmap.ToBitmapImage();
                //rectangleInRealBitmapImage.Freeze();
                rectangleBitmapImage.Freeze();
                CameraImage = rectangleBitmapImage;

                //CameraImage = filteredBitmapImage;
            }
        }
        private void UpdateFilter()
        {
            _filter.Hue = new IntRange(MinHue, MaxHue);
            _filter.Saturation = new Range(MinSaturation, MaxSaturation);
            _filter.Luminance = new Range(MinLuminance, MaxLuminance);
        }
    }
}
