using System.Windows;
using WeatherApp.ViewModels;

namespace WeatherApp;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}