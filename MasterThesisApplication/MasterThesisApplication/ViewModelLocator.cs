using Accord;
using MasterThesisApplication.ViewModel;

namespace MasterThesisApplication
{
    public class ViewModelLocator
    {
        public static GestureRecognitionViewModel GestureRecognitionViewModel { get; } = new GestureRecognitionViewModel();
        public static HslFilterViewModel HslFilterViewModel { get; } = new HslFilterViewModel(11, 25, 0.3f, 0.8f, 0.5f, 0.7f);
    }
}
