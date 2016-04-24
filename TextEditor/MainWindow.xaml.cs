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

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            bool? userClickedOk = openFileDialog.ShowDialog();

            if (userClickedOk != true)
            {
                return;
            }

            var content =  File.ReadAllText(openFileDialog.FileName);
            textEditor.SetContent(content);
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

        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save File As";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName == "")
            {
                return;
            }

            File.WriteAllText(saveFileDialog.FileName, textEditor.GetContent());
        }
    }
}
