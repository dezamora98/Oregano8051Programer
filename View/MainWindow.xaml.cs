using Microsoft.Win32;
using Oregano8051Programer.Model;
using System.Windows;
using System.IO.Ports;

namespace Oregano8051Programer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string FileNamePath { get; set; }
        private IntelHexFile _intelHexFile;
        private SerialPort _serialPort;


        public MainWindow()
        {
            InitializeComponent();
            FileNamePath = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                FileName = "Alarms", // Default file name
                DefaultExt = ".hex", // Default file extension
                Filter = "Text documents (.hex)|*.hex" // Filter files by extension
            };

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                HexEditorV.FileName = dialog.FileName;
                //_intelHexFile = new IntelHexFile(dialog.FileName,"00");
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
