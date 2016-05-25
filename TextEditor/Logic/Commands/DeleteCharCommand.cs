using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Logic.Commands
{
    class DeleteCharCommand : RemoveCharCommand
    {
        private string letter;

        protected override void executeAtomic()
        {
            letter = text.removeNextLetter(cursorX, cursorY);
        }

        protected override bool stacksWith(AbstractCommand command)
        {
            return command is RemoveCharCommand || command is DeleteCharCommand;
        }

        protected override void undoAtomic()
        {
            if (letter == "\n")
            {
                text.returnCaret(cursorX, cursorY);
            }
            else
            {
                text.addChar(letter, cursorX, cursorY);
            }
        }

        public override bool isExecutable()
        {
            return !(cursorY == text.text.Count - 1 && cursorX == text.text[text.text.Count - 1].Length);
        }
    }
}
