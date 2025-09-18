using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VSCodeToFigma
{
    public static class HtmlFormat
    {
        public static string GetHtmlFormat(string htmlCode)
        {

            List<string> lines = htmlCode.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var line = lines[i];
                if (line.StartsWith("Version:")
                    || line.StartsWith("StartHTML:")
                    || line.StartsWith("EndHTML:")
                    || line.StartsWith("StartFragment:")
                    || line.StartsWith("EndFragment:")
                    || line.Contains("<html>")
                    || line.Contains("</html>")
                    || line.Contains("<body>")
                    || line.Contains("</body>")
                   )
                {
                    lines.RemoveAt(i);
                }
            }

            // Reconstruct the HTML code without the unwanted lines
            htmlCode = string.Join(Environment.NewLine, lines).Trim();
            htmlCode = htmlCode.Replace("<!--StartFragment-->", "");
            htmlCode = htmlCode.Replace("<!--EndFragment-->", "");

            //Find the first occurence of: <div style=" and replace with <div style="padding: 10px; display: inline-block; width: fit-content; 

            int index = htmlCode.IndexOf("<div style=\"", StringComparison.OrdinalIgnoreCase);

            if (index != -1)
            {
                int endIndex = htmlCode.IndexOf(">", index);
                if (endIndex != -1)
                {
                    string styleContent = htmlCode.Substring(index + 12, endIndex - index - 12); // 12 is the length of "<div style=\""
                    string newStyleContent = "padding: 10px; display: inline-block; width: fit-content; " + styleContent;
                    htmlCode = htmlCode.Remove(index + 12, styleContent.Length).Insert(index + 12, newStyleContent);
                }
            }


            htmlCode = ShrinkLeftAdditionalTabs(htmlCode);





            return htmlCode;
        }

        private static string ShrinkLeftAdditionalTabs(string htmlCode)
        {
            string input = htmlCode;

            var pattern = @"<div><span style=""color: #[0-9a-fA-F]{6};?"">((?:&#160; )*)";

            var matches = Regex.Matches(input, pattern);

            int minTabs = int.MaxValue;

            // Găsește minimul de &#160;
            foreach (Match m in matches)
            {
                var spaces = m.Groups[1].Value; // ex: "&#160; &#160; &#160; "
                if (string.IsNullOrEmpty(spaces))
                {
                    minTabs = 0;
                    break;
                }
                int count = spaces.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (count < minTabs)
                    minTabs = count;
            }

            if (minTabs == int.MaxValue) minTabs = 0;

            // Construiește regex pentru înlocuire: eliminăm exact minTabs &#160; (cu spații) după <span>
            string tabsToRemove = string.Join(" ", new string[minTabs].Select(_ => "&#160;"));
            if (minTabs > 0)
                tabsToRemove += " ";

            string replacementPattern = $@"(<div><span style=""color: #[0-9a-fA-F]{{6}};?"">){tabsToRemove}";

            // Înlocuim în string:
            string output = Regex.Replace(input, replacementPattern, "$1");

            return output;
        }
    }
}
