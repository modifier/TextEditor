using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TextEditor.Logic;
using TextEditor.Logic.Commands;
using TextEditor.Visual;

namespace TextEditor.Controller
{
    class TextController
    {
        private Text text;

        private TextCursor cursor = new TextCursor();

        private TextSelection selection;

        private TextEditorRenderer renderer;

        private AbstractCommand executed;

        private AbstractCommand undoed;

        public TextController(TextEditorRenderer renderer)
        {
            this.renderer = renderer;
            text = new Text(cursor);
            cursor.setText(text);
        }

        public void keyPress(Key key)
        {
            // todo: refactoring
            AbstractCommand command = null;

            if (ShiftPressed() && isArrowKey(key) && selection == null)
            {
                selection = new TextSelection(text, cursor);
            }

            if (isArrowKey(key))
            {
                if (CtrlPressed() && key == Key.Home)
                {
                    cursor.x = 0;
                    cursor.y = 0;
                }
                else if (CtrlPressed() && key == Key.End)
                {
                    cursor.endY();
                    cursor.endX();
                }
                else
                {
                    moveCaret(key);
                }

                if (selection != null)
                {
                    if (!ShiftPressed())
                    {
                        selection = null;
                    }

                    renderer.DisplayText(transformText(text.text));
                }

                displayCursor();

                return;
            }

            if (CtrlPressed())
            {
                if (key == Key.Z)
                {
                    undo();
                }
                else if (key == Key.Y)
                {
                    redo();
                }

                return;
            }

            if (ModifierKeyPressed())
            {
                return;
            }

            if (isTextKey(key))
            {
                command = new AddCharCommand(key);
            }
            else if (key == Key.Back)
            {
                command = new RemoveCharCommand();
            }
            else if (key == Key.Return)
            {
                command = new ReturnCaretCommand();
            }

            if (command != null)
            {
                command.SetParameters(text, cursor);
                executeCommand(command);
                renderer.DisplayText(transformText(text.text));
                displayCursor();
            }
        }

        // http://stackoverflow.com/questions/2750576/subscript-superscript-in-formattedtext-class
        // https://msdn.microsoft.com/ru-ru/library/ms754036%28v=vs.100%29.aspx

        private List<CustomTextRun> transformText(List<string> text)
        {
            List<CustomTextRun> Runs = new List<CustomTextRun>();

            int i = 0;
            bool selectionRun = false;
            TextCursor leftSelectionCursor = null,
                rightSelectionCursor = null;

            if (selection != null && selection.selectionExists())
            {
                leftSelectionCursor = selection.getLeftCursor();
                rightSelectionCursor = selection.getRightCursor();
            }

            foreach (var line in text)
            {
                // todo: refactoring

                if (leftSelectionCursor != null && leftSelectionCursor.y == i && rightSelectionCursor.y == i)
                {
                    string leftPart = line.Substring(0, leftSelectionCursor.x),
                        middlePart = line.Substring(leftSelectionCursor.x, rightSelectionCursor.x - leftSelectionCursor.x),
                        rightPart = line.Substring(rightSelectionCursor.x);

                    if (leftPart.Length != 0)
                    {
                        Runs.Add(new CustomTextRun { Text = leftPart });
                    }

                    Runs.Add(new CustomTextRun { Text = middlePart, IsSelection = true });

                    if (rightPart.Length != 0)
                    {
                        Runs.Add(new CustomTextRun { Text = rightPart });
                    }
                }
                else if (leftSelectionCursor != null && leftSelectionCursor.y == i)
                {
                    string leftPart = line.Substring(0, leftSelectionCursor.x),
                        rightPart = line.Substring(leftSelectionCursor.x);

                    if (leftPart.Length != 0)
                    {
                        Runs.Add(new CustomTextRun { Text = leftPart });
                    }

                    Runs.Add(new CustomTextRun { Text = rightPart, IsSelection = true });
                    selectionRun = true;
                }
                else if (selectionRun && rightSelectionCursor.y == i)
                {
                    string leftPart = line.Substring(0, rightSelectionCursor.x),
                        rightPart = line.Substring(rightSelectionCursor.x);

                    if (leftPart.Length != 0)
                    {
                        Runs.Add(new CustomTextRun { Text = leftPart, IsSelection = true });
                    }

                    Runs.Add(new CustomTextRun { Text = rightPart });
                    selectionRun = false;
                }
                else if (selectionRun)
                {
                    Runs.Add(new CustomTextRun { Text = line, IsSelection = true });
                }
                else
                {
                    Runs.Add(new CustomTextRun { Text = line });
                }
                
                Runs.Add(new CustomTextRun { IsEndParagraph = true });
                i++;
            }

            return Runs;
        }

        private Key[] textKeys = {
            Key.OemQuestion,
            Key.OemQuotes,
            Key.OemPlus,
            Key.OemOpenBrackets,
            Key.OemCloseBrackets,
            Key.OemMinus,
            Key.DeadCharProcessed,
            Key.Oem1,
            Key.Oem3,
            Key.Oem5,
            Key.Oem7,
            Key.OemPeriod,
            Key.OemComma,
            Key.OemMinus,
            Key.Add,
            Key.Divide,
            Key.Multiply,
            Key.Subtract,
            Key.Oem102,
            Key.Decimal,
            Key.Tab,
            Key.Space
        };

        private bool isTextKey(Key key)
        {
            return (key >= Key.A && key <= Key.Z) || (key >= Key.D0 && key <= Key.D9)
                || (key >= Key.NumPad0 && key <= Key.NumPad9) || textKeys.Contains(key);
        }

        private bool isArrowKey(Key key)
        {
            return key == Key.Up || key == Key.Down || key == Key.Right || key == Key.Left || key == Key.Home || key == Key.End;
        }

        private bool ModifierKeyPressed()
        {
            return CtrlPressed() || AltPressed();
        }

        private bool CtrlPressed()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

        private bool AltPressed()
        {
            return Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
        }

        private bool ShiftPressed()
        {
            return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        private void executeCommand(AbstractCommand command)
        {
            if (command == null || !command.isExecutable())
            {
                return;
            }

            command.execute();

            if (executed != null)
            {
                executed.nextLink = command;
                command.prevLink = executed;
            }

            executed = command;
            undoed = null;
        }

        private void undo()
        {
            if (executed == null)
            {
                return;
            }

            AbstractCommand previous = executed.undo();
            executed = previous.prevLink;
            undoed = previous;

            renderer.DisplayText(transformText(text.text));
        }

        private void redo()
        {
            if (undoed == null)
            {
                return;
            }

            AbstractCommand next = undoed.execute();
            executed = next;
            undoed = next.nextLink;

            renderer.DisplayText(transformText(text.text));
        }

        private void moveCaret(Key key)
        {
            if (key == Key.Up)
            {
                cursor.decY();
            }
            else if (key == Key.Down)
            {
                cursor.incY();
            }
            else if (key == Key.Home)
            {
                cursor.x = 0;
            }
            else if (key == Key.End)
            {
                cursor.endX();
            }
            else if (key == Key.Left)
            {
                cursor.decX();
            }
            else if (key == Key.Right)
            {
                cursor.incX();
            }

            displayCursor();
        }

        private void displayCursor()
        {
            renderer.PlaceCursor(cursor.getHitPosition(), cursor.y);
        }

        public void setCursorFromPoint(Point point)
        {
            bool selectionChanged = false;

            if (!ShiftPressed())
            {
                selection = null;
                selectionChanged = true;
            }
            else if (selection == null)
            {
                selection = new TextSelection(text, cursor);
            }

            int currentHit = renderer.getCurrentHit(point);
            
            cursor.setHitPosition(currentHit);

            if (selection != null || selectionChanged)
            {
                renderer.DisplayText(transformText(text.text));
            }

            displayCursor();
        }
    }
}
