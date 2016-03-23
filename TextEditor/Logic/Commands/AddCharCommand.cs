using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TextEditor.Logic.Commands
{
    class AddCharCommand : AbstractCommand
    {
        private string letter;

        public AddCharCommand(Key key)
        {
            letter = KeyTranslator.GetCharFromKey(key).ToString();
        }

        protected override void executeAtomic()
        {
            text.addChar(letter, cursorX, cursorY);
        }

        protected override void undoAtomic()
        {
            text.removePreviousLetter(cursorX + 1, cursorY);
        }

        protected override bool stacksWith(AbstractCommand command)
        {
            return command is AddCharCommand || command is ReturnCaretCommand;
        }

        public override bool isExecutable()
        {
            return true;
        }
    }
}
