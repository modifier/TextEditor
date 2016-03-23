using System.Collections.Generic;

namespace TextEditor.Logic
{
    class Text
    {
        public List<string> text
        {
            get;

            private set;
        }

        private TextCursor cursor;

        public Text(TextCursor cursor)
        {
            text = new List<string>() { "" };
            this.cursor = cursor;
        }

        public string removePreviousLetter(int cursorX, int cursorY)
        {
            if (cursorX == 0 && cursorY == 0)
            {
                return null;
            }

            string currentLine = text[cursorY];

            if (cursorX == 0)
            {
                string previousLine = text[cursorY - 1];
                text.RemoveAt(cursorY);
                text[cursorY - 1] = previousLine + currentLine;

                cursor.y = cursorY - 1;
                cursor.x = previousLine.Length;

                return "\n";
            }
            else
            {
                string cutLetter = currentLine[cursorX - 1].ToString();
                text[cursorY] = currentLine.Remove(cursorX - 1);

                cursor.y = cursorY;
                cursor.x = cursorX - 1;

                return cutLetter;
            }
        }

        public void removeNextLetter(int cursorX, int cursorY)
        {
            // todo: TBD

            string currentLine = text[cursorY];

            if (cursorY == text.Count && cursorX == currentLine.Length)
            {
                return;
            }

            if (cursorX == currentLine.Length)
            {
                string nextLine = text[cursorY + 1];
                text.RemoveAt(cursorY + 1);
                text[cursorY + 1] = currentLine + nextLine;
            }
            else
            {
                text[cursorY] = currentLine.Remove(cursorX);
            }
        }

        public void returnCaret(int cursorX, int cursorY)
        {
            string currentLine = text[cursorY];

            string lineBefore = currentLine.Substring(0, cursorX);
            string lineAfter = currentLine.Substring(cursorX);

            text[cursorY] = lineBefore;
            text.Insert(cursorY + 1, lineAfter);

            cursor.x = 0;
            cursor.y = cursorY + 1;
        }

        public void addChar(string ch, int cursorX, int cursorY)
        {
            string textLine = text[cursorY];

            text[cursorY] = textLine.Insert(cursorX, ch);

            cursor.x = cursorX + 1;
            cursor.y = cursorY;
        }
    }
}
