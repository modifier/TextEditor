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
        private string fileContent = "(9+5) * 638 / (764 - (276 + 15))";

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
            var content = OpenFile("Open Highlight Scheme");

            if (content != null)
            {
                textEditor.SetHighlight(content);
            }
        }

        private void openGrammar_Click(object sender, RoutedEventArgs e)
        {
            var content = OpenFile("Open Grammar File");

            if (content != null)
            {
                textEditor.SetGrammar(content);
            }
        }

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            var content = OpenFile("Open File");
            
            if (content != null)
            {
                textEditor.OpenContent(content);
            }
        }

        private string OpenFile(string title)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = title;

            bool? userClickedOk = openFileDialog.ShowDialog();

            if (userClickedOk != true)
            {
                return null;
            }

            return openFileDialog.FileName;
        }

        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save File As";
            bool? userClickedOk = saveFileDialog.ShowDialog();

            if (userClickedOk != true)
            {
                return;
            }

            textEditor.SaveContent(saveFileDialog.FileName);
        }
    }
}
