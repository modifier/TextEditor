using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Logic
{
    class TextSelection
    {
        private TextCursor cursor1;

        private TextCursor cursor2;

        private bool supressCursorEvents = false;

        private Text text;

        public event EventHandler selectionChanged;

        public bool initialized
        {
            get;

            private set;
        }

        public TextSelection(TextCursor cursor)
        {
            cursor2 = cursor;
            cursor2.positionChanged += delegate { RaiseSelectionChanged(true); };
        }

        public void RaiseSelectionChanged(bool cursorEvent)
        {
            if (supressCursorEvents && cursorEvent)
            {
                return;
            }

            selectionChanged(this, new EventArgs());
        }

        public void initSelection(Text text)
        {
            supressCursorEvents = true;
            this.text = text;

            cursor1 = new TextCursor();
            cursor1.positionChanged += delegate { RaiseSelectionChanged(true); };

            cursor1.setText(text);
            cursor1.y = cursor2.y;
            cursor1.x = cursor2.x;

            initialized = true;
            supressCursorEvents = false;
        }

        public void selectAll(Text text, TextCursor cursor)
        {
            this.text = text;
            cursor1 = new TextCursor();
            cursor1.setText(text);
            cursor1.y = 0;
            cursor1.x = 0;

            cursor2 = cursor;
            cursor2.endY();
            cursor2.endX();
            initialized = true;

            supressCursorEvents = true;
            RaiseSelectionChanged(false);
            supressCursorEvents = false;
        }

        public bool selectionExists()
        {
            return initialized && (cursor1.x != cursor2.x || cursor1.y != cursor2.y);
        }

        public void deinitSelection()
        {
            initialized = false;

            RaiseSelectionChanged(false);
        }

        public string getSelectedText()
        {
            TextCursor leftCursor = getLeftCursor(),
                rightCursor = getRightCursor();

            if (leftCursor.y == rightCursor.y)
            {
                return text.text[leftCursor.y].Substring(leftCursor.x, rightCursor.x - leftCursor.x);
            }

            List<string> lines = new List<string>();
            lines.Add(text.text[leftCursor.y].Substring(leftCursor.x));

            for (int y = leftCursor.y + 1; y < rightCursor.y; y++)
            {
                lines.Add(text.text[y]);
            }

            lines.Add(text.text[rightCursor.y].Substring(0, rightCursor.x));

            return String.Join("\n", lines);
        }

        public TextCursor getLeftCursor()
        {
            if (cursor1.y != cursor2.y)
            {
                return cursor1.y < cursor2.y ? cursor1 : cursor2;
            }

            return cursor1.x < cursor2.x ? cursor1 : cursor2;
        }

        public TextCursor getRightCursor()
        {
            return getLeftCursor() == cursor1 ? cursor2 : cursor1;
        }
    }
}
