using BinaryFileManager.Data.ViewModels;
using Microsoft.Win32;
using System.Windows;

namespace BinaryFileManager;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel.BookInfo;
    }

    private void SelectFile_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Binary files (*.bin)|*.bin";

        if (openFileDialog.ShowDialog() == true)
            _viewModel.BookInfo.LoadBookBinaryFile(openFileDialog.FileName);
    }
}