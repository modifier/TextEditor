using System;
using System.Collections.Generic;

namespace TextEditor.Logic
{
    class Text
    {
        public event EventHandler textChanged;

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

        public void RaiseTextChanged()
        {
            textChanged(this, new EventArgs());
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
                RaiseTextChanged();

                cursor.y = cursorY - 1;
                cursor.x = previousLine.Length;

                return "\n";
            }
            else
            {
                string cutLetter = currentLine[cursorX - 1].ToString();
                text[cursorY] = currentLine.Remove(cursorX - 1, 1);
                RaiseTextChanged();

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
                RaiseTextChanged();
            }
            else
            {
                text[cursorY] = currentLine.Remove(cursorX);
                RaiseTextChanged();
            }
        }

        public void returnCaret(int cursorX, int cursorY)
        {
            string currentLine = text[cursorY];

            string lineBefore = currentLine.Substring(0, cursorX);
            string lineAfter = currentLine.Substring(cursorX);

            text[cursorY] = lineBefore;
            text.Insert(cursorY + 1, lineAfter);
            RaiseTextChanged();

            cursor.y = cursorY + 1;
            cursor.x = 0;
        }

        public void addChar(string ch, int cursorX, int cursorY)
        {
            string textLine = text[cursorY];

            text[cursorY] = textLine.Insert(cursorX, ch);
            RaiseTextChanged();

            cursor.y = cursorY;
            cursor.x = cursorX + 1;
        }

        public void insertText(int cursorX, int cursorY, string selectionText)
        {
            string[] lines = selectionText.Split('\n');

            if (lines.Length == 1)
            {
                text[cursorY] = text[cursorY].Insert(cursorX, lines[0]);
                RaiseTextChanged();

                return;
            }

            int verticalCursor = cursorY;

            text[verticalCursor] = text[verticalCursor] + lines[0];

            for (int i = 1; i < lines.Length - 1; i++)
            {
                verticalCursor++;
                text.Insert(verticalCursor, lines[i]);
            }

            verticalCursor++;
            string lastPart = text.Count < verticalCursor ? text[verticalCursor] : "";
            text.Insert(verticalCursor, lines[lines.Length - 1] + lastPart);
            RaiseTextChanged();
        }

        public void deleteText(int cursor1X, int cursor1Y, int cursor2X, int cursor2Y)
        {
            if (cursor1Y == cursor2Y)
            {
                string line = text[cursor1Y],
                    leftPart = line.Substring(0, cursor1X),
                    rightPart = line.Substring(cursor2X);

                text[cursor1Y] = leftPart + rightPart;
                RaiseTextChanged();

                return;
            }

            text[cursor1Y] = text[cursor1Y].Substring(0, cursor1X);

            for (int y = cursor1Y + 1; y <= cursor2Y - 1; y++)
            {
                text.RemoveAt(cursor1Y + 1);
            }

            text[cursor1Y + 1] = text[cursor1Y + 1].Substring(cursor2X);
            RaiseTextChanged();

            cursor.y = cursor1Y;
            cursor.x = cursor1X;
        }
    }
}
