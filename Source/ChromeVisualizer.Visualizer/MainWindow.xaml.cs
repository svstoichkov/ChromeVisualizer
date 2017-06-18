namespace ChromeVisualizer.Visualizer
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;

    using CefSharp;

    public partial class MainWindow
    {
        string mHtmlFile;

        /// <summary>
        /// Window constructor
        /// </summary>
        public MainWindow()
        {
            // Load UI
            this.InitializeComponent();

            // Listen for CefSharp browser events
            this.WebBrowser.LoadingStateChanged += this.BrowserLoadingStateChanged;
            this.WebBrowser.IsBrowserInitializedChanged += this.BrowserInitialized;
        }

        /// <summary>
        /// Invoked when the browser starts or finishes loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            // Done loading?
            if (!e.IsLoading)
            {
                // Find a better way to modify the UI than to invoke dispatcher
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                                                                                     {
                                                                                                         // Find the loading fade out Storyboard
                                                                                                         Storyboard fadeOut = (Storyboard) this.FindResource("FadeOutLoading");

                                                                                                         // Fade out the loading image
                                                                                                         fadeOut.Begin();
                                                                                                     }));
            }
        }

        /// <summary>
        /// Invoked when the browser has been loaded and is ready for injection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BrowserInitialized(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Load the HTML file from args
            this.LoadHTMLFile();
        }

        /// <summary>
        /// Extracts the HTML file from the process argument
        /// </summary>
        private void LoadHTMLFile()
        {
            // Get process arguments
            string[] args = Environment.GetCommandLineArgs();
            //string[] args = { @"C:\Users\Svetlozar\Desktop\google.html" };

            // Only executable path provided?
            if (args.Length == 1)
            {
                MessageBox.Show("Failed to extract HTML file path from arguments.");
                return;
            }

            // HTML file is passed as second argument
            this.mHtmlFile = args[1];

            // Invalid file path?
            if (!File.Exists(this.mHtmlFile))
            {
                // Report it to the user
                MessageBox.Show("The HTML file path is invalid.");
            }

            // Prepare page HTML
            string html;

            try
            {
                // Read it carefully from the temp file
                html = File.ReadAllText(this.mHtmlFile);
            }
            catch (Exception exc)
            {
                // Report it to the user
                MessageBox.Show("Failed to read from the HTML file:" + exc.Message);
                return;
            }

            // Load it into the CefSharp WebBrowser
            this.WebBrowser.LoadHtml(html, "http://localhost");
        }

        /// <summary>
        /// Invoked when keys are pressed via the keyboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Escape?
            if (e.Key == Key.Escape)
            {
                // Close form
                this.Close();
            }

            // Ctrl + U?
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.U)
            {
                // View source
                this.WebBrowser.ViewSource();
            }

            // F12?
            if (e.Key == Key.F12)
            {
                // Show dev tools
                this.WebBrowser.ShowDevTools();
            }
        }
    }
}
