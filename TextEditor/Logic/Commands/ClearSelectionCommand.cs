using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Logic.Commands
{
    class ClearSelectionCommand : AbstractCommand
    {
        private int cursor1X;

        private int cursor1Y;

        private int cursor2X;

        private int cursor2Y;

        private string copiedText;

        private TextSelection selection;

        public ClearSelectionCommand(TextSelection selection)
        {
            this.selection = selection;

            cursor1X = selection.getLeftCursor().x;
            cursor1Y = selection.getLeftCursor().y;
            cursor2X = selection.getRightCursor().x;
            cursor2Y = selection.getRightCursor().y;
            copiedText = selection.getSelectedText();
        }

        public override bool isExecutable()
        {
            return true;
        }

        protected override void executeAtomic()
        {
            text.deleteText(cursor1X, cursor1Y, cursor2X, cursor2Y);
        }

        protected override bool stacksWith(AbstractCommand command)
        {
            return false;
        }

        protected override void undoAtomic()
        {
            text.insertText(cursor1X, cursor1Y, copiedText);
        }
    }
}
