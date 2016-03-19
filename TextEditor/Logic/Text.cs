using System.Collections.Generic;
using System.Windows.Input;

namespace TextEditor.Logic
{
    class Text
    {
        private int cursorX = 0;

        private int cursorY = 0;

        public List<string> text
        {
            get;
            private set;
        }

        public Text()
        {
            text = new List<string>() { "" };
        }

        public void addLetter(Key key)
        {
            addChar(KeyTranslator.GetCharFromKey(key).ToString());
        }

        public void removePreviousLetter()
        {
            if (cursorX == 0 && cursorY == 0)
            {
                return;
            }

            string currentLine = text[cursorY];

            if (cursorX == 0)
            {
                string previousLine = text[cursorY - 1];
                text.RemoveAt(cursorY);
                text[cursorY - 1] = previousLine + currentLine;

                cursorY--;
                cursorX = previousLine.Length;
            }
            else
            {
                text[cursorY] = currentLine.Remove(cursorX - 1);

                cursorX--;
            }
        }

        public void removeNextLetter()
        {
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

        public void returnCaret()
        {
            text.Add("");
            cursorX = 0;
            cursorY++;
        }

        private void addChar(string ch)
        {
            string textLine = text[cursorY];

            text[cursorY] = textLine.Insert(cursorX, ch);

            cursorX++;
        }
    }
}
