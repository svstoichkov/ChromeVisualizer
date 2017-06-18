using System;
using System.Diagnostics;

using ChromeVisualizer.Debugger;

using Microsoft.VisualStudio.DebuggerVisualizers;

[assembly: DebuggerVisualizer(typeof(DebuggerIntegration), typeof(VisualizerObjectSource), Target = typeof(String), Description = "Chrome Visualizer")]

namespace ChromeVisualizer.Debugger
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Microsoft.VisualStudio.DebuggerVisualizers;

    public class DebuggerIntegration : DialogDebuggerVisualizer
    {
        private static readonly string chromeVisualizerPath = @"C:\Program Files\ChromeVisualizer\ChromeVisualizer.Visualizer.exe";
        
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            var html = objectProvider.GetObject().ToString();
            var htmlFilePath = Path.GetTempFileName();

            try
            {
                File.WriteAllText(htmlFilePath, html, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save HTML to a temporary text file: " + ex.Message);
                return;
            }
            
            var process = new Process();
            process.StartInfo.FileName = chromeVisualizerPath;
            process.StartInfo.Arguments = htmlFilePath;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to start the Chrome Visualizer executable: " + ex.Message);
                return;
            }
            
            process.WaitForExit();

            try
            {
                File.Delete(htmlFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete the temporary HTML file: " + ex.Message);
            }
        }
    }
}

