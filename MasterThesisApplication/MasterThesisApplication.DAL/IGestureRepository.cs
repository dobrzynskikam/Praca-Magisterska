using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterThesisApplication.Model;

namespace MasterThesisApplication.DAL
{
    public interface IGestureRepository
    {
        ObservableCollection<Gesture> GetGestures();
    }
}
