namespace MAUINavegacion;

public partial class MapaPage : ContentPage
{
    public MapaPage()
    {
        InitializeComponent();
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}