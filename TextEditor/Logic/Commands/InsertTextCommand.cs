using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Logic.Commands
{
    class InsertTextCommand : AbstractCommand
    {
        private TextSelection selection;

        private string selectionText;

        public InsertTextCommand(TextSelection selection, string text)
        {
            this.selection = selection;
            this.selectionText = text;
        }

        public override bool isExecutable()
        {
            return true;
        }

        protected override void executeAtomic()
        {
            string[] lines = selectionText.Split('\n');

            if (lines.Length == 1)
            {
                text.text[cursorY] = text.text[cursorY].Insert(cursorX, lines[0]);

                return;
            }

            string leftPart = text.text[cursorY].Substring(0, cursorX),
                rightPart = text.text[cursorY].Substring(cursorX);

            int verticalCursor = cursorY;

            text.text[verticalCursor] = leftPart + lines[0];

            for (int i = 1; i < lines.Length - 1; i++)
            {
                text.text.Insert(verticalCursor, lines[i]);
            }

            text.text[verticalCursor] = lines[lines.Length - 1] + rightPart;
        }

        protected override bool stacksWith(AbstractCommand command)
        {
            return false;
        }

        protected override void undoAtomic()
        {
            throw new NotImplementedException();
        }
    }
}
