using Accord.Imaging;
using Accord.MachineLearning;
using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Services;
using MasterThesisApplication.Utility;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MasterThesisApplication.ViewModel
{
    public class GestureDatabaseViewModel: INotifyPropertyChanged
    {
        public ICommand AddGestureCommand { get; set; }
        public ICommand ComputeCommand { get; set; }
        public IDialogService DialogService;
        public IGestureDataService GestureDataService = new GestureDataService();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        

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
            NumberOfBow = numberOfBow;
            GestureCollection = GestureDataService.GetAllGestures();
            if (GestureCollection.Count != 0)
            {
                SelectedGesture = GestureCollection[0];
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
            return GestureCollection.Count != 0;
        }

        private void ComputeBow(object obj)
        {

            Accord.Math.Random.Generator.Seed = 0;

            // Create a new Bag-of-Visual-Words (BoW) model
            var bow = BagOfVisualWords.Create(new BinarySplit(NumberOfBow));

            // Since we are using generics, we can setup properties 
            // of the binary split clustering algorithm directly:
            bow.Clustering.ComputeProportions = true;
            bow.Clustering.ComputeCovariances = false;

            // Get some training images
            var images = GestureDataService.GetAllImages();

            
            // Compute the model
            bow.Learn(images.Values.ToArray());

            foreach (var gesture in GestureCollection)
            {
                foreach (var feature in gesture.FeatureList)
                {
                    var vector = bow.Transform(images[feature.ImageName]);
                    feature.Vector = string.Join(" ", vector.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                }
            }

            GestureDataService.SaveGestures(GestureCollection);
        }
    }
}
