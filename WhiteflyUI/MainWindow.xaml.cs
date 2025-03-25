using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WhiteflyUI.ViewModels;

namespace WhiteflyUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _start;
        private Point _origin;
        public MainWindow()
        {
            InitializeComponent();
            // Asignar el ViewModel como DataContext de la ventana
            DataContext = new MainViewModel();
        }
        private void ZoomableImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Capturamos el mouse y almacenamos la posición inicial
            if (!ZoomableImage.IsMouseCaptured)
            {
                _start = e.GetPosition(ImageScrollViewer);
                // Obtener la posición actual del TranslateTransform
                _origin = new Point(translateTransform.X, translateTransform.Y);
                ZoomableImage.CaptureMouse();
            }
        }

        private void ZoomableImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (ZoomableImage.IsMouseCaptured)
            {
                // Calcula el vector de desplazamiento
                Vector v = _start - e.GetPosition(ImageScrollViewer);
                translateTransform.X = _origin.X - v.X;
                translateTransform.Y = _origin.Y - v.Y;
            }
        }

        private void ZoomableImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ZoomableImage.ReleaseMouseCapture();
        }
    }
}