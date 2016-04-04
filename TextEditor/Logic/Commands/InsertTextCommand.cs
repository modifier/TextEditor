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
            text.insertText(cursorX, cursorY, selectionText);
        }

        protected override bool stacksWith(AbstractCommand command)
        {
            return false;
        }

        protected override void undoAtomic()
        {
            // todo
        }
    }
}
