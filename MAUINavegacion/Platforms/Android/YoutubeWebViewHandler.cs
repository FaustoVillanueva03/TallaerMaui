using Android.Webkit;
using Microsoft.Maui.Handlers;

namespace MAUINavegacion;

public class YoutubeWebViewHandler : WebViewHandler
{
    protected override void ConnectHandler(
        Android.Webkit.WebView platformView)
    {
        base.ConnectHandler(platformView);

        platformView.Settings.JavaScriptEnabled = true;
        platformView.Settings.DomStorageEnabled = true;
        platformView.Settings.MediaPlaybackRequiresUserGesture = true;

        platformView.SetWebViewClient(
            new YoutubeWebViewClient());
    }
}

public class YoutubeWebViewClient : WebViewClient
{
    public override void OnPageFinished(
        Android.Webkit.WebView? view,
        string? url)
    {
        base.OnPageFinished(view, url);
    }
}