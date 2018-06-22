using System;
using MasterThesisApplication.DAL;
using MasterThesisApplication.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace MasterThesisApplication.Services
{
    public class GestureDataService : IGestureDataService
    {
        private readonly IGestureRepository _repository = new GestureRepository();
        public ObservableCollection<Gesture> GetAllGestures()
        {
            return _repository.GetGestures();
        }

        public void AddNewGesture(Gesture gesture)
        {
            _repository.AddNewGesture(gesture);
        }

        public Dictionary<string, Tuple<double[], Bitmap>> GetAllImages()
        {
            return _repository.GetImages();
        }

        public void SaveGestures(ObservableCollection<Gesture> gestures)
        {
            _repository.SaveGestures(gestures);
        }
    }
}
