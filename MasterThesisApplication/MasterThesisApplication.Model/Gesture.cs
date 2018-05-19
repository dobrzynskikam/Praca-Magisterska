using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MasterThesisApplication.Model.Annotations;

namespace MasterThesisApplication.Model
{
    public class Gesture :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _gestureName;
        private int _bowNumber;
        private ObservableCollection<Feature> _featureList;

        public string GestureName
        {
            get { return _gestureName; }
            set
            {
                _gestureName = value;
                OnPropertyChanged(nameof(GestureName));
            }
        }

        public ObservableCollection<Feature> FeatureList
        {
            get { return _featureList; }
            set
            {
                _featureList = value;
                OnPropertyChanged(nameof(FeatureList));
            }
        }

        //public List<Feature> FeatureList { get; set; }
    }
        
    public class Feature : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _vector;
        public string Vector
        {
            get { return _vector; }
            set
            {
                _vector = value;
                OnPropertyChanged(nameof(Vector));
            }
        }
    }
}
