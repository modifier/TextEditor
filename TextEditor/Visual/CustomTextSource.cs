using System;
using System.Collections.Generic;
using System.Windows.Media.TextFormatting;
using TextEditor.Visual.Hightlight;

namespace TextEditor.Visual
{
    class CustomTextSource : TextSource
    {
        private List<CustomTextRun> Runs;
        private HightlightScheme scheme;

        public CustomTextSource(List<CustomTextRun> Runs, HightlightScheme scheme)
        {
            this.Runs = Runs;
            this.scheme = scheme;
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

                    TextRunProperties props = new CustomTextRunProperties(scheme.GetConfiguration(currentRun.RuleName));

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
