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
using TextEditor.Logic;
using System;

namespace TextEditor.Visual
{
    class TextEditorRenderer
    {
        private DrawingGroup dg;
        private Rectangle Rectus;
        private HightlightScheme scheme;
        private Line cursor = null;
        private List<Rectangle> selectionRectangles = new List<Rectangle>();
        private Canvas surface;
        private List<TextLine> cachedLines = new List<TextLine>();
        public event EventHandler highlightSchemeUpdated;

        public TextEditorRenderer(DrawingGroup dg, Rectangle rect, Canvas surface, HightlightScheme scheme)
        {
            this.dg = dg;
            this.Rectus = rect;
            this.surface = surface;
            this.scheme = scheme;
        }

        public void SetHightlightScheme(HightlightScheme scheme)
        {
            this.scheme = scheme;

            highlightSchemeUpdated(this, new EventArgs());
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
                (int) scheme.GetDefaultConfiguration().TextHeight,
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

            surface.Width = rectDimensions[0];
            surface.Height = rectDimensions[1];
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

        public void DisplaySelection(TextSelection selection)
        {
            foreach (Rectangle selectionRect in selectionRectangles)
            {
                surface.Children.Remove(selectionRect);
            }

            selectionRectangles.Clear();

            if (!selection.selectionExists())
            {
                return;
            }

            TextCursor leftCursor = selection.getLeftCursor(),
                rightCursor = selection.getRightCursor();

            double leftPosition = cachedLines[leftCursor.y].GetDistanceFromCharacterHit(new CharacterHit(leftCursor.getHitPosition(), 0)),
                rightPosition = cachedLines[rightCursor.y].GetDistanceFromCharacterHit(new CharacterHit(rightCursor.getHitPosition(), 0));

            
            for (int i = leftCursor.y; i <= rightCursor.y; i++)
            {
                int positionLeft = 0,
                    positionRight = 0;

                if (leftCursor.y == i)
                {
                    positionLeft = leftCursor.getHitPosition();
                }
                else
                {
                    positionLeft = leftCursor.getHitPositionForLine(i - 1);
                }

                if (rightCursor.y == i)
                {
                    positionRight = rightCursor.getHitPosition();
                }
                else
                {
                    positionRight = rightCursor.getHitPositionForLine(i) - 1;
                }

                AddSelectionRectangle(i, positionLeft, positionRight);
            }
        }

        private void AddSelectionRectangle(int line, int positionLeft, int positionRight)
        {
            double y1 = 0;
            for (int i = 0; i < line; i++)
            {
                y1 += cachedLines[i].Height;
            }

            double y2 = y1 + cachedLines[line].Height;

            double leftPosition = cachedLines[line].GetDistanceFromCharacterHit(new CharacterHit(positionLeft, 0)),
                rightPosition = cachedLines[line].GetDistanceFromCharacterHit(new CharacterHit(positionRight, 0));

            Rectangle rect = new Rectangle();
            rect.Fill = Brushes.LightBlue;
            rect.Width = rightPosition - leftPosition;
            rect.Height = y2 - y1;

            surface.Children.Insert(0, rect);
            Canvas.SetTop(rect, y1);
            Canvas.SetLeft(rect, leftPosition);

            selectionRectangles.Add(rect);
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
