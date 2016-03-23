using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Logic.Commands
{
    class ReturnCaretCommand : AbstractCommand
    {
        public override bool isExecutable()
        {
            return true;
        }

        protected override void executeAtomic()
        {
            text.returnCaret(cursorX, cursorY);
        }

        protected override bool stacksWith(AbstractCommand command)
        {
            return command is AddCharCommand || command is ReturnCaretCommand;
        }

        protected override void undoAtomic()
        {
            text.removePreviousLetter(0, cursorY + 1);
        }
    }
}
