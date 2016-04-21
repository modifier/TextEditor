using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TextEditor.Visual.Hightlight
{
    class HightlightScheme
    {
        private Dictionary<string, TextEditorConfiguration> scheme = new Dictionary<string, TextEditorConfiguration>();

        private TextEditorConfiguration defaultConfiguration;

        public HightlightScheme(TextEditorConfiguration defaultConfiguration)
        {
            this.defaultConfiguration = defaultConfiguration;
        }

        public void AddHightlightRule(string ruleName, TextEditorConfiguration configuration)
        {
            if (scheme.ContainsKey(ruleName))
            {
                return;
            }

            configuration.MergeConfiguration(defaultConfiguration);

            scheme.Add(ruleName, configuration);
        }

        public TextEditorConfiguration GetConfiguration(string ruleName)
        {
            return ruleName != null && scheme.ContainsKey(ruleName) ? scheme[ruleName] : defaultConfiguration;
        }

        public TextEditorConfiguration GetDefaultConfiguration()
        {
            return defaultConfiguration;
        }
    }
}
