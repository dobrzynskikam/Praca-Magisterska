using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Services;
using MasterThesisApplication.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        private string _statusText;

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                OnPropertyChanged(nameof(StatusText));
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

            return GestureToSave.FeatureList.Count != 0 && _svm.CanClassify();
            //return GestureToSave.FeatureList.Count != 0 && GestureCollection.Count != 0;
        }

        private void Test(object obj)
        {
            var sw1 = Stopwatch.StartNew();
            _svm.CreateDescriptors(GestureToSave, false);
            sw1.Stop();

            var sw2 = Stopwatch.StartNew();
            var expectedLabel = GestureList.First(d => d.Value == ExpectedGesture.Value).Key;
            _svm.Classify(GestureToSave,expectedLabel);
            sw2.Stop();

            float positiveHits = GestureToSave.FeatureList.Count(f => f.State == FeatureState.CorrectClassification);

            float allFeatures = GestureToSave.FeatureList.Count;

            StatusText = "\nFeatures extracted in " + sw1.Elapsed.TotalSeconds.ToString("F") + "s."+
                         "\nClassification took " + sw2.Elapsed.TotalSeconds.ToString("F") + ".s" +
                         $"\nPositiveHits: {positiveHits}; Total: {allFeatures}" +
                         "\nAccuracy: " + (positiveHits / allFeatures * 100).ToString("F") + "%";
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
            var openFile = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png",
                InitialDirectory = Environment.CurrentDirectory
            };
            if (openFile.ShowDialog() != true) return;
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
}
