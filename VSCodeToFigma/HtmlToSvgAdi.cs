using HiQPdf;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VSCodeToFigma
{
    public static class HtmlToSvgAdi
    {
        public static void ConvertAndSaveToFile(string htmlCode, string fileNumber, string mode, bool setToClipboard)
        {
   


            HtmlToSvg htmlToSvgConverter = new HtmlToSvg
            {
                BrowserWidth = 0,
                BrowserHeight = 0,
                HtmlLoadedTimeout = 120,

            };

            string baseUrl = "https://www.example.com/"; // Set a base URL if needed








            string baseDirectory = @"A:\SVG";

            string svgLatestFileName = Path.Combine(baseDirectory, $"{fileNumber}_{mode}.svg");
          

            htmlToSvgConverter.ConvertHtmlToFile(htmlCode, baseUrl, svgLatestFileName);

            // Replace in File <svg width="264.583mm" height="264.583mm" with <svg 

            var content = File.ReadAllText(svgLatestFileName);

            content = ReplaceWH(content);

            File.WriteAllText(svgLatestFileName, content);



            if (!File.Exists(svgLatestFileName))
            {
                return ;
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
                return ;
            }


            if(setToClipboard)
            {
                Clipboard.SetFileDropList(new System.Collections.Specialized.StringCollection { svgLatestFileName });

                var svgContent = File.ReadAllText(svgLatestFileName);
                var dataObj = new DataObject();
                dataObj.SetData(DataFormats.Text, svgContent); // text simplu
                dataObj.SetData("image/svg+xml", svgContent);  // format SVG
                Clipboard.SetDataObject(dataObj, true);
            }
         

        }

      
 
    public static string ReplaceWH(string input)
    {
        // elimină width="...mm"
        input = Regex.Replace(input, @"\s*width=""[^""]*mm""", "", RegexOptions.IgnoreCase);

        // elimină height="...mm"
        input = Regex.Replace(input, @"\s*height=""[^""]*mm""", "", RegexOptions.IgnoreCase);

        return input;
    }




}
}
