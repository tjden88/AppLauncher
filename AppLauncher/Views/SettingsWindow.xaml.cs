using System.Windows;

namespace AppLauncher.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = App.SettingsWindowViewModel;
        }
    }
}
