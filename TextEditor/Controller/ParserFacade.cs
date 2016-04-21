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

            var terminals = getTerminals(joinedText);
            int tokenId = 0;

            string currentLine = "";
            for (int i = 0; i < joinedText.Length; i++)
            {
                var token = tokenId < terminals.Count ? terminals[tokenId] : null;

                if (token != null && isTokenStart(token, joinedText, i))
                {
                    Runs.Add(new CustomTextRun { Text = currentLine });
                    Runs.Add(new CustomTextRun { Text = token.Content, RuleName = token.Rule.Name });
                    currentLine = "";

                    i += token.Content.Length - 1;
                    tokenId++;

                    continue;
                }

                if (joinedText[i] == '\n')
                {
                    Runs.Add(new CustomTextRun { Text = currentLine });
                    Runs.Add(new CustomTextRun { IsEndParagraph = true });
                    currentLine = "";

                    continue;
                }

                currentLine += joinedText[i];
            }
            
            Runs.Add(new CustomTextRun { Text = currentLine });
            Runs.Add(new CustomTextRun { IsEndParagraph = true });

            return Runs;
        }

        private bool isTokenStart(IParsingTreeTerminal token, string text, int position)
        {
            string tokenContent = token.Content;

            for (int i = 0; i < token.Content.Length; i++)
            {
                if (text[position + i] != tokenContent[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
