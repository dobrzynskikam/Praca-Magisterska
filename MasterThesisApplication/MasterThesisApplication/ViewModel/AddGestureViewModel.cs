using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Utility;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MasterThesisApplication.Services;

namespace MasterThesisApplication.ViewModel
{
    public class AddGestureViewModel :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand LoadGestureCommand { get; set; }
        public ICommand ComputeCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        //private int _numberOfBow;

        //public int NumberOfBow
        //{
        //    get { return _numberOfBow; }
        //    set
        //    {
        //        _numberOfBow = value;
        //        OnPropertyChanged(nameof(NumberOfBow));
        //    }
        //}

        //private ObservableCollection<Feature> _featureCollection = new ObservableCollection<Feature>();
        //public ObservableCollection<Feature> FeatureCollection
        //{
        //    get { return _featureCollection; }
        //    set
        //    {
        //        _featureCollection = value;
        //        OnPropertyChanged(nameof(FeatureCollection));
        //    }
        //}

        private Gesture _gestureToSave;
        public Gesture GestureToSave
        {
            get
            {
                return _gestureToSave;
            }
            set
            {
                _gestureToSave = value;
                OnPropertyChanged(nameof(GestureToSave));
            }
        }

        public AddGestureViewModel(int numberOfBow)
        {
            GestureToSave = new Gesture();
            GestureToSave.BowNumber = numberOfBow;
            LoadCommands();
        }

        private void LoadCommands()
        {
            ComputeCommand = new CustomCommand(ComputeBow, CanComputeBow);
            LoadGestureCommand = new CustomCommand(LoadGesture, CanLoadGesture);
            SaveCommand = new CustomCommand(Save, CanSave);
        }

        private void Save(object obj)
        {
            IGestureDataService gestureDataService = new GestureDataService();
            var existingGestures = gestureDataService.GetAllGestures();

            //true if gesture to save exists in database
            if (existingGestures.Any(g => g.GestureName == GestureToSave.GestureName))
            {
                
            }
            else
            {
                //add new gesture to xml with name and BoW
                gestureDataService.AddNewGesture(GestureToSave);
            }
        }

        private bool CanSave(object obj)
        {
            if (GestureToSave.FeatureList == null || GestureToSave.GestureName == null)
            {
                return false;
            }

            return GestureToSave.FeatureList.Count != 0 && GestureToSave.GestureName != "";

            //return GestureToSave.FeatureList.Count != 0;
        }

        private bool CanLoadGesture(object obj)
        {
            return true;
        }

        private void LoadGesture(object obj)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Multiselect = true;
            openFile.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            openFile.InitialDirectory = Environment.CurrentDirectory;
            if (openFile.ShowDialog() == true)
            {

                var newFeatureList = new ObservableCollection<Feature>();
                foreach (var fileName in openFile.FileNames)
                {
                    newFeatureList.Add(new Feature()
                    {
                        ImageName = fileName
                    });
                }

                GestureToSave.FeatureList = newFeatureList;
            }
        }

        private bool CanComputeBow(object obj)
        {
            return true;
            //return GestureToSave.FeatureList.Count != 0;
        }

        private void ComputeBow(object obj)
        {
            //BinarySplit binarySplit = new BinarySplit(NumberOfBow);

            //// Create bag-of-words (BoW) with the given algorithm
            //BagOfVisualWords surfBow = new BagOfVisualWords(binarySplit);

            //_images = GetImages(FeatureCollection);

            //// Compute the BoW codebook using training images only
            //_bow = surfBow.Learn(_images.Values.ToArray());

            //foreach (var feature in FeatureCollection)
            //{
                
            //    var image = (Bitmap)Image.FromFile(feature.ImageName);
              
            //    double[] featureVector = (_bow as ITransform<Bitmap, double[]>).Transform(image);
            //    feature.Vector = featureVector.ToString(DefaultArrayFormatProvider.InvariantCulture);
            //}
        }

        //private Dictionary<string, Bitmap> GetImages(ObservableCollection<Feature> featureCollection)
        //{
        //    Dictionary<string, Bitmap> imageDict = new Dictionary<string, Bitmap>();
        //    foreach (var feature in featureCollection)
        //    {
        //        imageDict.Add(feature.ImageName, (Bitmap)Image.FromFile(feature.ImageName));
        //    }

        //    return imageDict;
        //}
    }
}
