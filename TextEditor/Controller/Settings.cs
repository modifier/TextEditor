using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Controller
{
    class Settings
    {
        private Dictionary<string, Dictionary<string, string>> settings = new Dictionary<string, Dictionary<string, string>>();

        public void SetGrammarForExtension(string extension, string grammarPath)
        {
            AddData(extension, grammarPath, "typeGrammars");
        }

        public void SetHighlightForExtension(string extension, string extensionPath)
        {
            AddData(extension, extensionPath, "typeHighlight");
        }

        public string GetGrammarForExtension(string extension)
        {
            return GetData(extension, "typeGrammars");
        }

        public string GetHighlightForExtension(string extension)
        {
            return GetData(extension, "typeHighlight");
        }

        private void AddData(string extension, string path, string type)
        {
            GetData(extension, type);

            settings[type][extension] = path;
            Properties.Settings.Default[type] = ConvertToString(settings[type]);
            Properties.Settings.Default.Save();
        }

        private string GetData(string extension, string type)
        {
            settings[type] = ConvertFromString((string) Properties.Settings.Default[type]);

            if (!settings[type].ContainsKey(extension))
            {
                return null;
            }

            return settings[type][extension];
        }

        private Dictionary<string, string> ConvertFromString(string data)
        {
            if (data == "")
            {
                return new Dictionary<string, string>();
            }

            return data.Split(';')
                    .Select(s => s.Split(','))
                    .ToDictionary(
                        p => p[0].Trim(),
                        p => p[1].Trim()
                    );
        }

        private string ConvertToString(Dictionary<string, string> dict)
        {
            return string.Join(";", dict.Select(
                p => string.Format("{0},{1}", p.Key, p.Value)
            ));
        }
    }
}
