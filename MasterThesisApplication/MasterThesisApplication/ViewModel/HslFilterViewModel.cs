using Accord;
using Accord.Imaging.Filters;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Utility;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using MasterThesisApplication.Model.Utility;

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

        
        private BitmapImage _cameraImage;
        private readonly HSLFiltering _filter = new HSLFiltering();

        private int _minHue;
        private int _maxHue;
        private float _minSaturation;
        private float _maxSaturation;
        private float _minLuminance;
        private float _maxLuminance;

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

        private void OnImageReceived(BitmapImage image)
        {
            var bitmap = BitmapImage2Bitmap(image);
            var filteredBitmap = _filter.Apply(bitmap).ToBitmapImage();
            filteredBitmap.Freeze();
            CameraImage = filteredBitmap;
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
            MinHue = minHue;
            MaxHue = maxHue;
            MinSaturation = minSaturation;
            MaxSaturation= maxSaturation;
            MinLuminance = minLuminance;
            MaxLuminance = maxLuminance;

            UpdateFilter();
        }

        private void UpdateFilter()
        {
            _filter.Hue = new IntRange(MinHue, MaxHue);
            _filter.Saturation = new Range(MinSaturation, MaxSaturation);
            _filter.Luminance = new Range(MinLuminance, MaxLuminance);
            //_filter.ApplyInPlace(BitmapImage2Bitmap(CameraImage));
        }

        private static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}
