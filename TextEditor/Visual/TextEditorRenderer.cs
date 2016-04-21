using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.TextFormatting;
using System.Windows;
using System.Windows.Controls;
using Portable.Parser;
using Portable.Parser.Grammar;
using System.Linq;
using TextEditor.Visual.Hightlight;

namespace TextEditor.Visual
{
    class TextEditorRenderer
    {
        private DrawingGroup dg;
        private Rectangle Rectus;
        private HightlightScheme scheme;
        private Line cursor = null;
        private Canvas surface;
        private List<TextLine> cachedLines = new List<TextLine>();

        public TextEditorRenderer(DrawingGroup dg, Rectangle rect, Canvas surface)
        {
            this.dg = dg;
            this.Rectus = rect;
            this.surface = surface;
        }

        public void SetHightlightScheme(HightlightScheme scheme)
        {
            this.scheme = scheme;
        }

        public void DisplayText(List<CustomTextRun> runs)
        {
            cachedLines.Clear();

            var dc = dg.Open();

            var textSource = new CustomTextSource(runs, scheme);
            var formatter = TextFormatter.Create();
            var properties = new CustomTextParagraphProperties(
                FlowDirection.LeftToRight,
                TextAlignment.Left,
                true,
                false,
                new CustomTextRunProperties(scheme.GetDefaultConfiguration()),
                TextWrapping.NoWrap,
                scheme.GetDefaultConfiguration().TextHeight,
                0
            );
            int textStorePosition = 0;
            Point linePosition = new Point(0, 0);

            double[] rectDimensions = { 0, 0 };

            while (textStorePosition < textSource.Length)
            {
                TextLine line = formatter.FormatLine(textSource, textStorePosition, 5000, properties, null);
                cachedLines.Add(line);

                line.Draw(dc, linePosition, InvertAxes.None);
                textStorePosition += line.Length;
                linePosition.Y += line.Height;

                rectDimensions[0] = line.Width > rectDimensions[0] ? line.Width : rectDimensions[0];
                rectDimensions[1] += line.Height;
            }

            dc.Close();
            Rectus.Width = rectDimensions[0];
            Rectus.Height = rectDimensions[1];
        }

        public void PlaceCursor(int hitPosition, int cursorY)
        {
            if (cursor == null)
            {
                cursor = new Line();
                cursor.Stroke = Brushes.Black;
                cursor.StrokeThickness = 1;
                surface.Children.Add(cursor);
            }

            double y1 = 0;
            for (int i = 0; i < cursorY; i++)
            {
                y1 += cachedLines[i].Height;
            }
            
            var position = cachedLines[cursorY].GetDistanceFromCharacterHit(new CharacterHit(hitPosition, 0));

            cursor.X1 = cursor.X2 = position;
            cursor.Y1 = y1;
            cursor.Y2 = y1 + cachedLines[cursorY].Height;
        }

        public int getCurrentHit(Point point)
        {
            var currentHit = cachedLines[getYFromHeight(point.Y)].GetCharacterHitFromDistance(point.X);

            return currentHit.FirstCharacterIndex;
        }

        private int getYFromHeight(double height)
        {
            for (int i = 0; i < cachedLines.Count; i++)
            {
                if (height < cachedLines[i].Height)
                {
                    return i;
                }

                height -= cachedLines[i].Height;
            }

            return cachedLines.Count - 1;
        }
    }
}
