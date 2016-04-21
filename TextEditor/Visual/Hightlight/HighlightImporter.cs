using Portable.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TextEditor.Controller;

namespace TextEditor.Visual.Hightlight
{
    class HighlightImporter
    {
        private ParserFacade parser;

        public HighlightImporter()
        {
            string coloringGrammar = Properties.Resources.ColoringGrammar;

            parser = new ParserFacade(coloringGrammar);
        }

        public HightlightScheme ImportHighlightScheme(string text)
        {
            ITreeParsingResult tree = parser.getTree(text);

            return ImportHighlightScheme();
        }

        public HightlightScheme ImportHighlightScheme()
        {

            HightlightScheme scheme = new HightlightScheme(new TextEditorConfiguration { FontFamily = "Lucida Console", FontSize = 14, TextHeight = 14, ForegroundColor = Brushes.Black });
            scheme.AddHightlightRule("braces", Brushes.Violet);
            scheme.AddHightlightRule("num", Brushes.DarkRed);

            return scheme;
        }
    }
}
