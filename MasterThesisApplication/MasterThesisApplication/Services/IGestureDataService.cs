using System.Collections.Generic;
using MasterThesisApplication.Model;
using System.Collections.ObjectModel;
using System.Drawing;

namespace MasterThesisApplication.Services
{
    public interface IGestureDataService
    {
        ObservableCollection<Gesture> GetAllGestures();
        void AddNewGesture(Gesture gesture);
        Dictionary<string, Bitmap> GetAllImages();
        void SaveGestures(ObservableCollection<Gesture> gestures);
    }
}
