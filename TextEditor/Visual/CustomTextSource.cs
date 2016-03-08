using System;
using System.Collections.Generic;
using System.Windows.Media.TextFormatting;

namespace TextEditor.Visual
{
    class CustomTextSource : TextSource
    {
        private List<CustomTextRun> Runs;
        private TextEditorConfiguration configuration;

        public CustomTextSource(List<CustomTextRun> Runs, TextEditorConfiguration configuration)
        {
            this.Runs = Runs;
            this.configuration = configuration;
        }

        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
        {
            throw new NotImplementedException();
        }

        public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
        {
            throw new NotImplementedException();
        }

        public override TextRun GetTextRun(int textSourceCharacterIndex)
        {
            int pos = 0;
            foreach (var currentRun in Runs)
            {
                if (textSourceCharacterIndex < pos + currentRun.Length)
                {
                    if (currentRun.IsEndParagraph)
                    {
                        return new TextEndOfParagraph(1);
                    }

                    var props = new CustomTextRunProperties(configuration);

                    return new TextCharacters(
                        currentRun.Text,
                        textSourceCharacterIndex - pos,
                        currentRun.Length - (textSourceCharacterIndex - pos),
                        props);
                }
                pos += currentRun.Length;
            }
            
            return new TextEndOfParagraph(1);
        }

        public int Length
        {
            get
            {
                int r = 0;
                foreach (var currentRun in Runs)
                {
                    r += currentRun.Length;
                }
                return r;
            }
        }
    }
}
