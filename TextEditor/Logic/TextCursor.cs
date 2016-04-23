using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Logic
{
    class TextCursor
    {
        private int innerX = 0;

        private int innerY = 0;

        private Text text = null;

        public event EventHandler positionChanged;

        public void setText(Text text)
        {
            this.text = text;
        }

        private void RaisePositionChanged()
        {
            positionChanged(this, new EventArgs());
        }

        public int Clamp(int val, int minVal, int maxVal)
        {
            return val > maxVal ? maxVal : val < minVal ? minVal : val;
        }

        public int x
        {
            get
            {
                return innerX;
            }
            set
            {
                int newX = Clamp(value, 0, text.text[y].Length);

                if (newX != innerX)
                {
                    innerX = newX;

                    RaisePositionChanged();
                }
            }
        }

        public int y
        {
            get
            {
                return innerY;
            }
            set
            {
                int newY = Clamp(value, 0, text.text.Count - 1);

                if (newY != innerY)
                {
                    innerY = newY;
                    innerX = Clamp(innerX, 0, text.text[y].Length);

                    RaisePositionChanged();
                }
            }
        }

        public void endX()
        {
            x = text.text[y].Length;
        }

        public void endY()
        {
            y = text.text.Count - 1;
        }

        public void incX()
        {
            int lineLength = text.text[y].Length;

            if (x == lineLength && y == text.text.Count - 1)
            {
                return;
            }

            if (x == lineLength)
            {
                y++;
                x = 0;
            }
            else
            {
                x++;
            }
        }

        public void decX()
        {
            if (x == 0 && y == 0)
            {
                return;
            }

            if (x == 0)
            {
                y--;
            }
            else
            {
                x--;
            }
        }

        public void incY()
        {
            y++;
        }

        public void decY()
        {
            y--;
        }

        public int getHitPosition()
        {
            return text.getHitPosition(x, y - 1);
        }

        public int getHitPositionForLine(int line)
        {
            return text.getHitPosition(0, line);
        }

        public void setHitPosition(int hitPosition)
        {
            for (int i = 0; i <= y; i++)
            {
                if (hitPosition < text.text[i].Length)
                {
                    y = i;
                    x = hitPosition;

                    return;
                }

                hitPosition -= text.text[i].Length + 1;
            }

            endY();
            x = hitPosition;
        }
    }
}
