using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Services;
using MasterThesisApplication.Utility;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MasterThesisApplication.Model.Utility;

namespace MasterThesisApplication.ViewModel
{
    public class GestureDatabaseViewModel: INotifyPropertyChanged
    {
        #region Commands
        public ICommand AddGestureCommand { get; set; }
        public ICommand ComputeCommand { get; set; }
        public ICommand StartTrainingCommand { get; set; }
        public ICommand ClassifyCommand { get; set; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IDialogService DialogService;
        public IGestureDataService GestureDataService = new GestureDataService();

        private ObservableCollection<Gesture> _gestureCollection;
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

        private BitmapImage _testImage;
        public BitmapImage TestImage
        {
            get
            {
                Messenger.Default.Register<BitmapImage>(this, OnCameraImageReceived);
                return _testImage;
            }
            set
            {
                _testImage = value;
                OnPropertyChanged(nameof(TestImage));
                
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


        #region SVM parameters

        private Classifier _svm;
        public Classifier Svm
        {
            get { return _svm; }
            set
            {
                _svm = value;
                OnPropertyChanged(nameof(Svm));
            }
        }
        #endregion

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



        #region Constructor
        public GestureDatabaseViewModel()
        {
            //Set default parameters for SVM
            Svm = new Classifier(36, 100, 0.01f, 1, 1, 1, 1);

            GestureCollection = GestureDataService.GetAllGestures();
            LoadCommands();

            Messenger.Default.Register<Gesture>(GestureCollection, OnUpdateGestureListMessageReceived);
            Messenger.Default.Register<BitmapImage>(TestImage, OnCameraImageReceived);
        }

        #endregion


        private void LoadCommands()
        {
            AddGestureCommand = new CustomCommand(AddGesture, CanAddGesture);
            ComputeCommand = new CustomCommand(ComputeBow, CanComputeBow);
            StartTrainingCommand = new CustomCommand(StartTraining, CanStartTraining);
            ClassifyCommand = new CustomCommand(Classify, CanClassify);
        }

        private void OnCameraImageReceived(BitmapImage cameraImage)
        {
            var testImageDescriptor = Svm.CreateDescriptor(cameraImage.BitmapImage2Bitmap());
            var result = Svm.Classify(testImageDescriptor);
            var resultLabel = GestureCollection.First(g => g.Label == result).GestureName;
            Messenger.Default.Send(resultLabel);
        }

        private void OnUpdateGestureListMessageReceived(Gesture obj)
        {
            GestureCollection = GestureDataService.GetAllGestures();
            DialogService.CloseDialog();
        }

        private bool CanAddGesture(object obj)
        {
            return true;
        }

        private void AddGesture(object obj)
        {
            var data = GestureCollection.ToDictionary(g => g.Label, g => g.GestureName);
            Messenger.Default.Send(data);
            Messenger.Default.Send(_svm);
            DialogService = new AddGestureDialogService();
            DialogService.ShowDialog();
        }

        private void ComputeBow(object obj)
        {
            var sw1 = Stopwatch.StartNew();

            Svm.ComputeBow(GestureCollection);

            sw1.Stop();


            var sw2 = Stopwatch.StartNew();

            foreach (var gesture in GestureCollection)
            {
                Svm.CreateDescriptors(gesture, true);
            }

            sw2.Stop();

            StatusText = "CREATING DESCRIPTORS" +
                         "\nClustering took " + sw1.Elapsed.TotalSeconds.ToString("F") + "s." +
                         "\nFeatures extracted in " + sw2.Elapsed.TotalSeconds.ToString("F") + "s.";
        }

        private bool CanComputeBow(object obj)
        {
            return GestureCollection.Count != 0 && Svm.NumberOfBow > 1;
        }

        private bool CanStartTraining(object obj)
        {
            return GestureCollection.Count > 1 && GestureCollection.All(g => g.FeatureList.All(f => f.Vector != null)) && _svm.CanTrain();
        }

        private void StartTraining(object obj)
        {
            var sw1 = Stopwatch.StartNew();
            Svm.Train(GestureCollection);
            sw1.Stop();

            StatusText = "TRAINING SVM MODEL" +
                         "\nIt took " + sw1.Elapsed.TotalSeconds.ToString("F") + ".s";
        }

        private bool CanClassify(object obj)
        {
            return CanStartTraining(null) && _svm.CanClassify();
        }

        private void Classify(object obj)
        {
            var sw1 = Stopwatch.StartNew();

            foreach (var gesture in GestureCollection)
            {
                Svm.Classify(gesture);
            }

            float positiveHits = GestureCollection.Sum(g =>
                g.FeatureList.Count(f => f.State == FeatureState.CorrectClassification));

            float allFeatures = GestureCollection.Sum(g=>g.FeatureList.Count);

            sw1.Stop();


            StatusText = "CLASSIFICATION" +
                         "\nIt took " + sw1.Elapsed.TotalSeconds.ToString("F") + ".s" +
                         $"\nPositiveHits: {positiveHits}; Total: {allFeatures}" + 
                         "\nAccuracy: " +  (positiveHits/allFeatures *100).ToString("F") + "%";
        }
    }
}
