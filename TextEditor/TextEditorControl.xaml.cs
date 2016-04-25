using System.Collections.Generic;
using System.IO;
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

        private string extension;

        private Settings settings = new Settings();

        private bool redraw = true;

        private string _grammarPath;

        private string _highlightPath;

        private string grammarPath
        {
            get
            {
                return _grammarPath;
            }

            set
            {
                if (_grammarPath == value)
                {
                    return;
                }

                _grammarPath = value;

                SaveSettings();
                UpdateGrammar();
            }
        }

        private string highlightPath
        {
            get
            {
                return _highlightPath;
            }

            set
            {
                if (_highlightPath == value)
                {
                    return;
                }

                _highlightPath = value;

                SaveSettings();
                UpdateHighlight();
            }
        }

        public TextEditorControl()
        {
            InitializeComponent();

            importer = new HighlightImporter();
            renderer = new TextEditorRenderer(mainBrush, Rectus, Surface, importer.GetDefaultScheme());
            parser = new ParserFacade();

            controller = new TextController(renderer, parser);
        }

        public void SetGrammar(string filepath)
        {
            grammarPath = filepath;
        }

        public void SetHighlight(string filepath)
        {
            highlightPath = filepath;
        }

        public void SetContent(string content)
        {
            controller.setContent(content);
        }

        public void OpenContent(string filepath)
        {
            var content = File.ReadAllText(filepath);
            extension = Path.GetExtension(filepath);

            GetSettings();
            controller.setContent(content);
        }

        public void SaveContent(string filePath)
        {
            if (filePath == "")
            {
                return;
            }

            File.WriteAllText(filePath, controller.getContent());
            string extension = Path.GetExtension(filePath);

            if (this.extension == extension)
            {
                return;
            }

            this.extension = extension;
            SaveSettings();
        }

        public void SaveSettings()
        {
            if (grammarPath != null)
            {
                settings.SetGrammarForExtension(extension, grammarPath);
            }

            if (highlightPath != null)
            {
                settings.SetHighlightForExtension(extension, highlightPath);
            }
        }

        public void GetSettings()
        {
            redraw = false;

            grammarPath = settings.GetGrammarForExtension(extension);
            highlightPath = settings.GetHighlightForExtension(extension);

            redraw = true;
        }

        public void UpdateGrammar()
        {
            if (!File.Exists(grammarPath))
            {
                parser.UnsetGrammar(redraw);

                return;
            }

            string content = File.ReadAllText(grammarPath);

            parser.SetGrammar(content, redraw);
        }

        public void UpdateHighlight()
        {
            if (!File.Exists(highlightPath))
            {
                renderer.SetHightlightScheme(importer.GetDefaultScheme(), redraw);

                return;
            }

            string content = File.ReadAllText(highlightPath);

            var scheme = importer.ImportHighlightScheme(content);

            if (scheme != null)
            {
                renderer.SetHightlightScheme(scheme, redraw);
            }
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
