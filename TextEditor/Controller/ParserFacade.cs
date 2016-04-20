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

            var result = getTerminals(string.Join("\n", text));

            int i = 0;
            bool selectionRun = false;
            TextCursor leftSelectionCursor = null,
                rightSelectionCursor = null;

            if (selection.selectionExists())
            {
                leftSelectionCursor = selection.getLeftCursor();
                rightSelectionCursor = selection.getRightCursor();
            }

            foreach (var line in text)
            {
                // todo: refactoring

                if (leftSelectionCursor != null && leftSelectionCursor.y == i && rightSelectionCursor.y == i)
                {
                    string leftPart = line.Substring(0, leftSelectionCursor.x),
                        middlePart = line.Substring(leftSelectionCursor.x, rightSelectionCursor.x - leftSelectionCursor.x),
                        rightPart = line.Substring(rightSelectionCursor.x);

                    if (leftPart.Length != 0)
                    {
                        Runs.Add(new CustomTextRun { Text = leftPart });
                    }

                    Runs.Add(new CustomTextRun { Text = middlePart, IsSelection = true });

                    if (rightPart.Length != 0)
                    {
                        Runs.Add(new CustomTextRun { Text = rightPart });
                    }
                }
                else if (leftSelectionCursor != null && leftSelectionCursor.y == i)
                {
                    string leftPart = line.Substring(0, leftSelectionCursor.x),
                        rightPart = line.Substring(leftSelectionCursor.x);

                    if (leftPart.Length != 0)
                    {
                        Runs.Add(new CustomTextRun { Text = leftPart });
                    }

                    Runs.Add(new CustomTextRun { Text = rightPart, IsSelection = true });
                    selectionRun = true;
                }
                else if (selectionRun && rightSelectionCursor.y == i)
                {
                    string leftPart = line.Substring(0, rightSelectionCursor.x),
                        rightPart = line.Substring(rightSelectionCursor.x);

                    if (leftPart.Length != 0)
                    {
                        Runs.Add(new CustomTextRun { Text = leftPart, IsSelection = true });
                    }

                    Runs.Add(new CustomTextRun { Text = rightPart });
                    selectionRun = false;
                }
                else if (selectionRun)
                {
                    Runs.Add(new CustomTextRun { Text = line, IsSelection = true });
                }
                else
                {
                    Runs.Add(new CustomTextRun { Text = line });
                }

                Runs.Add(new CustomTextRun { IsEndParagraph = true });
                i++;
            }

            return Runs;
        }
    }
}
