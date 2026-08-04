namespace MAUINavegacion;

public class BasePage : ContentPage
{
    public BasePage()
    {
        ToolbarItem carritoItem = new()
        {
            Text = "🛒",
            Order = ToolbarItemOrder.Primary,
            Priority = 0
        };

        carritoItem.Clicked += OnCarritoClicked;

        ToolbarItems.Add(carritoItem);
    }

    private async void OnCarritoClicked(
        object? sender,
        EventArgs e)
    {
        if (this is CarritoPage)
        {
            return;
        }

        await Navigation.PushAsync(
            new CarritoPage());
    }
}