using System.Drawing;
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
                    if (!htmlData.StartsWith("<HTML><HEAD><meta charset=\"UTF-8\"><TITLE>Snippet</TITLE></HEAD><BODY><!--StartFragment--><pre style"))
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
                        string htmlFormatted = HtmlFormat.GetHtmlFormat(htmlData);

                        HtmlToSvgAdi.Convert(htmlFormatted);

                    }

                }
            }

        }

    }
}
