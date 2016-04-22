using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace TextEditor.Visual
{
    class CustomTextRunProperties : TextRunProperties
    {
        protected TextEditorConfiguration configuration;

        public CustomTextRunProperties(TextEditorConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public override Brush BackgroundBrush
        {
            get
            {
                return configuration.BackgroundColor;
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
                return (int) configuration.FontSize;
            }
        }

        public override double FontRenderingEmSize
        {
            get
            {
                return (int) configuration.FontSize;
            }
        }

        public override Brush ForegroundBrush
        {
            get
            {
                return configuration.ForegroundColor;
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
                return new Typeface(
                    new FontFamily(configuration.FontFamily),
                    configuration.IsItalic == true ? FontStyles.Italic : FontStyles.Normal,
                    configuration.IsBold == true ? FontWeights.Heavy : FontWeights.Normal,
                    FontStretches.Normal
               );
            }
        }
    }
}
