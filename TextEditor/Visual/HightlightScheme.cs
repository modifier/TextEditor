using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TextEditor.Visual
{
    class HightlightScheme
    {
        private Dictionary<string, Brush> scheme = new Dictionary<string, Brush>();

        private TextEditorConfiguration defaultConfiguration;

        public HightlightScheme(TextEditorConfiguration defaultConfiguration)
        {
            this.defaultConfiguration = defaultConfiguration;
        }

        public void AddHightlightRule(string ruleName, Brush color)
        {
            scheme.Add(ruleName, color);
        }

        public Brush GetHighlightColor(string ruleName)
        {
            return scheme.ContainsKey(ruleName) ? scheme[ruleName] : defaultConfiguration.ForegroundColor;
        }

        public TextEditorConfiguration GetConfiguration(string ruleName)
        {
            if (ruleName == "" || ruleName == null)
            {
                return defaultConfiguration;
            }

            return new TextEditorConfiguration {
                FontFamily = defaultConfiguration.FontFamily,
                FontSize = defaultConfiguration.FontSize,
                TextHeight = defaultConfiguration.TextHeight,
                ForegroundColor = GetHighlightColor(ruleName)
            };
        }

        public TextEditorConfiguration GetDefaultConfiguration()
        {
            return defaultConfiguration;
        }
    }
}
