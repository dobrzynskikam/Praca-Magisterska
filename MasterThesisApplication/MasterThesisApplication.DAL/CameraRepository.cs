using Accord.Video.DirectShow;
using MasterThesisApplication.Model;
using System.Collections.ObjectModel;

namespace MasterThesisApplication.DAL
{
    public class CameraRepository : ICameraRepository
    {
        private ObservableCollection<Camera> _cameras;

        public ObservableCollection<Camera> GetCameras()
        {
            if (_cameras == null)
            {
                GetVideoDevices();
            }

            return _cameras;
        }

        private void GetVideoDevices()
        {
            _cameras = new ObservableCollection<Camera>();
            foreach (var device in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                _cameras.Add(new Camera() { Name = device.Name, MonikerString = device.MonikerString});
            }
        }
    }
}
