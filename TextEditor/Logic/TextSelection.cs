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

        private Text text;

        public bool initialized
        {
            get;

            private set;
        }

        public void initSelection(Text text, TextCursor cursor)
        {
            this.text = text;

            cursor1 = new TextCursor();
            cursor1.setText(text);
            cursor1.x = cursor.x;
            cursor1.y = cursor.y;

            cursor2 = cursor;
            initialized = true;
        }

        public bool selectionExists()
        {
            return initialized && (cursor1.x != cursor2.x || cursor1.y != cursor2.y);
        }

        public void deinitSelection()
        {
            initialized = false;
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

            lines.Add(text.text[leftCursor.y].Substring(0, rightCursor.x));

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
