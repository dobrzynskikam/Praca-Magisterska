using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Services;
using MasterThesisApplication.Utility;
using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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
        public ICommand SaveCommand { get; set; }
        public ICommand TestCommand { get; set; }

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

        private Dictionary<int, string> _gestureList;

        public Dictionary<int, string> GestureList
        {
            get { return _gestureList; }
            set
            {
                _gestureList = value;
                OnPropertyChanged(nameof(GestureList));
            }
        }

        private KeyValuePair<int, string> _expectedGesture;

        public KeyValuePair<int, string> ExpectedGesture
        {
            get { return _expectedGesture; }
            set
            {
                _expectedGesture = value;
                OnPropertyChanged(nameof(ExpectedGesture));
            }
        }
        //private ObservableCollection<Gesture> _gestureCollection;

        //public ObservableCollection<Gesture> GestureCollection
        //{
        //    get { return _gestureCollection; }
        //    set
        //    {
        //        _gestureCollection = value;
        //        OnPropertyChanged(nameof(GestureCollection));
        //    }
        //}

        private Classifier _svm = new Classifier();

        public AddGestureViewModel()
        {
            GestureToSave = new Gesture()
            {
                FeatureList = new ObservableCollection<Feature>()
            };
            LoadCommands();
            //Messenger.Default.Register<ObservableCollection<Gesture>>(this, OnGestureCollectionReceived);
            Messenger.Default.Register<Dictionary<int, string>>(this, OnGestureListReceived);
            Messenger.Default.Register<Classifier>(_svm, OnClassifierReceived);
        }

        private void OnGestureListReceived(Dictionary<int, string> list)
        {
            GestureList = list;
        }

        private void OnClassifierReceived(Classifier svm)
        {
            _svm = svm;
        }

        //private void OnGestureCollectionReceived(ObservableCollection<Gesture> gestureCollection)
        //{
        //    GestureCollection = gestureCollection;
        //}

        private void LoadCommands()
        {
            TestCommand = new CustomCommand(Test, CanTest);
            LoadGestureCommand = new CustomCommand(LoadGesture, CanLoadGesture);
            SaveCommand = new CustomCommand(Save, CanSave);
        }

        private bool CanTest(object obj)
        {
            return true;
            //return GestureToSave.FeatureList.Count != 0 && GestureCollection.Count != 0;
        }

        private void Test(object obj)
        {
            Messenger.Default.Register<Classifier>(this, OnClassifierReceived);
            _svm.CreateDescriptors(GestureToSave, false);
            //POPRAWKA
            var ssss = GestureList.Select(d => d.Value);

            var expectedLabel = GestureList.First(d => d.Value == ExpectedGesture.Value).Key;
            _svm.Classify(GestureToSave,expectedLabel);
        }

        private void Save(object obj)
        {
            ////var gestureNameToSave = _gestureList.First(g => g.GestureName == GestureToSave.GestureName);
            //if (_gestureList.Any(g => g.GestureName == GestureToSave.GestureName))
            //{
            //    //Add to existing gesture
            //    _gestureList.AddRange(g => g.GestureName == GestureToSave.GestureName);

            //}
            IGestureDataService gestureDataService = new GestureDataService();
           
            gestureDataService.AddNewGesture(GestureToSave);
            Messenger.Default.Send(new Gesture());
        }

        private bool CanSave(object obj)
        {
            if (GestureToSave.FeatureList == null || GestureToSave.GestureName == null)
            {
                return false;
            }

            return GestureToSave.FeatureList.Count != 0 && GestureToSave.GestureName != "";
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

        //private bool CanComputeBow(object obj)
        //{
        //    return true;
        //}

        //private void ComputeBow(object obj)
        //{
            
        //    //BinarySplit binarySplit = new BinarySplit(NumberOfBow);

        //    //// Create bag-of-words (BoW) with the given algorithm
        //    //BagOfVisualWords surfBow = new BagOfVisualWords(binarySplit);

        //    //_images = GetImages(FeatureCollection);

        //    //// Compute the BoW codebook using training images only
        //    //_bow = surfBow.Learn(_images.Values.ToArray());

        //    //foreach (var feature in FeatureCollection)
        //    //{
                
        //    //    var image = (Bitmap)Image.FromFile(feature.ImageName);
              
        //    //    double[] featureVector = (_bow as ITransform<Bitmap, double[]>).Transform(image);
        //    //    feature.Vector = featureVector.ToString(DefaultArrayFormatProvider.InvariantCulture);
        //    //}
        //}

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
