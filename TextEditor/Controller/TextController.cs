using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private TextEditorRenderer renderer;

        private AbstractCommand executed;

        private AbstractCommand undoed;

        public TextController(TextEditorRenderer renderer)
        {
            this.renderer = renderer;
            text = new Text(cursor);
        }

        public void keyPress(Key key)
        {
            AbstractCommand command = null;

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
                else if (key == Key.Home)
                {
                    cursor.x = 0;
                    cursor.y = 0;
                }
                else if (key == Key.End)
                {
                    cursor.y = text.text.Count - 1;
                    cursor.x = text.text[cursor.y].Length;
                }

                displayCursor();

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
            else if (isArrowKey(key))
            {
                moveCaret(key);
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

            foreach (var line in text)
            {
                Runs.Add(new CustomTextRun { Text = line });
                Runs.Add(new CustomTextRun { IsEndParagraph = true });
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
                cursor.y = Math.Max(0, cursor.y - 1);
                cursor.x = Math.Min(text.text[cursor.y].Length, cursor.x);
            }
            else if (key == Key.Down)
            {
                cursor.y = Math.Min(text.text.Count - 1, cursor.y + 1);
                cursor.x = Math.Min(text.text[cursor.y].Length, cursor.x);
            }
            else if (key == Key.Home)
            {
                cursor.x = 0;
            }
            else if (key == Key.End)
            {
                cursor.x = text.text[cursor.y].Length;
            }
            else if (key == Key.Left)
            {
                if (cursor.x == 0 && cursor.y == 0)
                {
                    return;
                }

                if (cursor.x == 0)
                {
                    cursor.y--;
                    cursor.x = text.text[cursor.y].Length;
                }
                else
                {
                    cursor.x--;
                }
            }
            else if (key == Key.Right)
            {
                int lineLength = text.text[cursor.y].Length;

                if (cursor.x == lineLength && cursor.y == text.text.Count - 1)
                {
                    return;
                }

                if (cursor.x == lineLength)
                {
                    cursor.y++;
                    cursor.x = 0;
                }
                else
                {
                    cursor.x++;
                }
            }

            displayCursor();
        }

        private void displayCursor()
        {
            int hitPosition = 0;
            for (int i = 0; i < cursor.y; i++)
            {
                hitPosition += text.text[i].Length + 1;
            }

            renderer.PlaceCursor(hitPosition + cursor.x, cursor.y);
        }
    }
}
