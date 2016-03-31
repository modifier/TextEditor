using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Visual
{
    class CustomTextRun
    {
        public string Text;
        public bool IsEndParagraph;
        public bool IsSelection;
        public int Length { get { return IsEndParagraph ? 1 : Text.Length; } }
    }
}
