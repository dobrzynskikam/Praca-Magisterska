using MasterThesisApplication.ViewModel;

namespace MasterThesisApplication
{
    public class ViewModelLocator
    {
        public static GestureRecognitionViewModel GestureRecognitionViewModel { get; } = new GestureRecognitionViewModel();
        public static HslFilterViewModel HslFilterViewModel { get; } = new HslFilterViewModel(331, 40, 0.11f, 0.62f, 0.11f, 0.82f);
        public GestureDatabaseViewModel GestureDatabaseViewModel { get; } = new GestureDatabaseViewModel(36);
        public AddGestureViewModel AddGestureViewModel { get; } = new AddGestureViewModel(36);
    }
}
