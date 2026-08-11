namespace MAUINavegacion;

public class YoutubeWebView : WebView
{
    public static readonly BindableProperty VideoIdProperty =
        BindableProperty.Create(
            nameof(VideoId),
            typeof(string),
            typeof(YoutubeWebView),
            string.Empty);

    public string VideoId
    {
        get => (string)GetValue(VideoIdProperty);
        set => SetValue(VideoIdProperty, value);
    }
}