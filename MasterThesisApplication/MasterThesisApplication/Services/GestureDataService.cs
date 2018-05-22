﻿using MasterThesisApplication.DAL;
using MasterThesisApplication.Model;
using System.Collections.ObjectModel;

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
    }
}