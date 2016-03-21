using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Logic
{
    class TextCursor
    {
        public int x = 0;

        public int y = 0;

        public TextCursor clone()
        {
            TextCursor c = new TextCursor();

            c.x = x;
            c.y = y;

            return c;
        }
    }
}
