using Accord;
using MasterThesisApplication.Model;
using MasterThesisApplication.ViewModel;

namespace MasterThesisApplication
{
    public class ViewModelLocator
    {
        public static GestureRecognitionViewModel GestureRecognitionViewModel { get; } = new GestureRecognitionViewModel();
        public HslFilterViewModel HslFilterViewModel { get; } = new HslFilterViewModel(331, 40, 0.11f, 0.62f, 0.11f, 0.82f);
    }
}
