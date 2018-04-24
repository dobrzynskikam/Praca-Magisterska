using GestureApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureApplication
{
    public class ViewModelLocator
    {
        private static GestureRecognitionViewModel gestureRecognitionViewModel = new GestureRecognitionViewModel();

        public static GestureRecognitionViewModel GestureRecognitionViewModel
        {
            get
            {
                return gestureRecognitionViewModel;
            }
        }
    }
}
