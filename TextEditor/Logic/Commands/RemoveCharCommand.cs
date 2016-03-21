using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TextEditor.Logic.Commands
{
    class RemoveCharCommand : AbstractCommand
    {
        private string letter;

        protected override void executeAtomic()
        {
            letter = text.removePreviousLetter(cursorX, cursorY);
        }

        protected override bool stacksWith(AbstractCommand command)
        {
            return command is RemoveCharCommand;
        }

        protected override void undoAtomic()
        {
            text.addChar(letter, cursorX - 1, cursorY);
        }

        public override bool isExecutable()
        {
            return !(cursorX == 0 && cursorY == 0);
        }
    }
}
