using AlarmViewProyect;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfHexaEditor;

namespace Oregano8051Programer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        private readonly Dictionary<string, Parity> strParity = new()
        {
            { "None", Parity.None },
            {"Mark",Parity.Mark},
            {"Odd",Parity.Odd},
            {"Space",Parity.Space}
        };

        private readonly Dictionary<string, StopBits> strStopBits = new()
        {
            { "1", StopBits.One },
            { "2", StopBits.Two },
            { "None", StopBits.None },
            { "1.5", StopBits.OnePointFive }
        };


        #region Properties

        public string? HexFileName { get; set; }
        public ObservableCollection<string>? UartPortList { get; private set; }
        public string? PortSelect { get; set; }
        public int SpeedSelect { get; set; }
        public int DataSizeSelect { get; set; }
        public string? ParitySelect { get; set; }
        public string? StopBitSelect { get; set; }
        public string? FlowCtrlSelect { get; set; }

        private SerialPort? serialPort;

        #endregion

        #region Commands

        private RelayCommand? setUartConf;
        public ICommand SetUartConf => setUartConf ??= new RelayCommand(PerformSetUartConf);

        private RelayCommand? loadHexFileCommand;
        public ICommand LoadHexFileCommand => loadHexFileCommand ??= new RelayCommand(LoadHexFile);

        private RelayCommand programingCommand;
        public ICommand ProgramingCommand => programingCommand ??= new RelayCommand(Programing);


        #endregion

        #region Methods

        public MainViewModel()
        {
            UartPortList = new(SerialPort.GetPortNames());
        }
        private void LoadHexFile(object commandParameter)
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
                HexFileName = dialog.FileName;
                //_intelHexFile = new IntelHexFile(dialog.FileName,"00");
                OnPropertyChanged(nameof(HexFileName));
            }

        }
        private void PerformSetUartConf(object commandParameter)
        {
            serialPort = new SerialPort();

            try
            {
                serialPort.PortName = PortSelect;
                serialPort.BaudRate = SpeedSelect;
                serialPort.DataBits = DataSizeSelect;

                serialPort.Parity = strParity[ParitySelect];
                serialPort.StopBits = strStopBits[StopBitSelect];
                serialPort.Open();
            }
            catch (Exception e)
            {
                throw;
            }
        }


        private void Programing(object commandParameter)
        {
        }

        #endregion
    }
}
