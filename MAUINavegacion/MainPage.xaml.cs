using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion
{
    public partial class MainPage : ContentPage
    {
        private readonly MovieService _movieService;
        private bool _peliculasCargadas;

        public MainPage()
        {
            InitializeComponent();

            _movieService = new MovieService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_peliculasCargadas)
                return;

            try
            {
                Cargando.IsVisible = true;
                Cargando.IsRunning = true;
                ListaPeliculas.IsVisible = false;

                List<Pelicula> peliculas =
                    await _movieService.ObtenerPeliculasAsync();

                ListaPeliculas.ItemsSource = peliculas;
                _peliculasCargadas = true;
            }
            catch (Exception error)
            {
                await DisplayAlert(
                    "Error",
                    $"No se pudieron cargar las películas.\n\n{error.Message}",
                    "Aceptar");
            }
            finally
            {
                Cargando.IsVisible = false;
                Cargando.IsRunning = false;
                ListaPeliculas.IsVisible = true;
            }
        }

        private async void OnPeliculaSeleccionada(
            object sender,
            SelectionChangedEventArgs e)
        {
            Pelicula? peliculaSeleccionada =
                e.CurrentSelection.FirstOrDefault() as Pelicula;

            if (peliculaSeleccionada == null)
                return;

            // Permite seleccionar nuevamente la misma película.
            ((CollectionView)sender).SelectedItem = null;

            await Shell.Current.Navigation.PushAsync(
                new NewPage1(peliculaSeleccionada));
        }
    }
}