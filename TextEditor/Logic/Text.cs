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

        private void addChar(string ch)
        {
            string textLine = text[cursorY];

            text[cursorY] = textLine.Insert(cursorX, ch);

            cursorX++;
        }
    }
}
