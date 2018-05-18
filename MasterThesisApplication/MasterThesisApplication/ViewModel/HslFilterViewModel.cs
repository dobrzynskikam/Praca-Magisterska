using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Vision.Tracking;
using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Model.Utility;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace MasterThesisApplication.ViewModel
{
    public class HslFilterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
            }
        }

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

                _hslBlobTracker = new HslBlobTracker(_filter)
                {
                    MinHeight = 100,
                    MinWidth = 150,
                    Extract = true
                };
                var imgData = filteredBitmap.LockBits(ImageLockMode.ReadWrite);

                var img = new UnmanagedImage(imgData);

                _hslBlobTracker.ProcessFrame(img);
                var rect = _hslBlobTracker.TrackingObject.Rectangle;
                _selectedCamera.Rectangle = rect;
                Messenger.Default.Send(rect);

                RectanglesMarker marker = new RectanglesMarker(rect);
                var rectanImage = marker.Apply(img);
                var bitmapasd = rectanImage.ToManagedImage();
                var rectangleBitmapImage = bitmapasd.ToBitmapImage();
                rectangleBitmapImage.Freeze();
                CameraImage = rectangleBitmapImage;
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
