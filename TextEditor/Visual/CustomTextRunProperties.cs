using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace TextEditor.Visual
{
    class CustomTextRunProperties : TextRunProperties
    {
        public override Brush BackgroundBrush
        {
            get
            {
                return null;
            }
        }

        public override CultureInfo CultureInfo
        {
            get
            {
                return CultureInfo.CurrentCulture;
            }
        }

        public override double FontHintingEmSize
        {
            get
            {
                return 14;
            }
        }

        public override double FontRenderingEmSize
        {
            get
            {
                return 14;
            }
        }

        public override Brush ForegroundBrush
        {
            get
            {
                return Brushes.Black;
            }
        }

        public override TextDecorationCollection TextDecorations
        {
            get
            {
                return new TextDecorationCollection();
            }
        }

        public override TextEffectCollection TextEffects
        {
            get
            {
                return new TextEffectCollection();
            }
        }

        public override Typeface Typeface
        {
            get
            {
                return new Typeface("Lucida Console");
            }
        }
    }
}
