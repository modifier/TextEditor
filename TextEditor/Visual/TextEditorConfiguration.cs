using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TextEditor.Visual
{
    struct TextEditorConfiguration
    {
        public string FontFamily;
        public int? FontSize;
        public int? TextHeight;
        public Brush ForegroundColor;
        public Brush BackgroundColor;

        public TextEditorConfiguration MergeConfiguration(TextEditorConfiguration config)
        {
            if (FontFamily == "" || FontFamily == null)
            {
                FontFamily = config.FontFamily;
            }

            if (FontSize == null)
            {
                FontSize = config.FontSize;
            }

            if (TextHeight == null)
            {
                TextHeight = config.TextHeight;
            }

            if (ForegroundColor == null)
            {
                ForegroundColor = config.ForegroundColor;
            }

            if (BackgroundColor == null)
            {
                BackgroundColor = config.BackgroundColor;
            }

            return this;
        }
    }
}
