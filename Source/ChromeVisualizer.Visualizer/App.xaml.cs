namespace ChromeVisualizer.Visualizer
{
    using CefSharp;

    public partial class App
    {
        public App()
        {
            Cef.Initialize(new CefSettings());
        }
    }
}
