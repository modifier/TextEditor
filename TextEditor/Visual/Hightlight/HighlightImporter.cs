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

        private bool isDefaultConfiguration = false;

        private Dictionary<string, TextEditorConfiguration> configurations;

        public HighlightImporter()
        {
            string coloringGrammar = Properties.Resources.ColoringGrammar;

            parser = new ParserFacade(coloringGrammar);
        }

        public HightlightScheme ImportHighlightScheme(string text)
        {
            configurations = new Dictionary<string, TextEditorConfiguration>();
            defaultConfiguration = new TextEditorConfiguration();

            ITreeParsingResult tree = parser.getTree(text);
            VisitNode(tree.Tree);

            defaultConfiguration.MergeConfiguration(new TextEditorConfiguration { FontFamily = "Lucida Console", FontSize = 14, TextHeight = 14, ForegroundColor = Brushes.Black });
            scheme = new HightlightScheme(defaultConfiguration);
            foreach (KeyValuePair<string, TextEditorConfiguration> entry in configurations)
            {
                scheme.AddHightlightRule(entry.Key, entry.Value);
            }

            return scheme;
        }

        private void VisitNode(IParsingTreeNode node)
        {
            if (node is IParsingTreeTerminal && node.Rule.Name == "name" && HasParentRule("ruleName"))
            {
                ruleNames.Add(((IParsingTreeTerminal) node).Content);
            }

            if (node is IParsingTreeTerminal && node.Rule.Name == "default_")
            {
                isDefaultConfiguration = true;
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
                    if (!configurations.ContainsKey(ruleName))
                    {
                        configurations.Add(ruleName, currentConfiguration);
                    }
                }

                ruleNames.Clear();
                currentConfiguration = new TextEditorConfiguration();
            }

            if (node.Rule != null && node.Rule.Name == "directive" && (GetPreviousNode() == null || GetPreviousNode().Rule.Name != "directive"))
            {
                if (isDefaultConfiguration)
                {
                    defaultConfiguration = currentConfiguration;
                }

                isDefaultConfiguration = false;
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
