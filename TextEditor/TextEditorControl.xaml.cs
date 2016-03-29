using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TextEditor.Controller;
using TextEditor.Visual;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for TextEditorControl.xaml
    /// </summary>
    public partial class TextEditorControl : UserControl
    {
        private TextEditorRenderer renderer;

        private TextController controller;

        public TextEditorControl()
        {
            InitializeComponent();

            renderer = new TextEditorRenderer(mainBrush, Rectus, Surface);
            renderer.SetConfiguration(new TextEditorConfiguration { FontFamily = "Lucida Console", FontSize = 14, TextHeight = 14, ForegroundColor = Brushes.Black });

            controller = new TextController(renderer);
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsFocused)
            {
                Focus();
            }

            var position = e.GetPosition((IInputElement)sender);
            controller.setCursorFromPoint(position);
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            controller.keyPress(e.Key);
        }
    }
}
