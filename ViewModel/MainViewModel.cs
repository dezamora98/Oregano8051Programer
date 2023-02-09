using AlarmViewProyect;
using Microsoft.Win32;
using Oregano8051Programer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Oregano8051Programer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties
        public Dictionary<string, Parity> StrParity { get; } = new()
        {
            {"None", Parity.None },
            {"Mark",Parity.Mark},
            {"Odd",Parity.Odd},
            {"Space",Parity.Space}
        };

        public Dictionary<string, StopBits> StrStopBits { get; } = new()
        {
            { "1", StopBits.One },
            { "2", StopBits.Two },
            { "None", StopBits.None },
            { "1.5", StopBits.OnePointFive }
        };

        public ObservableCollection<int> Baud { get; } = new() { 1200, 2400, 4800, 9600, 14400, 19200, 57600, 115200 };
        public ObservableCollection<int> NumbBits { get; } = new() { 4, 5, 6, 7, 8 };
        public string? HexFileName { get; set; }
        public ObservableCollection<string>? UartPortList { get; private set; }
        public string PortSelect { get; set; }
        public int SpeedSelect { get; set; }
        public int DataSizeSelect { get; set; }
        public string ParitySelect { get; set; }
        public string StopBitSelect { get; set; }
        public string FlowCtrlSelect { get; set; }

        private SerialPort? serialPort;

        public bool CanPrograming => !(HexFileName is "" or null);

        public byte ByteSend;

        public string DataSend { get; set; }

        public ObservableCollection<UartFrameData> HistoryDataSent { get; private set; }
        public static ObservableCollection<UartFrameData> ReceivedDataSent { get; private set; }

        #endregion

        #region Commands

        private RelayCommand? setUartConf;
        public ICommand SetUartConf => setUartConf ??= new RelayCommand(PerformSetUartConf);

        private RelayCommand? loadHexFileCommand;
        public ICommand LoadHexFileCommand => loadHexFileCommand ??= new RelayCommand(LoadHexFile);

        private RelayCommand programingCommand;
        public ICommand ProgramingCommand => programingCommand ??= new RelayCommand(Programing,x => CanPrograming);

        private RelayCommand defaultUartConfCommand;
        public ICommand DefaultUartConfCommand => defaultUartConfCommand ??= new RelayCommand(DefaultUartConf);

        private RelayCommand sendByteCommand;
        public ICommand SendByteCommand => sendByteCommand ??= new RelayCommand(SendByte);

        #endregion

        #region Methods

        public MainViewModel()
        {
            DefaultUartConf();
            HistoryDataSent = new();
            ReceivedDataSent = new();
            serialPort = new SerialPort();
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
            try
            {
                serialPort.PortName = PortSelect;
                serialPort.DataBits = DataSizeSelect;
                serialPort.BaudRate = SpeedSelect;

                serialPort.Parity = StrParity[ParitySelect];
                serialPort.StopBits = StrStopBits[StopBitSelect];
                serialPort.DataReceived += DataReceivedHandler;
                serialPort.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Programing(object commandParameter)
        {
        }

        private void DefaultUartConf(object commandParameter)
        {
            DefaultUartConf();
        }
        private void DefaultUartConf()
        {
            UartPortList = new(SerialPort.GetPortNames());
            DataSizeSelect = 8;
            SpeedSelect = 9600;
            ParitySelect = "None";
            StopBitSelect = "1";
            PortSelect = UartPortList[0];

            OnPropertyChanged(nameof(DataSizeSelect));
            OnPropertyChanged(nameof(SpeedSelect));
            OnPropertyChanged(nameof(ParitySelect));
            OnPropertyChanged(nameof(StopBitSelect));
            OnPropertyChanged(nameof(PortSelect));
        }


        private void SendByte(object commandParameter)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("Configure the serial port by pressing " +
                                "the set button.", "Serial port not configured",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            if (!CheckDataSend(DataSend))
            {
                MessageBox.Show("The data was not sent because of incorrect numerical format," +
                                "\r\nthe available formats are:\r\n\ndecimal: <number> \r\nBinary: " +
                                "0b<number>\r\nHexadecimal: 0x<number>",
                                "Data not sent", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            byte[] TxBuffer = {ByteSend};
            
            try 
            {
                serialPort.Write(TxBuffer, 0, 1);
                HistoryDataSent.Add(new UartFrameData()
                {
                    Bin = Convert.ToString(ByteSend, 2),
                    Hex = ByteSend.ToString("x"),
                    ASCII = Convert.ToChar(ByteSend).ToString(),
                    N = HistoryDataSent.Count() + 1
                });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CheckDataSend(string data)
        {
            if (DataSend != null)
            {
                if (data.Length > 2)
                {
                    if (data[..2] == "0x" && data.Length < 5)
                    {
                        ByteSend = Convert.ToByte(data[2..], 16);
                        return true;
                    }
                    if (data[..2] == "0b" && data.Length < 11)
                    {
                        ByteSend = Convert.ToByte(data[2..], 2);
                        return true;
                    }
                }
                return byte.TryParse(data, out ByteSend);
            }

            return false;
        }

        #endregion

        #region Events
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort spL = (SerialPort)sender;
            byte[] buf = new byte[spL.BytesToRead];
            spL.Read(buf, 0, buf.Length); 

            foreach (byte b in buf)
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    ReceivedDataSent.Add(new UartFrameData()
                    {
                        Bin = Convert.ToString(b, 2),
                        Hex = b.ToString("x"),
                        ASCII = Convert.ToChar(b).ToString(),
                        N = ReceivedDataSent.Count + 1
                    });
                });

            }
        }
        #endregion
    }
}
