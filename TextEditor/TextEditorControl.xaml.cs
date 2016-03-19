﻿using System.Collections.Generic;
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

        private string[] text = {
        "The quick brown fox jumps over the lazy dog",
        "Съешь ещё этих мягких францзских булок, да выпей же чаю"
        };

        public TextEditorControl()
        {
            InitializeComponent();

            renderer = new TextEditorRenderer(mainBrush, Rectus);
            renderer.SetConfiguration(new TextEditorConfiguration { FontFamily = "Lucida Console", FontSize = 14, TextHeight = 14, ForegroundColor = Brushes.Black });

            controller = new TextController(renderer);
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsFocused)
            {
                Focus();
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            controller.keyPress(e.Key);
        }
    }
}
