using System.Collections.Generic;
using MasterThesisApplication.Model;
using System.Collections.ObjectModel;
using System.Drawing;

namespace MasterThesisApplication.DAL
{
    public interface IGestureRepository
    {
        ObservableCollection<Gesture> GetGestures();
        void AddNewGesture(Gesture gesture);
        Dictionary<string, Bitmap> GetImages();
        void SaveGestures(ObservableCollection<Gesture> gestures);
    }
}
