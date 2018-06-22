﻿using MasterThesisApplication.Model.Annotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

        public string GestureName
        {
            get { return _gestureName; }
            set
            {
                _gestureName = value;
                OnPropertyChanged(nameof(GestureName));
            }
        }

        private int _bowNumber;
        public int BowNumber
        {
            get { return _bowNumber; }
            set
            {
                _bowNumber = value;
                OnPropertyChanged(nameof(BowNumber));
            }
        }

        private int _label;

        public int Label
        {
            get
            {
                return _label;
            }
            set
            {
                _label = value;
                OnPropertyChanged(nameof(Label));
            }

        }

        private ObservableCollection<Feature> _featureList;

        public ObservableCollection<Feature> FeatureList
        {
            get { return _featureList; }
            set
            {
                _featureList = value;
                OnPropertyChanged(nameof(FeatureList));
            }
        }
    }
        
    public class Feature : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private double[] _vector;
        public double[] Vector
        {
            get { return _vector; }
            set
            {
                _vector = value;
                OnPropertyChanged(nameof(Vector));
            }
        }

        private string _imageName;
        public string ImageName
        {
            get { return _imageName; }
            set
            {
                _imageName = value;
                OnPropertyChanged(nameof(ImageName));
            }
        }

        private bool _isClassifiedCorrectly;

        public bool IsClassifiedCorrectly
        {
            get { return _isClassifiedCorrectly; }
            set
            {
                _isClassifiedCorrectly = value;
                OnPropertyChanged(nameof(IsClassifiedCorrectly));
            }
        }

        private FeatureState _state;

        public FeatureState State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged(nameof(State));
            }
        }
    }
    public enum FeatureState
    {
        BeforeClassification,
        CorrectClassification,
        IncorrectClassification
    }
}
