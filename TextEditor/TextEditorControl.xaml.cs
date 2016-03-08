using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TextEditor.Visual;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for TextEditorControl.xaml
    /// </summary>
    public partial class TextEditorControl : UserControl
    {
        public TextEditorControl()
        {
            InitializeComponent();
            DisplayText();
        }

        // http://stackoverflow.com/questions/2750576/subscript-superscript-in-formattedtext-class
        // https://msdn.microsoft.com/ru-ru/library/ms754036%28v=vs.100%29.aspx

        private string[] text = {
        "The quick brown fox jumps over the lazy dog",
        "Съешь ещё этих мягких францзских булок, да выпей же чаю"
        };

        private void DisplayText()
        {
            var testDest = new DrawingGroup();
            var dc = mainBrush.Open();

            var textSource = new CustomTextSource(transformText(text));
            var formatter = TextFormatter.Create();
            var properties = new CustomTextParagraphProperties(
                FlowDirection.LeftToRight,
                TextAlignment.Left,
                true,
                false,
                new CustomTextRunProperties(),
                TextWrapping.Wrap,
                30,
                0
            );
            int textStorePosition = 0;
            Point linePosition = new Point(0, 0);

            while (textStorePosition < textSource.Length)
            {
                using (var line = formatter.FormatLine(textSource, textStorePosition, 96*6, properties, null))
                {
                    line.Draw(dc, linePosition, InvertAxes.None);
                    textStorePosition += line.Length;
                    linePosition.Y += line.Height;
                }
            }

            dc.Close();
        }

        private List<CustomTextRun> transformText(string[] text)
        {
            List<CustomTextRun> Runs = new List<CustomTextRun>();

            foreach (var line in text)
            {
                Runs.Add(new CustomTextRun { Text = line });
                Runs.Add(new CustomTextRun { IsEndParagraph = true });
            }

            return Runs;
        }
    }
}
