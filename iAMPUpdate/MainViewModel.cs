using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommLibrary;
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;

namespace iAMPUpdate
{
    public class MainViewModel
    {
        public RelayCommand<object> RadioBtnCommand { get; private set; }

        public RelayCommand OpenFileCommand { get; private set; }

        public RelayCommand TestSerialCommand { get; private set; }

        public RelayCommand ConnectSerialCommand { get; private set; }

        public RelayCommand GetSerialPortNameCommand { get; private set; }

        public RelayCommand ChangePasswordCommand { get; private set; }

        public RelayCommand StartUpdateFirmwareCommand { get; private set; }

        public DataModel Data { get; private set; }

        public string Password { get; set; }

        private int ModuleType;
        private byte[] FileData;
        private SerialPort MySerialPort = new SerialPort();
        private OpenFileDialog OpenDialog = new OpenFileDialog();

        public ObservableCollection<string> SerialPortNames { get; set; }

        private int _SelectedSerialPortIndex = -1;
        public int SelectedSerialPortIndex
        {
            get { return _SelectedSerialPortIndex; }
            set { _SelectedSerialPortIndex = value; }
        }

        private int _SelectedBaudRateIndex = -1;
        public int SelectedBaudRateIndex
        {
            get { return _SelectedBaudRateIndex; }
            set { _SelectedBaudRateIndex = value; }
        }

        private string[] _BaudStringArray = { "Custom", "110", "300", "600", "1200", "2400", "4800", "9600", "14400", "19200", "38400", "56000", "57600", "115200", "128000", "256000" };
        public string[] BaudStringArray
        {
            get { return _BaudStringArray; }
        }
        public MainViewModel()
        {
            Data = new DataModel();
            RadioBtnCommand = new RelayCommand<object>(RadioPGType2Click);
            OpenFileCommand = new RelayCommand(OpenFile);
            TestSerialCommand = new RelayCommand(TestSerialPort, CanSendCMD);
            ConnectSerialCommand = new RelayCommand(ConnectSerialPort);
            GetSerialPortNameCommand = new RelayCommand(GetSerialPortNameExcute);
            ChangePasswordCommand = new RelayCommand(ChangePassword,CanSendCMD);
            StartUpdateFirmwareCommand = new RelayCommand(StartUpdateFirmwareExcute,CanSendupdateFirmware);
            SerialPortNames = new ObservableCollection<string>();
            Password = "123";
        }

        private void RadioPGType2Click(object Param)
        {
            Data.ProgressBarValue = 0;
            int TempModuleType = int.Parse(Param.ToString());
            if(TempModuleType==1)
            {
                if(PasswordWindow.OpenPassword(Password))
                {
                    ModuleType = TempModuleType;
                    Data.ServiceOnlyModule = true;
                    Data.FirmwareModule = false;
                }
                else
                {
                    Data.ServiceOnlyModule = false;
                    Data.FirmwareModule = true;
                }
            }
            else
            {
                Data.ServiceOnlyModule = false;
                Data.FirmwareModule = true;
            }
        }

        private void OpenFile()
        {
            OpenDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            OpenDialog.Filter = "|*.bin";
            if(OpenDialog.ShowDialog()==true)
            {
                string filepath = OpenDialog.FileName;
                if(File.Exists(filepath))
                {
                    int filelen = (int)IOBinaryOperation.fileLength(filepath);
                    FileData = IOBinaryOperation.readBinaryFile(filepath,filelen);
                    Data.ProgressBarMax = filelen;
                    Data.ProgressBarValue = 0;
                    Data.FileInfo += filepath;
                    Data.FileInfo += "\n";
                }
            }
        }

        private void GetSerialPortNameExcute()
        {
            string[] tempNameArray = SerialPort.GetPortNames();
            SerialPortNames.Clear();
            foreach (string str in tempNameArray)
            {
                SerialPortNames.Add(str.Trim());
            }
        }

        private void ConnectSerialPort()
        {
            if (_SelectedBaudRateIndex == -1 || _SelectedSerialPortIndex == -1)
            {
                MessageBox.Show("please select baud and serialport");
                return;
            }
            if (MySerialPort.IsOpen)
            {
                MySerialPort.Close();
            }
            else
            {
                MySerialPort.PortName = SerialPortNames[_SelectedSerialPortIndex];
                MySerialPort.BaudRate = int.Parse(_BaudStringArray[_SelectedBaudRateIndex]);
                try
                {
                    MySerialPort.Open();
                }
                catch
                {
                    MessageBox.Show("Fail to connect device");
                }
            }
            Data.ConnectionState = MySerialPort.IsOpen;
        }

        private void TestSerialPort()
        {
            if (_SelectedBaudRateIndex == -1 || _SelectedSerialPortIndex == -1)
            {
                MessageBox.Show("please select baud and serialport");
                return;
            }
            byte[] ary = { 0x01, 0x20, 0x03, 0x00, 0x0e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x7c, 0x40 };
            if (MySerialPort.IsOpen)
                MySerialPort.Write(ary, 0, ary.Length);//write all byte of ary into serialport;
        }

        private void ChangePassword()
        {
            if (_SelectedBaudRateIndex == -1 || _SelectedSerialPortIndex == -1)
            {
                MessageBox.Show("please select baud and serialport");
                return;
            }
            byte[] ary = { 0x01, 0x20, 0x03, 0x00, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x01, 0x02, 0x03, 0x04, 0x00, 0x7c, 0x40 };
            if (MySerialPort.IsOpen)
                MySerialPort.Write(ary, 0, ary.Length);//write all byte of ary into serialport;
        }

        private void StartUpdateFirmwareExcute()
        {
            Data.ProgressBarValue = 0;
        }

        private bool CanSendCMD()
        {
            return _SelectedBaudRateIndex != -1 && _SelectedSerialPortIndex != -1;
        }

        private bool CanSendupdateFirmware()
        {
            return _SelectedBaudRateIndex != -1 && _SelectedSerialPortIndex != -1 && FileData != null && FileData.Length != 0;
        }
    }
}
