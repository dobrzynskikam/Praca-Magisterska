using MasterThesisApplication.Model;
using System.Collections.ObjectModel;

namespace MasterThesisApplication.DAL
{
    public interface IGestureRepository
    {
        ObservableCollection<Gesture> GetGestures();
    }
}
