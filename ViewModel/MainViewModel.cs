using AlarmViewProyect;
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

        #endregion

        #region Methods
        private void UpdateUartPort(object sender, RoutedEventArgs e)
        {
            UartPortList = new(SerialPort.GetPortNames());
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
                Console.WriteLine("Exception " + e.Message);
                throw;
            }
        }
        #endregion



    }
}
