using MasterThesisApplication.Model;
using MasterThesisApplication.Model.Annotations;
using MasterThesisApplication.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MasterThesisApplication.ViewModel
{
    public class GestureDatabaseViewModel: INotifyPropertyChanged
    {
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

        public GestureDatabaseViewModel()
        {
            IGestureDataService gestureDataService = new GestureDataService();
            GestureCollection = gestureDataService.GetAllGestures();
            SelectedGesture = GestureCollection[1];
        }
    }
}
