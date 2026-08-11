#if WINDOWS

using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace MAUINavegacion;

public class YoutubeWebViewHandlerWindows : WebViewHandler
{
    protected override async void ConnectHandler(
        WebView2 platformView)
    {
        base.ConnectHandler(platformView);

        try
        {
            await platformView.EnsureCoreWebView2Async();

            platformView.CoreWebView2
                .AddWebResourceRequestedFilter(
                    "https://www.youtube.com/*",
                    CoreWebView2WebResourceContext.All);

            platformView.CoreWebView2
                .WebResourceRequested +=
                OnWebResourceRequested;
        }
        catch
        {
            // Si WebView2 todavía no está listo,
            // dejamos que MAUI siga funcionando.
        }
    }

    private void OnWebResourceRequested(
        CoreWebView2 sender,
        CoreWebView2WebResourceRequestedEventArgs args)
    {
        try
        {
            args.Request.Headers.SetHeader(
                "Referer",
                "https://com.fausto.movieapp");
        }
        catch
        {
        }
    }

    protected override void DisconnectHandler(
        WebView2 platformView)
    {
        if (platformView.CoreWebView2 != null)
        {
            platformView.CoreWebView2
                .WebResourceRequested -=
                OnWebResourceRequested;
        }

        base.DisconnectHandler(platformView);
    }
}

#endif