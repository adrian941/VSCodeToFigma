using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCodeToFigma
{
    internal class ColorMapping
    {
        public string DarkModeColor { get; set; }
        public string LightModeColor { get; set; }
    }
    internal class Replacer
    {
        internal static string ReplaceColors(string htmlCode)
        {
            string oldDarkModeFileContent = htmlCode;

            //Get the lines of file Colors.txt
            string[] lines = File.ReadAllLines("Colors.txt");
            List<ColorMapping> colorMappings = new List<ColorMapping>();
            foreach (string line in lines)
            {
                string currentLine = line.Trim();
                //string[] parts = line.Split(',');
                //split by every type of spaces
                //string[] parts = currentLine.Split({' ', '\t', ','], StringSplitOptions.RemoveEmptyEntries);
                string[] parts = currentLine.Split(new char[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 3)
                {
                    colorMappings.Add(new ColorMapping
                    {
                        DarkModeColor = parts[1].Trim(),
                        LightModeColor = parts[2].Trim()
                    });
                }
            }


 

            //Executa all the replacers (colorMapping)
            foreach (ColorMapping colorMapping in colorMappings)
            {
                oldDarkModeFileContent = oldDarkModeFileContent.Replace(colorMapping.DarkModeColor, colorMapping.LightModeColor);
            }


            return oldDarkModeFileContent;
        }
    }
}
