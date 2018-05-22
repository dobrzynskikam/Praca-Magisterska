using MasterThesisApplication.Model;
using System.Collections.ObjectModel;

namespace MasterThesisApplication.Services
{
    public interface IGestureDataService
    {
        ObservableCollection<Gesture> GetAllGestures();
        void AddNewGesture(Gesture gesture);
    }
}
