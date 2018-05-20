using MasterThesisApplication.DAL;
using MasterThesisApplication.Model;
using System.Collections.ObjectModel;

namespace MasterThesisApplication.Services
{
    class CameraDataService : ICameraDataService
    {
        private readonly ICameraRepository _repository = new CameraRepository();
        public ObservableCollection<Camera> GetAllCameras()
        {
            return _repository.GetCameras();
        }
    }
}
