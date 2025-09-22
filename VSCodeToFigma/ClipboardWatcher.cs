using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace VSCodeToFigma
{
    public partial class Program
    {
        private class ClipboardWatcher : NativeWindow
        {
            public ClipboardWatcher()
            {
                CreateHandle(new CreateParams());
                AddClipboardFormatListener(this.Handle);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_CLIPBOARDUPDATE)
                {
                    OnClipboardChanged();
                }
                base.WndProc(ref m);
            }


            private void OnClipboardChanged()
            {
                // Check if clipboard contains HTML format
                if (Clipboard.ContainsData(DataFormats.Html))
                {
                    var htmlData = Clipboard.GetData(DataFormats.Html) as string;
                    if (!htmlData.Contains("<HTML><HEAD><meta charset=\"UTF-8\"><TITLE>Snippet</TITLE></HEAD><BODY><!--StartFragment-->"))
                    {
                        return;
                    }

                    NotifyIcon notifyIcon = new NotifyIcon
                    {
                        Icon = SystemIcons.Information,
                        Visible = true,
                        BalloonTipTitle = $"SVG Conversion Started",
                        BalloonTipText = $"SVG Conversion Started",
                    };
                    notifyIcon.ShowBalloonTip(500);




                    if (!string.IsNullOrEmpty(htmlData))
                    {
                        string htmlFormattedDark = HtmlFormat.GetHtmlFormat(htmlData);
                        string htmlFormattedLight = Replacer.ReplaceColors(htmlFormattedDark);

                        string baseDirectory = @"A:\SVG";
                        if (!Directory.Exists(baseDirectory))
                        {
                            //throw new DirectoryNotFoundException($"The directory '{baseDirectory}' does not exist.");

                            MessageBox.Show($"The directory '{baseDirectory}' does not exist.", "Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            return;
                        }
                        string[] svgAlreadyExistingFiles = Directory.GetFiles(baseDirectory, "*.svg");
                        int maximumFileNumber = 0;
                        int fileNumber = -1;
                        foreach (string svgFile in svgAlreadyExistingFiles)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(svgFile).Replace("_dark", "").Replace("_light", "");
                            if (int.TryParse(fileName, out fileNumber))
                            {
                                if (fileNumber > maximumFileNumber)
                                {
                                    maximumFileNumber = fileNumber;
                                }
                            }
                        }
                        maximumFileNumber++;


                        HtmlToSvgAdi.ConvertAndSaveToFile(htmlFormattedDark, maximumFileNumber.ToString(), "dark" , setToClipboard:false);
                        HtmlToSvgAdi.ConvertAndSaveToFile(htmlFormattedLight, maximumFileNumber.ToString(), "light", setToClipboard:true);

                        // Show a little icon to indicate success and dissapearing after 2 seconds
                        // The NotifyIcon should have a background foto the .svg file !

                        if(fileNumber == -1)
                        {
                            return;
                        }

                        notifyIcon = new NotifyIcon
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

        }

    }
}
