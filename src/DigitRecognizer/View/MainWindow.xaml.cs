using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DigitRecognizer.ViewModel;

namespace DigitRecognizer.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            this.DataContext = _viewModel;
        }

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            _viewModel.StartLearning();
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            _viewModel.StopLearning();
        }

        private void Button_Update(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateParameters();
        }

        private void Logs_TextChanged(object sender, TextChangedEventArgs e)
        {
            Logs.PageDown();
        }

        private void Button_Recognize(object sender, RoutedEventArgs e)
        {
            _viewModel.Recognize();
        }

        private void Button_Next(object sender, RoutedEventArgs e)
        {
            _viewModel.NextImage();
        }

        private void Button_Prev(object sender, RoutedEventArgs e)
        {
            _viewModel.PrevImage();
        }

        private void Button_Reset(object sender, RoutedEventArgs e)
        {
            _viewModel.ResetWeights();
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveNetwork();
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            var image = sender as Image;
            var size = new Size(image.ActualWidth, image.ActualHeight);

            _viewModel.ImageMouseMove(e.LeftButton == MouseButtonState.Pressed, e.GetPosition(image), size);
        }

        private void Button_Clear(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearPaintImage();
        }

        private void Button_RecognizePaint(object sender, RoutedEventArgs e)
        {
            _viewModel.RecognizePaint();
        }

        private void Button_Load(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadNetwork();
        }

        private void Button_CreateDefault(object sender, RoutedEventArgs e)
        {
            _viewModel.CreateDefaultNetwork();
        }

        private void Button_CalcError(object sender, RoutedEventArgs e)
        {
            _viewModel.CalcTestSetError();
        }
    }
}
