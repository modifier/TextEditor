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

        private HightlightScheme scheme;

        private TextEditorConfiguration defaultConfiguration;

        private TextEditorConfiguration currentConfiguration;

        private List<IParsingTreeNode> nodeStack = new List<IParsingTreeNode>();

        private List<string> ruleNames = new List<string>();

        public HighlightImporter()
        {
            string coloringGrammar = Properties.Resources.ColoringGrammar;

            parser = new ParserFacade(coloringGrammar);

            defaultConfiguration = new TextEditorConfiguration { FontFamily = "Lucida Console", FontSize = 14, TextHeight = 14, ForegroundColor = Brushes.Black };
        }

        public HightlightScheme ImportHighlightScheme(string text)
        {
            ITreeParsingResult tree = parser.getTree(text);
            scheme = new HightlightScheme(defaultConfiguration);

            VisitNode(tree.Tree);

            return scheme;
        }

        public HightlightScheme ImportHighlightScheme()
        {
            HightlightScheme scheme = new HightlightScheme(new TextEditorConfiguration { FontFamily = "Lucida Console", FontSize = 14, TextHeight = 14, ForegroundColor = Brushes.Black });

            return scheme;
        }

        private void VisitNode(IParsingTreeNode node)
        {
            if (node is IParsingTreeTerminal && node.Rule.Name == "name" && HasParentRule("ruleName"))
            {
                ruleNames.Add(((IParsingTreeTerminal) node).Content);
            }

            if (node is IParsingTreeTerminal && node.Rule.Name == "colorValue" && HasParentRule("color"))
            {
                var converter = new BrushConverter();
                currentConfiguration.ForegroundColor = (Brush)converter.ConvertFromString(((IParsingTreeTerminal)node).Content);
            }

            if (node is IParsingTreeTerminal && node.Rule.Name == "colorValue" && HasParentRule("background"))
            {
                var converter = new BrushConverter();
                currentConfiguration.BackgroundColor = (Brush)converter.ConvertFromString(((IParsingTreeTerminal)node).Content);
            }

            if (node is IParsingTreeGroup)
            {
                foreach (IParsingTreeNode innerNode in ((IParsingTreeGroup)node).Childs)
                {
                    nodeStack.Add(innerNode);
                    VisitNode(innerNode);
                    nodeStack.RemoveAt(nodeStack.Count - 1);
                }
            }

            if (node.Rule != null && node.Rule.Name == "rule" && (GetPreviousNode() == null || GetPreviousNode().Rule.Name != "rule"))
            {
                foreach (string ruleName in ruleNames)
                {
                    scheme.AddHightlightRule(ruleName, currentConfiguration);
                }

                ruleNames.Clear();
                currentConfiguration = new TextEditorConfiguration();
            }
        }

        private bool HasParentRule(string ruleName)
        {
            for (int i = nodeStack.Count - 1; i >= 0; i--)
            {
                var node = nodeStack[i];

                if (node.Rule != null && node.Rule.Name == ruleName)
                {
                    return true;
                }
            }

            return false;
        }

        private IParsingTreeNode GetPreviousNode()
        {
            return nodeStack.Count > 1 ? nodeStack[nodeStack.Count - 2] : null;
        }
    }
}
