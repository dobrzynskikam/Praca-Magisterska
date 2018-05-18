using MasterThesisApplication.Model;
using System.Collections.ObjectModel;

namespace MasterThesisApplication.Services
{
    public interface ICameraDataService
    {
        ObservableCollection<Camera> GetAllCameras();
    }
}
