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

        public ClearSelectionCommand(TextSelection selection)
        {
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
            if (cursor1Y == cursor2Y)
            {
                string line = text.text[cursor1Y],
                    leftPart = line.Substring(0, cursor1X),
                    rightPart = line.Substring(cursor2X);

                text.text[cursor1Y] = leftPart + rightPart;

                return;
            }

            text.text[cursor1Y] = text.text[cursor1Y].Substring(0, cursor1X);

            for (int y = cursor1Y + 1; y <= cursor2Y - 1; y++)
            {
                text.text.RemoveAt(y);
            }

            text.text[cursor1Y + 1] = text.text[cursor1Y + 1].Substring(cursor2X);
        }

        protected override bool stacksWith(AbstractCommand command)
        {
            return false;
        }

        protected override void undoAtomic()
        {
            
        }
    }
}
