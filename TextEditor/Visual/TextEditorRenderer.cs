using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.TextFormatting;
using System.Windows;

namespace TextEditor.Visual
{
    class TextEditorRenderer
    {
        private DrawingGroup dg;
        private Rectangle Rectus;
        private TextEditorConfiguration configuration;

        public TextEditorRenderer(DrawingGroup dg, Rectangle rect)
        {
            this.dg = dg;
            this.Rectus = rect;
        }

        public void SetConfiguration(TextEditorConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void DisplayText(List<CustomTextRun> runs)
        {
            var dc = dg.Open();

            var textSource = new CustomTextSource(runs, configuration);
            var formatter = TextFormatter.Create();
            var properties = new CustomTextParagraphProperties(
                FlowDirection.LeftToRight,
                TextAlignment.Left,
                true,
                false,
                new CustomTextRunProperties(configuration),
                TextWrapping.NoWrap,
                configuration.TextHeight,
                0
            );
            int textStorePosition = 0;
            Point linePosition = new Point(0, 0);

            double[] rectDimensions = { 0, 0 };

            while (textStorePosition < textSource.Length)
            {
                using (var line = formatter.FormatLine(textSource, textStorePosition, 5000, properties, null))
                {
                    line.Draw(dc, linePosition, InvertAxes.None);
                    textStorePosition += line.Length;
                    linePosition.Y += line.Height;

                    rectDimensions[0] = line.Width > rectDimensions[0] ? line.Width : rectDimensions[0];
                    rectDimensions[1] += line.Height;
                }
            }

            dc.Close();
            Rectus.Width = rectDimensions[0];
            Rectus.Height = rectDimensions[1];
        }
    }
}
