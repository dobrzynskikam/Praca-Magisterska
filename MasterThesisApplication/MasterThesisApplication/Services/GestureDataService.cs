using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterThesisApplication.DAL;
using MasterThesisApplication.Model;

namespace MasterThesisApplication.Services
{
    public class GestureDataService : IGestureDataService
    {
        private readonly IGestureRepository _repository = new GestureRepository();
        public ObservableCollection<Gesture> GetAllGestures()
        {
            return _repository.GetGestures();
        }
    }
}
