using MAUINavegacion.Models;

namespace MAUINavegacion
{
    public partial class NewPage1 : ContentPage
    {
        public NewPage1()
        {
            InitializeComponent();
        }

        public NewPage1(Pelicula pelicula) : this()
        {
            BindingContext = pelicula;
        }

        private async void OnVolverClicked(
            object sender,
            EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }
    }
}