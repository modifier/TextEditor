using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            textEditor.Focus();
        }

        private void openHighlight_Click(object sender, RoutedEventArgs e)
        {
            var content = OpenFile();

            if (content != null)
            {
                textEditor.SetHighlight(content);
            }
        }

        private void openGrammar_Click(object sender, RoutedEventArgs e)
        {
            var content = OpenFile();

            if (content != null)
            {
                textEditor.SetGrammar(content);
            }
        }

        private string OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            bool? userClickedOk = openFileDialog.ShowDialog();

            if (userClickedOk != true)
            {
                return null;
            }

            return File.ReadAllText(openFileDialog.FileName);
        }
    }
}
