﻿using System;
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

        public TextSelection(Text text, TextCursor cursor)
        {
            cursor1 = new TextCursor();
            cursor1.setText(text);
            cursor1.x = cursor.x;
            cursor1.y = cursor.y;

            cursor2 = cursor;
        }

        public bool selectionExists()
        {
            return cursor1.x != cursor2.x || cursor1.y != cursor2.y;
        }

        public string getSelectedText()
        {
            // todo

            return "";
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