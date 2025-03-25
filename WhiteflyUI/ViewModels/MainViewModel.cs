using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhiteflyUI.Models;

namespace WhiteflyUI.ViewModels
{
    // Implementación sencilla de ICommand
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter) => _execute(parameter);
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        // Zoom
        private double _zoomFactor = 1.0;
        public double ZoomFactor
        {
            get => _zoomFactor;
            set { _zoomFactor = value; OnPropertyChanged(nameof(ZoomFactor)); }
        }
        // Propiedades para enlazar con la vista
        private double _scoreThreshold = 0.25;
        public double ScoreThreshold
        {
            get => _scoreThreshold;
            set { _scoreThreshold = value; OnPropertyChanged(nameof(ScoreThreshold)); }
        }

        private double _nmsThreshold = 0.45;
        public double NMSThreshold
        {
            get => _nmsThreshold;
            set { _nmsThreshold = value; OnPropertyChanged(nameof(NMSThreshold)); }
        }

        private int _maxBoxes = 100;
        public int MaxBoxes
        {
            get => _maxBoxes;
            set { _maxBoxes = value; OnPropertyChanged(nameof(MaxBoxes)); }
        }

        private ImageSource _originalImage;
        public ImageSource OriginalImage
        {
            get => _originalImage;
            set { _originalImage = value; OnPropertyChanged(nameof(OriginalImage)); }
        }

        // Nueva propiedad para la imagen procesada (devuelta por la API)
        private ImageSource _processedImage;
        public ImageSource ProcessedImage
        {
            get => _processedImage;
            set { _processedImage = value; OnPropertyChanged(nameof(ProcessedImage)); }
        }

        // Ruta de la imagen seleccionada
        private string selectedImagePath;

        // Comandos
        public ICommand SelectImageCommand { get; }
        public ICommand ProcessImageCommand { get; }

        // Constructor
        public MainViewModel()
        {
            SelectImageCommand = new RelayCommand(async _ => await SelectImageAsync());
            ProcessImageCommand = new RelayCommand(async _ => await ProcessImageAsync(), _ => !string.IsNullOrEmpty(selectedImagePath));
        }

        // Método para seleccionar imagen usando OpenFileDialog
        private async Task SelectImageAsync()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            bool? result = await Application.Current.Dispatcher.InvokeAsync(() => dlg.ShowDialog());
            if (result == true)
            {
                selectedImagePath = dlg.FileName;
                OriginalImage = new BitmapImage(new Uri(selectedImagePath));
            }
        }

        // Método para llamar a la API y obtener la imagen procesada
        private async Task ProcessImageAsync()
        {
            if (string.IsNullOrEmpty(selectedImagePath))
            {
                MessageBox.Show("Por favor, selecciona una imagen primero.");
                return;
            }

            try
            {
                using (var client = new HttpClient())
                {
                    // Ajusta la URL según la ubicación de tu API
                    var url = "http://localhost:8000/predict";

                    using (var form = new MultipartFormDataContent())
                    {
                        // Leer la imagen
                        byte[] fileBytes = File.ReadAllBytes(selectedImagePath);
                        var imageContent = new ByteArrayContent(fileBytes);
                        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                        form.Add(imageContent, "image", Path.GetFileName(selectedImagePath));

                        // Añadir parámetros
                        form.Add(new StringContent(ScoreThreshold.ToString(System.Globalization.CultureInfo.InvariantCulture)), "score_threshold");
                        form.Add(new StringContent(NMSThreshold.ToString(System.Globalization.CultureInfo.InvariantCulture)), "nms_threshold");
                        form.Add(new StringContent(MaxBoxes.ToString(System.Globalization.CultureInfo.InvariantCulture)), "max_boxes");

                        // Realizar la solicitud POST
                        var response = await client.PostAsync(url, form);
                        if (response.IsSuccessStatusCode)
                        {
                            // Leer la imagen procesada como stream
                            var stream = await response.Content.ReadAsStreamAsync();
                            var bmp = new BitmapImage();
                            bmp.BeginInit();
                            bmp.CacheOption = BitmapCacheOption.OnLoad;
                            bmp.StreamSource = stream;
                            bmp.EndInit();
                            bmp.Freeze(); // Para permitir el acceso desde otros hilos

                            // Actualizar la propiedad ProcessedImage en el hilo de la UI
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                ProcessedImage = bmp;
                            });
                        }
                        else
                        {
                            MessageBox.Show($"Error en la API: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar la imagen: " + ex.Message);
            }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
