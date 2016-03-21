using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Logic.Commands
{
    abstract class AbstractCommand
    {
        protected Text text;

        protected int cursorX;

        protected int cursorY;

        public void SetParameters(Text text, TextCursor cursor)
        {
            this.text = text;
            cursorX = cursor.x;
            cursorY = cursor.y;
        }

        public AbstractCommand execute()
        {
            executeAtomic();

            if (stacksWith(nextLink))
            {
                return nextLink.execute();
            }

            return this;
        }

        public AbstractCommand undo()
        {
            undoAtomic();

            if (stacksWith(prevLink))
            {
                return prevLink.undo();
            }

            return this;
        }

        public abstract bool isExecutable();
        
        public AbstractCommand nextLink = null;

        public AbstractCommand prevLink = null;

        protected abstract bool stacksWith(AbstractCommand command);

        protected abstract void executeAtomic();

        protected abstract void undoAtomic();
    }
}
