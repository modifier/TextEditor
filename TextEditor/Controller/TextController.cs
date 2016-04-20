﻿using Portable.Parser;
using Portable.Parser.Grammar;
using System.Collections.Generic;
using System.Linq;
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

        private TextSelection selection = new TextSelection();

        private TextEditorRenderer renderer;

        private AbstractCommand executed;

        private AbstractCommand undoed;

        private ParserFacade parser = new ParserFacade();

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

            if (ShiftPressed() && isArrowKey(key) && !selection.initialized)
            {
                selection.initSelection(text, cursor);
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

                if (selection.initialized)
                {
                    if (!ShiftPressed())
                    {
                        selection.deinitSelection();
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
                else if (key == Key.C)
                {
                    copySelection();
                }
                else if (key == Key.V)
                {
                    insertSelection();
                }
                else if (key == Key.A)
                {
                    selectAll();
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
            else if (key == Key.Back && !selection.initialized)
            {
                command = new RemoveCharCommand();
            }
            else if ((key == Key.Back || key == Key.Delete) && selection.initialized)
            {
                command = new ClearSelectionCommand(selection);

                selection.deinitSelection();
            }
            else if (key == Key.Return)
            {
                command = new ReturnCaretCommand();
            }

            if (command != null)
            {
                executeCommand(command);
                renderer.DisplayText(transformText(text.text));
                displayCursor();
            }
        }

        // http://stackoverflow.com/questions/2750576/subscript-superscript-in-formattedtext-class
        // https://msdn.microsoft.com/ru-ru/library/ms754036%28v=vs.100%29.aspx

        private List<CustomTextRun> transformText(List<string> text)
        {
            return parser.getTransformedText(text, selection);
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
            if (command == null)
            {
                return;
            }

            command.SetParameters(text, cursor);

            if (!command.isExecutable())
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
            displayCursor();
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
            displayCursor();
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
                selection.deinitSelection();
                selectionChanged = true;
            }
            else if (!selection.initialized)
            {
                selection.initSelection(text, cursor);
            }

            int currentHit = renderer.getCurrentHit(point);
            
            cursor.setHitPosition(currentHit);

            if (selection.initialized || selectionChanged)
            {
                renderer.DisplayText(transformText(text.text));
            }

            displayCursor();
        }

        private void copySelection()
        {
            if (!selection.selectionExists())
            {
                return;
            }

            Clipboard.SetText(selection.getSelectedText());
        }

        private void insertSelection()
        {
            var data = Clipboard.GetText();

            if (data == null)
            {
                return;
            }

            if (selection.initialized)
            {
                executeCommand(new ClearSelectionCommand(selection));

                selection.deinitSelection();
            }

            var command = new InsertTextCommand(selection, data);
            executeCommand(command);

            renderer.DisplayText(transformText(text.text));
            displayCursor();
        }

        private void selectAll()
        {
            selection.selectAll(text, cursor);

            renderer.DisplayText(transformText(text.text));
            displayCursor();
        }
    }
}
