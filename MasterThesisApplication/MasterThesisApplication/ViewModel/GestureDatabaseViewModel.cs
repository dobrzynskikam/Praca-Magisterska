using Accord.Imaging;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Services;
using MasterThesisApplication.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MasterThesisApplication.ViewModel
{
    public class GestureDatabaseViewModel: INotifyPropertyChanged
    {

        public ICommand AddGestureCommand { get; set; }
        public ICommand ComputeCommand { get; set; }
        public IDialogService DialogService;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IGestureDataService GestureDataService = new GestureDataService();

        private ObservableCollection<Gesture> _gestureCollection = new ObservableCollection<Gesture>();
        public ObservableCollection<Gesture> GestureCollection
        {
            get { return _gestureCollection; }
            set
            {
                _gestureCollection = value;
                OnPropertyChanged(nameof(GestureCollection));
            }
        }

        private Gesture _selectedGesture;
        public Gesture SelectedGesture
        {
            get
            {
                return _selectedGesture;
            }
            set
            {
                _selectedGesture = value;
                OnPropertyChanged(nameof(SelectedGesture));
            }
        }

        private int _numberOfBow;
        public int NumberOfBow
        {
            get { return _numberOfBow; }
            set
            {
                _numberOfBow = value;
                OnPropertyChanged(nameof(NumberOfBow));
            }
        }

        public GestureDatabaseViewModel(int numberOfBow)
        {
            IGestureDataService gestureDataService = new GestureDataService();
            GestureCollection = gestureDataService.GetAllGestures();
            if (GestureCollection.Count != 0)
            {
                SelectedGesture = GestureCollection[0];
                SelectedGesture.BowNumber = numberOfBow;
            }
            
            LoadCommands();

            Messenger.Default.Register<Gesture>(this, OnUpdateGestureListMessageReceived);
        }

        private void OnUpdateGestureListMessageReceived(Gesture obj)
        {
            GestureCollection = GestureDataService.GetAllGestures();
            DialogService.CloseDialog();
        }

        private void LoadCommands()
        {
            AddGestureCommand = new CustomCommand(AddGesture, CanAddGesture);
            ComputeCommand = new CustomCommand(ComputeBow, CanComputeBow);
        }

        private bool CanAddGesture(object obj)
        {
            return true;
        }

        private void AddGesture(object obj)
        {
            DialogService = new AddGestureDialogService();
            DialogService.ShowDialog();
        }

        private bool CanComputeBow(object obj)
        {
            return true;
        }

        private void ComputeBow(object obj)
        {
            var bow = BagOfVisualWords.Create(new BinarySplit(NumberOfBow));
            //BinarySplit binarySplit = new BinarySplit(NumberOfBow);

            //// Create bag-of-words (BoW) with the given algorithm
            //BagOfVisualWords surfBow = new BagOfVisualWords(binarySplit);

            //// Compute the BoW codebook using training images only
            //var bow = surfBow.Learn(GestureCollection.);
            var gestureTupleList = new List<Tuple<int, string, double[]>>();
            //var teacher = new SequentialMinimalOptimization<Linear>()
            //{
            //    Complexity = 10000 // make a hard margin SVM
            //};
            //var svm = teacher.Learn()

            foreach (var gesture in GestureCollection)
            {
                var gestureName = gesture.GestureName;
            }

            GestureCollection[0].FeatureList[0].ImageName
            foreach (var feature in FeatureCollection)
            {

                var image = (Bitmap)Image.FromFile(feature.ImageName);

                double[] featureVector = (_bow as ITransform<Bitmap, double[]>).Transform(image);
                feature.Vector = featureVector.ToString(DefaultArrayFormatProvider.InvariantCulture);
            }
        }
    }
}
