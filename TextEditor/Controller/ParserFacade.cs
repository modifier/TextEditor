using Portable.Parser;
using Portable.Parser.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Logic;
using TextEditor.Visual;

namespace TextEditor.Controller
{
    class ParserFacade
    {
        private IParserFabric factory;

        private string grammar = @"[OmitPattern(""[\s]*"")]
[RootRule(expr)]
SimpleArithmetics {

    productOp: '*' | '/';
    sumOp: '+' | '-';

    [RewriteRecursion]
    /*[ExpandRecursion]*/
    #expr: {
        |sum: expr sumOp expr;
        |product: expr productOp expr;
        |[right]power: expr '^' expr;
        |#braces: '(' expr ')';
        |num: ""[0-9]+"";
    };
}";

        public ParserFacade()
        {
            factory = getParserFactory(grammar);
        }

        private IParserFabric getParserFactory(string grammar)
        {
            var grammarResult = DefinitionGrammar.Parse(grammar, false);
            var rules = grammarResult.Rules.Cast<RuleSet>().ToArray();

            return Parsers.CreateFabric(rules.First().Name, rules);
        }

        private ITreeParsingResult getTree(string text)
        {
            var textReader = new StringSourceTextReader(text);
            var parser = factory.CreateTreeParser();
            parser.EnableLog = false;
            parser.MaterializeOmittedFragments = false;
            parser.UseDelayedStates = false;
            parser.RestoreRewritedRecursion = false;

            return parser.Parse(textReader);
        }

        public List<IParsingTreeTerminal> getTerminals(string text)
        {
            var tree = getTree(text);

            var flattener = new TreeFlattener(tree);

            return flattener.getTerminals();
        }

        public List<CustomTextRun> getTransformedText(List<string> text, TextSelection selection)
        {
            List<CustomTextRun> Runs = new List<CustomTextRun>();

            string joinedText = string.Join("\n", text);

            bool selectionRun = false;
            TextCursor leftSelectionCursor = null,
                rightSelectionCursor = null;

            int leftSelectionCursorPosition = -1,
                rightSelectionCursorPosition = -1;

            if (selection.selectionExists())
            {
                leftSelectionCursor = selection.getLeftCursor();
                rightSelectionCursor = selection.getRightCursor();

                leftSelectionCursorPosition = leftSelectionCursor.getHitPosition();
                rightSelectionCursorPosition = rightSelectionCursor.getHitPosition();
            }

            string currentLine = "";
            for (int i = 0; i < joinedText.Length; i++)
            {
                if (i == leftSelectionCursorPosition)
                {
                    selectionRun = true;

                    Runs.Add(new CustomTextRun { Text = currentLine, IsSelection = false });
                    currentLine = "";
                }
                else if (i == rightSelectionCursorPosition)
                {
                    selectionRun = false;

                    Runs.Add(new CustomTextRun { Text = currentLine, IsSelection = true });
                    currentLine = "";
                }

                if (joinedText[i] == '\n')
                {
                    Runs.Add(new CustomTextRun { Text = currentLine, IsSelection = selectionRun });
                    Runs.Add(new CustomTextRun { IsEndParagraph = true, IsSelection = selectionRun });
                    currentLine = "";

                    continue;
                }

                currentLine += joinedText[i];
            }
            
            Runs.Add(new CustomTextRun { Text = currentLine, IsSelection = selectionRun });
            Runs.Add(new CustomTextRun { IsEndParagraph = true, IsSelection = selectionRun });

            return Runs;
        }
    }
}
