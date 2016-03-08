using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using TextEditor.Visual;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for TextEditorControl.xaml
    /// </summary>
    public partial class TextEditorControl : UserControl
    {
        private TextEditorRenderer renderer;

        private string[] text = {
        "The quick brown fox jumps over the lazy dog",
        "Съешь ещё этих мягких францзских булок, да выпей же чаю"
        };

        public TextEditorControl()
        {
            InitializeComponent();

            renderer = new TextEditorRenderer(mainBrush, Rectus);
            renderer.SetConfiguration(new TextEditorConfiguration { FontFamily = "Lucida Console", FontSize = 14, TextHeight = 14, ForegroundColor = Brushes.Black });
            renderer.DisplayText(transformText(text));
        }

        // http://stackoverflow.com/questions/2750576/subscript-superscript-in-formattedtext-class
        // https://msdn.microsoft.com/ru-ru/library/ms754036%28v=vs.100%29.aspx

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
