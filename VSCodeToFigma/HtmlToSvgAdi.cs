using HiQPdf;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VSCodeToFigma
{
    public static class HtmlToSvgAdi
    {
        public static void Convert(string htmlCode)
        {

            HtmlToSvg htmlToSvgConverter = new HtmlToSvg
            {
                BrowserWidth = 0,
                BrowserHeight = 0,
                HtmlLoadedTimeout = 120,

            };

            string baseUrl = "https://www.example.com/"; // Set a base URL if needed



            string baseDirectory = @"A:\SVG";
            if (!Directory.Exists(baseDirectory))
            {
                //throw new DirectoryNotFoundException($"The directory '{baseDirectory}' does not exist.");

                MessageBox.Show($"The directory '{baseDirectory}' does not exist.", "Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }


            string[] svgAlreadyExistingFiles = Directory.GetFiles(baseDirectory, "*.svg");
            int maximumFileNumber = 0;
            foreach (string svgFile in svgAlreadyExistingFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(svgFile);
                if (int.TryParse(fileName, out int fileNumber))
                {
                    if (fileNumber > maximumFileNumber)
                    {
                        maximumFileNumber = fileNumber;
                    }
                }
            }
            maximumFileNumber++;



            string svgLatestFileName = Path.Combine(baseDirectory, $"{maximumFileNumber}.svg");
            htmlToSvgConverter.ConvertHtmlToFile(htmlCode, baseUrl, svgLatestFileName);

            // Replace in File <svg width="264.583mm" height="264.583mm" with <svg 

            var content = File.ReadAllText(svgLatestFileName);

            content = Regex.Replace(content, @"\s(width|height)=(""|').*?\2", "", RegexOptions.IgnoreCase);

            File.WriteAllText(svgLatestFileName, content);



            if (!File.Exists(svgLatestFileName))
            {
                return;
            }


            Process process = new Process();
            process.StartInfo.FileName = @"InkScape\inkscape.exe";
            process.StartInfo.Arguments = $"\"{svgLatestFileName}\" --export-filename=\"{svgLatestFileName}\" --export-text-to-path";

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            process.WaitForExit();



            if (!File.Exists(svgLatestFileName))
            {
                return;
            }

            Clipboard.SetFileDropList(new System.Collections.Specialized.StringCollection { svgLatestFileName });

            var svgContent = File.ReadAllText(svgLatestFileName);
            var dataObj = new DataObject();
            dataObj.SetData(DataFormats.Text, svgContent); // text simplu
            dataObj.SetData("image/svg+xml", svgContent);  // format SVG

            Clipboard.SetDataObject(dataObj, true);

            // Show a little icon to indicate success and dissapearing after 2 seconds
            // The NotifyIcon should have a background foto the .svg file !

            NotifyIcon notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Information,
                Visible = true,
                BalloonTipTitle = $"SVG '{maximumFileNumber}.svg' is Done!",
                BalloonTipText = $"File is ready",


            };
            notifyIcon.ShowBalloonTip(2000);





        }
    }
}
