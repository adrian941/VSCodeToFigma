using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace VSCodeToFigma
{
    public static class HtmlFormat
    {
        public static string GetHtmlFormat(string htmlCode)
        {

            htmlCode = htmlCode.Replace("pre>","div>");
            //htmlCode looks too compact , do we have something to format it on lines ?



            List<string> lines = htmlCode.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                if (line.StartsWith("Version:")
                    || line.StartsWith("StartHTML:")
                    || line.StartsWith("EndHTML:")
                    || line.StartsWith("StartFragment:")
                    || line.StartsWith("EndFragment:")
                    || line.StartsWith("StartSelection")
                    || line.StartsWith("EndSelection")
                    || line.StartsWith("<html>")
                    || line.StartsWith("</html>")
                    || line.StartsWith("<body>")
                    || line.StartsWith("</body>")

                    || line.StartsWith("<head>")
                    || line.StartsWith("</head>")
                    || line.StartsWith("<meta")
                    || line.StartsWith("<title>")
                   )
                {
                    lines.RemoveAt(i);
                }
            }

            htmlCode = string.Join(Environment.NewLine, lines).Trim();





             


             

          


            // Reconstruct the HTML code without the unwanted lines
            htmlCode = string.Join(Environment.NewLine, lines).Trim();
            htmlCode = htmlCode.Replace("<!--StartFragment-->", "");
            htmlCode = htmlCode.Replace("<!--EndFragment-->", "");


          
            htmlCode = htmlCode.Replace("font-family:Cascadia Mono;", "font-family: Consolas, 'Courier New', monospace; font-weight: normal;width:fit-content; padding:5px;");
            htmlCode = htmlCode.Replace("<pre style=\"", "<pre style=\"");



            return htmlCode;
        }

      
    }
}
