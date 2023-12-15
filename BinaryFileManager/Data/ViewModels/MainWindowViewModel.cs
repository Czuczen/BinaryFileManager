using BinaryFileManager.Data.ViewModels.Shared;

namespace BinaryFileManager.Data.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    private BookViewModel _bookViewModel;

    public BookViewModel BookInfo
    {
        get => _bookViewModel;
        set
        {
            _bookViewModel = value;
            OnPropertyChanged(nameof(BookInfo));
        }
    }

    public MainWindowViewModel()
    {
        BookInfo = new BookViewModel();
    }
}