using MasterThesisApplication.Model;
using System.Collections.ObjectModel;

namespace MasterThesisApplication.DAL
{
    public interface ICameraRepository
    {
        ObservableCollection<Camera> GetCameras();
        //void StopCamera(Camera camera);
        //void StartCamera(Camera camera);
    }
}
