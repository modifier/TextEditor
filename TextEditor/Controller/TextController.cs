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

        public TextController(TextEditorRenderer renderer)
        {
            this.renderer = renderer;
            text = new Text(cursor);
        }

        public void keyPress(Key key)
        {
            AbstractCommand command = null;

            if (isTextKey(key))
            {
                command = new AddCharCommand(key);
            }
            else if (isArrowKey(key))
            {

            }
            else if (key == Key.Back)
            {
                command = new RemoveCharCommand();
            }
            else if (key == Key.Return)
            {
                //text.returnCaret();
            }

            if (command != null)
            {
                command.SetParameters(text, cursor);
                executeCommand(command);
                renderer.DisplayText(transformText(text.text));
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
            return key == Key.Up || key == Key.Down || key == Key.Right || key == Key.Left;
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
        }
    }
}
