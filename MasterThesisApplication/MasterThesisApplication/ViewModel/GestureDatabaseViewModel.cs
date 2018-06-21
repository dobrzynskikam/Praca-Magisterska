using Accord.Imaging;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Model.Utility;
using MasterThesisApplication.Services;
using MasterThesisApplication.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Image = Accord.Imaging.Image;

namespace MasterThesisApplication.ViewModel
{
    public class GestureDatabaseViewModel: INotifyPropertyChanged
    {
        #region Commands
        public ICommand AddGestureCommand { get; set; }
        public ICommand ComputeCommand { get; set; }
        public ICommand StartTrainingCommand { get; set; }
        public ICommand ClassifyCommand { get; set; }
        public ICommand AddTestImageCommand { get; set; }
        public ICommand ClassifyTestImageCommand { get; set; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IDialogService DialogService;
        public IGestureDataService GestureDataService = new GestureDataService();

        Dictionary<string, Tuple<int, double[]>> VectorLabelDictionary { get; set; }
        private Dictionary<string, Tuple<int, double[]>> _vectorLabelDictionary = new Dictionary<string, Tuple<int, double[]>>();


        private IBagOfWords<Bitmap> _bow;

        public MulticlassSupportVectorMachine<IKernel> Machine { get; set; }

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

        

        private string _testedImageLabel;
        public string TestedImageLabel
        {
            get { return _testedImageLabel; }
            set
            {
                _testedImageLabel = value;
                OnPropertyChanged(nameof(TestedImageLabel));
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
        private float _complexity;

        public float Complexity
        {
            get { return _complexity; }
            set
            {
                _complexity = value;
                OnPropertyChanged(nameof(Complexity));
                UpdateSvmModel();
            }
        }
        private float _tolerance;
        public float Tolerance
        {
            get { return _tolerance; }
            set
            {
                _tolerance = value;
                OnPropertyChanged(nameof(Tolerance));
                UpdateSvmModel();
            }
        }

        private int _degree;
        public int Degree
        {
            get { return _degree; }
            set
            {
                _degree = value;
                OnPropertyChanged(nameof(Degree));
                UpdateSvmModel();
            }
        }

        private int _constant;
        public int Constant
        {
            get { return _constant; }
            set
            {
                _constant = value;
                OnPropertyChanged(nameof(Constant));
                UpdateSvmModel();
            }
        }

        private int _sigma;
        public int Sigma
        {
            get { return _sigma; }
            set
            {
                _sigma = value;
                OnPropertyChanged(nameof(Sigma));
                UpdateSvmModel();
            }
        }

        private IKernel _kernel;
        private MulticlassSupportVectorLearning<IKernel> _svm;

        private void UpdateSvmModel()
        {
            _svm = new MulticlassSupportVectorLearning<IKernel>()
            {
                Kernel = _kernel,
                Learner = (param) => new SequentialMinimalOptimization<IKernel>()
                {
                    Kernel = _kernel,
                    Complexity = Complexity,
                    Tolerance = Tolerance,
                }
            };
        }

        private int _isSuccess;

        public int IsSuccess
        {
            get { return _isSuccess; }
            set
            {
                _isSuccess = value;
                switch (_isSuccess)
                {
                    case 1:
                        _kernel = new Linear(Constant);
                        UpdateSvmModel();
                        break;
                    case 2:
                        _kernel = new Gaussian(Sigma);
                        UpdateSvmModel();
                        break;

                }
                OnPropertyChanged(nameof(IsSuccess));
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



        //Constructor
        public GestureDatabaseViewModel()
        {
            //Set default parameters for SVM
            NumberOfBow = 36;
            Complexity = 100;
            Tolerance = 0.0001f;
            IsSuccess = 1;
            Degree = 1;
            Constant = 1;
            Sigma = 1;

            GestureCollection = GestureDataService.GetAllGestures();
            if (GestureCollection.Count != 0)
            {
                SelectedGesture = GestureCollection[0];
                if (GestureCollection.All(g=>g.FeatureList.All(f=>f.Vector!=null)))
                {
                    foreach (var gesture in GestureCollection)
                    {
                        foreach (var feature in gesture.FeatureList)
                        {
                            var tempArray = feature.Vector.Split(' ').ToList().Select(s => double.Parse(s)).ToArray();
                            _vectorLabelDictionary.Add(feature.ImageName, new Tuple<int, double[]>(gesture.Label, tempArray));
                        }


                        VectorLabelDictionary = _vectorLabelDictionary;
                    }
                }
            }

            LoadCommands();

            Messenger.Default.Register<Gesture>(GestureCollection, OnUpdateGestureListMessageReceived);
            Messenger.Default.Register<BitmapImage>(TestImage, OnCameraImageReceived);
        }

        private void LoadCommands()
        {
            AddGestureCommand = new CustomCommand(AddGesture, CanAddGesture);
            ComputeCommand = new CustomCommand(ComputeBow, CanComputeBow);
            StartTrainingCommand = new CustomCommand(StartTraining, CanStartTraining);
            ClassifyCommand = new CustomCommand(Classify, CanClassify);
            AddTestImageCommand = new CustomCommand(AddTestImage, null);
            ClassifyTestImageCommand = new CustomCommand(ClassifyTestImage, CanClassifyTestImage);
        }

        private void OnCameraImageReceived(BitmapImage cameraImage)
        {
            TestImage = cameraImage;
            var testVector = (_bow as ITransform<Bitmap, double[]>).Transform(TestImage.BitmapImage2Bitmap());
            var result = Machine.Decide(testVector);
            var resultLabel = GestureCollection.First(g => g.Label == result).GestureName;
            Messenger.Default.Send(resultLabel);
        }

        private void OnUpdateGestureListMessageReceived(Gesture obj)
        {
            VectorLabelDictionary = null;
            _vectorLabelDictionary = null;
            GestureCollection = GestureDataService.GetAllGestures();
            DialogService.CloseDialog();
        }

        private bool CanClassifyTestImage(object obj)
        {
            return TestImage != null;
        }

        private void ClassifyTestImage(object obj)
        {
            var testVector = (_bow as ITransform<Bitmap, double[]>).Transform(TestImage.BitmapImage2Bitmap());
            var result = Machine.Decide(testVector);
            TestedImageLabel = GestureCollection.First(g => g.Label == result).GestureName;
            Messenger.Default.Send(TestedImageLabel);
        }

        private void AddTestImage(object obj)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            openFile.InitialDirectory = Environment.CurrentDirectory;
            if (openFile.ShowDialog() == true)
            {
                TestImage = Image.FromFile(openFile.FileName).ToBitmapImage();
            }
        }

        private bool CanClassify(object obj)
        {
            return Machine != null && CanStartTraining(null);
        }

        private void Classify(object obj)
        {
            var sw1 = Stopwatch.StartNew();

            foreach (var record in VectorLabelDictionary)
            {
                var input = record.Value.Item2;
                int expected = record.Value.Item1;

                int actual = Machine.Decide(input);

                if (expected == actual)
                {
                    
                }
            }

            var allFeatures = 0f;
            var positiveHits = 0f;
            foreach (var gesture in GestureCollection)
            {
                var expectedLabel = gesture.Label;
                foreach (var feature in gesture.FeatureList)
                {
                    var vector = feature.Vector.Split(' ').Select(x => double.Parse(x)).ToArray();
                    var actualLabel = Machine.Decide(vector);
                    if (actualLabel == expectedLabel)
                    {
                        //feature.IsClassifiedCorrectly = true;
                        feature.State = FeatureState.CorrectClassification;
                        positiveHits++;
                    }
                    else
                    {
                        //feature.IsClassifiedCorrectly = false;
                        feature.State = FeatureState.IncorrectClassification;
                    }

                    allFeatures++;
                }
            }

            sw1.Stop();

            StatusText = "CLASSIFICATION OF ALL TRAINING IMAGES" +
                         "\nIt took " + sw1.Elapsed.TotalSeconds.ToString("F") + ".s" +
                         "Accuracy: " + positiveHits/allFeatures *100 + "%";
        }

        private bool CanStartTraining(object obj)
        {
            return VectorLabelDictionary != null;
        }

        private void StartTraining(object obj)
        {
            var sw1 = Stopwatch.StartNew();

            var inputs = VectorLabelDictionary.Values.Select(x => x.Item2).ToArray();
            var outputs = VectorLabelDictionary.Values.Select(x => x.Item1).ToArray();
            Machine = _svm.Learn(inputs, outputs);

            sw1.Stop();

            StatusText = "TRAINING SVM MODEL" +
                         "\nIt took " + sw1.Elapsed.TotalSeconds.ToString("F") + ".s";
        }

        private bool CanAddGesture(object obj)
        {
            return true;
        }

        private void AddGesture(object obj)
        {
            Messenger.Default.Send(GestureCollection.ToList());
            DialogService = new AddGestureDialogService();
            DialogService.ShowDialog();
        }

        private bool CanComputeBow(object obj)
        {
            return GestureCollection.Count != 0;
        }

        private void ComputeBow(object obj)
        {
           //StatusText = "Feature extraction for all images. It may take significant amount of time.";
            StatusText = "Feature";

            var sw1 = Stopwatch.StartNew();

            Accord.Math.Random.Generator.Seed = 0;

            // Create a Binary-Split clustering algorithm
            BinarySplit binarySplit = new BinarySplit(NumberOfBow);

            // Create bag-of-words (BoW) with the given algorithm
            BagOfVisualWords surfBow = new BagOfVisualWords(binarySplit);

            // Get some training images
            var images = GestureDataService.GetAllImages();
            
            // Compute the model
            _bow = surfBow.Learn(images.Values.ToArray());

            sw1.Stop();

            Stopwatch sw2 = Stopwatch.StartNew();

            _vectorLabelDictionary = new Dictionary<string, Tuple<int, double[]>>();
            //_vectorLabelDictionary = new Dictionary<string, Tuple<int, double[]>>();
            foreach (var gesture in GestureCollection)
            {
                foreach (var feature in gesture.FeatureList)
                {
                    var vector = (_bow as ITransform<Bitmap, double[]>).Transform(images[feature.ImageName]);
                    feature.Vector = string.Join(" ", vector.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                    _vectorLabelDictionary.Add(feature.ImageName, new Tuple<int, double[]>(gesture.Label, vector));
                }
            }

            VectorLabelDictionary = _vectorLabelDictionary;
            GestureDataService.SaveGestures(GestureCollection);

            sw2.Stop();

            StatusText = "CREATING DESCRIPTORS FOR ALL IMAGES" + 
                         "Clustering took " + sw1.Elapsed.TotalSeconds.ToString("F") + "s." +
                         "\nFeatures extracted in " + sw2.Elapsed.TotalSeconds.ToString("F") + "s.";
        }
    }
}
