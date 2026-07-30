using MAUINavegacion.Models;
using MAUINavegacion.Services;

namespace MAUINavegacion
{
    public partial class PeliculasPage : ContentPage
    {
        private readonly MovieService _movieService;
        private bool _peliculasCargadas;

        public PeliculasPage()
        {
            InitializeComponent();

            _movieService = new MovieService();

            // Solamente guarda la edad si todavía no existe.
            if (!Preferences.Default.ContainsKey("Edad"))
            {
                Preferences.Default.Set("Edad", 20);
            }
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

        private async void OnPreferenciasClicked(
            object sender,
            EventArgs e)
        {
            int edad = Preferences.Default.Get("Edad", 0);

            await DisplayAlert(
                "Información",
                $"La edad guardada es: {edad}",
                "Aceptar");
        }

        private async void OnPeliculaSeleccionada(
            object sender,
            SelectionChangedEventArgs e)
        {
            Pelicula? peliculaSeleccionada =
                e.CurrentSelection.FirstOrDefault() as Pelicula;

            if (peliculaSeleccionada == null)
                return;

            ((CollectionView)sender).SelectedItem = null;

            await Shell.Current.Navigation.PushAsync(
                new DetallePeliculaPage(peliculaSeleccionada));
        }

    }
}