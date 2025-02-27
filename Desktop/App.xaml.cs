using System.Windows;

namespace Flavouru.Desktop
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем и показываем стартовое окно
            var startWindow = new StartWindow();
            startWindow.Show();

            // Закрываем MainWindow, если оно было создано автоматически
            if (Current.MainWindow != null && Current.MainWindow != startWindow)
            {
                Current.MainWindow.Close();
            }
        }
    }
}

