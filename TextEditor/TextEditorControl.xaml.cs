﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TextEditor.Controller;
using TextEditor.Visual;
using TextEditor.Visual.Hightlight;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for TextEditorControl.xaml
    /// </summary>
    public partial class TextEditorControl : UserControl
    {
        private TextEditorRenderer renderer;

        private TextController controller;

        private HighlightImporter importer;

        private string grammar = @"[OmitPattern(""[\s]*"")]
[RootRule(expr)]
SimpleArithmetics {

    productOp: '*' | '/';
    sumOp: '+' | '-';

    [RewriteRecursion]
    /*[ExpandRecursion]*/
    #expr: {
        |sum: expr sumOp expr;
        |product: expr productOp expr;
        |[right]power: expr '^' expr;
        |#braces: '(' expr ')';
        |num: ""[0-9]+"";
    };
}";

        private string highlight = @"!default {
	color: #000000;
}

num {
	color: #0000ff;
}

sumOp, productOp {
	color: #008800;
}

braces {
	background: #00ffff;
}

/braces, braces/braces {
	color: #888888;
}";

        public TextEditorControl()
        {
            InitializeComponent();

            importer = new HighlightImporter();
            renderer = new TextEditorRenderer(mainBrush, Rectus, Surface, importer.GetDefaultScheme());

            ParserFacade parser = new ParserFacade();

            controller = new TextController(renderer, parser);
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

        private void ScrollViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            controller.keyPress(e.Key);
        }
    }
}
