using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Services;
using MasterThesisApplication.Utility;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MasterThesisApplication.ViewModel
{
    public class GestureDatabaseViewModel: INotifyPropertyChanged
    {

        public ICommand AddGestureCommand { get; set; }
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

        private ObservableCollection<Feature> _featureCollection = new ObservableCollection<Feature>();
        public ObservableCollection<Feature> FeatureCollection
        {
            get { return _featureCollection; }
            set
            { 
                _featureCollection = value;
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

        public GestureDatabaseViewModel(int numberOfBow)
        {
            //NumberOfBow = numberOfBow;
            IGestureDataService gestureDataService = new GestureDataService();
            GestureCollection = gestureDataService.GetAllGestures();
            SelectedGesture = GestureCollection[1];
            SelectedGesture.BowNumber = numberOfBow;
            LoadCommands();
        }

        private void LoadCommands()
        {
            AddGestureCommand = new CustomCommand(AddGesture, CanAddGesture);
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
    }
}
