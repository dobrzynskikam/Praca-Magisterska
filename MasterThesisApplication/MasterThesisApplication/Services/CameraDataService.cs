using System;
using MasterThesisApplication.DAL;
using MasterThesisApplication.Model;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using Accord.Video;
using Accord.Video.DirectShow;
using MasterThesisApplication.Utility;

namespace MasterThesisApplication.Services
{
    class CameraDataService : ICameraDataService
    {
        private readonly ICameraRepository _repository = new CameraRepository();
        public ObservableCollection<Camera> GetAllCameras()
        {
            return _repository.GetCameras();
        }

        //public void StopCamera(Camera camera)
        //{
        //    _repository.StopCamera(camera);
        //}

        //public void StartCamera(Camera camera)
        //{
        //    _repository.StartCamera(camera);
        //}
    }
}
