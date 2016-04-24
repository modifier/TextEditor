using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TextEditor.Controller;
using TextEditor.Visual;
using TextEditor.Visual.Hightlight;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for TextEditorControl.xaml
    /// </summary>
    public partial class TextEditorControl : UserControl
    {
        private TextEditorRenderer renderer;

        private TextController controller;

        private HighlightImporter importer;

        private ParserFacade parser;

        public TextEditorControl()
        {
            InitializeComponent();

            importer = new HighlightImporter();
            renderer = new TextEditorRenderer(mainBrush, Rectus, Surface, importer.GetDefaultScheme());
            parser = new ParserFacade();

            controller = new TextController(renderer, parser);
        }

        public void SetGrammar(string grammar)
        {
            parser.SetGrammar(grammar);
        }

        public void SetHighlight(string highlightScheme)
        {
            var scheme = importer.ImportHighlightScheme(highlightScheme);

            if (scheme != null)
            {
                renderer.SetHightlightScheme(scheme);
            }
        }

        public void SetContent(string content)
        {
            controller.setContent(content);
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsFocused)
            {
                Focus();
            }

            var position = e.GetPosition((IInputElement)sender);
            controller.setCursorFromPoint(position);
        }

        private void ScrollViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            controller.keyPress(e.Key);
        }
    }
}
