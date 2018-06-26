using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Accord.Imaging;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using MasterThesisApplication.Model.Annotations;
using Image = System.Drawing.Image;

namespace MasterThesisApplication.Model
{
    public class Classifier : INotifyPropertyChanged
    {
        private static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string DatabasePath = Path.Combine(AssemblyPath.Replace("MasterThesisApplication\\bin\\Debug", ""), "GestureDatabase");

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int NumberOfBow { get; set; }
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

        private float _sigma;
        public float Sigma
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
            if (IsSuccess == 1)
                _kernel = Degree == 1 ? (IKernel) new Linear(Constant) : new Polynomial(Degree, Constant);
            else if (IsSuccess == 2)
            {
                _kernel = new Gaussian(Sigma);
            }

            _svm = new MulticlassSupportVectorLearning<IKernel>()
            {
                Kernel = _kernel,
                Learner = (param) => new SequentialMinimalOptimization<IKernel>()
                {
                    Kernel = _kernel,
                    //Complexity = Complexity,
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
                UpdateSvmModel();
                OnPropertyChanged(nameof(IsSuccess));
            }
        }

        private IBagOfWords<Bitmap> _bow;
        private MulticlassSupportVectorMachine<IKernel> _machine;

        public Classifier()
        {

        }
        public Classifier(int numberOfBow, int complexity, float tolerance, int isSuccess, int degree, int constant, int sigma)
        {
            NumberOfBow = numberOfBow;
            Complexity = complexity;
            Tolerance = tolerance;
            IsSuccess = isSuccess;
            Degree = degree;
            Constant = constant;
            Sigma = sigma;
        }

        public void ComputeBow(ObservableCollection<Gesture> gestureCollection)
        {
            var imageArray = gestureCollection.SelectMany(g => g.FeatureList.Select(f=>(Bitmap)Image.FromFile(Path.Combine(DatabasePath, g.GestureName, f.ImageName)))).ToArray();

            Accord.Math.Random.Generator.Seed = 0;

            // Create a Binary-Split clustering algorithm
            BinarySplit binarySplit = new BinarySplit(NumberOfBow);

            // Create bag-of-words (BoW) with the given algorithm
            BagOfVisualWords surfBow = new BagOfVisualWords(binarySplit);
            _bow = surfBow.Learn(imageArray);
        }

        public void CreateDescriptors(Gesture gesture, bool gestureInDatabase)
        {
            foreach (var feature in gesture.FeatureList)
            {
                //Differences in path between gestures in database and tested images.
                feature.Vector = CreateDescriptor(gestureInDatabase ? Path.Combine(DatabasePath, gesture.GestureName, feature.ImageName) : Path.Combine(feature.ImageName));
            }
        }

        public double[] CreateDescriptor(string imagePath)
        {
            return CreateDescriptor((Bitmap) Image.FromFile(imagePath));
            //return (_bow as ITransform<Bitmap, double[]>).Transform((Bitmap)Image.FromFile(imagePath));
        }

        public double[] CreateDescriptor(Bitmap image)
        {
            return (_bow as ITransform<Bitmap, double[]>).Transform(image);
        }


        public bool CanTrain()
        {
            return _bow != null;
        }

        public void Train(ObservableCollection<Gesture> gestureCollection)
        {
            var inputs = new List<double[]>();
            var outputs = new List<int>();
            foreach (var gesture in gestureCollection)
            {
                foreach (var feature in gesture.FeatureList)
                {
                   inputs.Add(feature.Vector);
                   outputs.Add(gesture.Label);
                }
                
            }

            try
            {
                _machine = _svm.Learn(inputs.ToArray(), outputs.ToArray());
            }
            catch (Exception e)
            {
            }
            

        }

        public bool CanClassify()
        {
            return CanTrain() && _machine != null;
        }

        public void Classify(Gesture gesture)
        {
            Classify(gesture, gesture.Label);
        }

        public void Classify(Gesture gesture, int expectedResult)
        {
            foreach (var feature in gesture.FeatureList)
            {
                var result = _machine.Decide(feature.Vector);
                feature.State = expectedResult == result ? FeatureState.CorrectClassification : FeatureState.IncorrectClassification;
            }
        }

        public int Classify(double[] descriptor)
        {
            return _machine.Decide(descriptor);
        }
    }
}
