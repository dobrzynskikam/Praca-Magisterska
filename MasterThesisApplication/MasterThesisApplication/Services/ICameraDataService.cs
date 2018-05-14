using MasterThesisApplication.Model;
using System.Collections.ObjectModel;

namespace MasterThesisApplication.Services
{
    public interface ICameraDataService
    {
        ObservableCollection<Camera> GetAllCameras();
        //void StopCamera(Camera camera);
        //void StartCamera(Camera camera);
    }
}
