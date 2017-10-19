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
using System.Threading;

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

        public RelayCommand SavePresetCommand { get; private set; }

        public RelayCommand CreateSinglePresetFileCommand { get; private set; }
        public RelayCommand UploadSelectedPresetFileCommand { get; private set; }
        public RelayCommand ResetPresetsCommand { get; private set; }
        public RelayCommand SavePresetsFileCommand { get; private set; }
        public RelayCommand UploadPresetsToDeviceCommand { get; private set; }


        public DataModel Data { get; private set; }

        public string Password { get; set; }

        public TCoreData CoreData { get; set; }

        private int ModuleType;
        private byte[,] FileData;
        //private byte[] PresetLoadData;
        private byte[] MemoryLoadData;
        private SerialPort MySerialPort = new SerialPort();
        private OpenFileDialog OpenDialog = new OpenFileDialog();
        private SaveFileDialog SaveDialog = new SaveFileDialog();
        private ManagePresets ManagePresetsWin = null;
        private List<byte> Message = new List<byte>();
        private bool ReceiveData = false;
        private System.Timers.Timer ConnectionTimer = new System.Timers.Timer(1000);
        private System.Timers.Timer nTimer = new System.Timers.Timer(500);
        private byte[,] MyPreset = new byte[16,13];
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

        private int _SelectedPresetIndex=-1;
        public int SelectedPresetIndex
        {
            get { return _SelectedPresetIndex; }
            set { _SelectedPresetIndex = value; }
        }


        private string[] _BaudStringArray = { "Custom", "110", "300", "600", "1200", "2400", "4800", "9600", "14400", "19200", "38400", "56000", "57600", "115200", "128000", "256000" };
        public string[] BaudStringArray
        {
            get { return _BaudStringArray; }
        }


        private MainViewModel()
        {
            Data = new DataModel();
            CoreData = TCoreData.GetInstance();
            RadioBtnCommand = new RelayCommand<object>(RadioPGType2Click);
            OpenFileCommand = new RelayCommand(OpenFile);
            TestSerialCommand = new RelayCommand(TestSerialPort, CanSendCMD);
            ConnectSerialCommand = new RelayCommand(ConnectSerialPort);
            GetSerialPortNameCommand = new RelayCommand(GetSerialPortNameExcute);
            ChangePasswordCommand = new RelayCommand(ChangePassword,CanSendCMD);
            StartUpdateFirmwareCommand = new RelayCommand(StartUpdateFirmwareExcute,CanSendupdateFirmware);
            SavePresetCommand = new RelayCommand(SavePresetExcute,CanSendCMD);
            SerialPortNames = new ObservableCollection<string>();
            MySerialPort.DataReceived += MySerialPort_DataReceived;
            Thread Converter = new Thread(new ThreadStart(ConverterLoop));
            Converter.IsBackground = true;
            ConnectionTimer.AutoReset = false;
            ConnectionTimer.Elapsed += ConnectionTimer_Elapsed;
            nTimer.Elapsed += NTimer_Elapsed;
            Password = "123";
            for (int i = 0; i < 16; i++)
            {
                Data.PresetCollection.Add(i.ToString());
            }
            //manage 
            CreateSinglePresetFileCommand = new iAMPUpdate.RelayCommand(CreateSinglePreserFile, CanCreateSinglePresetFile);
            UploadSelectedPresetFileCommand = new RelayCommand(UploadSelectedPresetFile);
            ResetPresetsCommand = new RelayCommand(ResetPresets, CanCreateSinglePresetFile);
            SavePresetsFileCommand = new RelayCommand(SavePresetsFile);
            UploadPresetsToDeviceCommand = new RelayCommand(UploadPresetsToDevice);
        }

        

        private void NTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SerialPortSendCMD(CMDSender.sendCMD_ReadyToProgram(Data.ProgressBarMax));
        }

        private static readonly MainViewModel _MainViewModel = new MainViewModel();

        public static MainViewModel GetInstance()
        {
            return _MainViewModel;
        }

        private void ConnectionTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            MessageBox.Show("Fail to connect");
        }

        private void MySerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] TempByteArray = new byte[MySerialPort.ReadBufferSize];
            MySerialPort.Read(TempByteArray, 0, TempByteArray.Count());
            MySerialPort.DiscardInBuffer();
            foreach (byte item in TempByteArray)
            {
                if(Message.Count==0&&item!=FinalConst.UTRAL_H0)
                {
                    Message.Clear();
                    break;
                }
                if(Message.Count==1&&item!=FinalConst.UTRAL_H1)
                {
                    Message.Clear();
                    break;
                }
                if(Message.Count==2&&item!=FinalConst.UTRAL_H2)
                {
                    Message.Clear();
                    break;
                }
                Message.Add(item);
                if(Message.Count>5)
                {
                    int len = Message[3] * 256 + Message[4];
                    if(Message.Count==len&&Message[len-1]==FinalConst.UTRAL_TAIL)
                    {
                        ReceiveData = true;
                    }
                }
            }
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
            OpenDialog.Filter = "Bin File|*.bin";
            if (OpenDialog.ShowDialog()==true)
            {
                string filepath = OpenDialog.FileName;
                if(File.Exists(filepath))
                {
                    int filelen = (int)IOBinaryOperation.fileLength(filepath);
                    byte[] TempFileData = IOBinaryOperation.readBinaryFile(filepath,filelen);
                    int SegementsCount = filelen / FinalConst.MaxLength + 1;
                    FileData = new byte[SegementsCount, FinalConst.MaxLength];
                    int Count = 0;
                    for (int i = 0; i < SegementsCount; i++)
                    {
                        for (int j = 0; j < FinalConst.MaxLength; j++)
                        {
                            if (Count < TempFileData.Length)
                                FileData[i, j] = TempFileData[Count++];
                            else
                                break;
                        }
                    }
                    Data.ProgressBarMax = SegementsCount;
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
            {
                MySerialPort.Write(ary, 0, ary.Length);//write all byte of ary into serialport;
                ConnectionTimer.Start();
            }
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
            if (Data.FirmwareModule)
            {
                SerialPortSendCMD(CMDSender.sendCMD_GotoReset());
            }
            else if(Data.ServiceOnlyModule)
            {
                nTimer.Start();
            }
            Data.UpdateState = true;
        }

        private void SavePresetExcute()
        {
            SerialPortSendCMD(CMDSender.sendCMD_ReadPresetList());
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ManagePresetsWin = new ManagePresets();
                ManagePresetsWin.ShowDialog();
            }));
        }

        private bool CanSendCMD()
        {
            return _SelectedBaudRateIndex != -1 && _SelectedSerialPortIndex != -1 && MySerialPort.IsOpen;
        }

        private bool CanSendupdateFirmware()
        {
            return CanSendCMD() && FileData != null && FileData.Length != 0;
        }

        private void SerialPortSendCMD(byte[] message)
        {
            int count = message.Length;
            MySerialPort.Write(message, 0, count);
            MySerialPort.DiscardOutBuffer();
        }

        private void ConverterLoop()
        {
            while (true)
            {
                if (ReceiveData)
                {
                    ConvertToData(Message);
                    ReceiveData = false;
                }
            }
        }

        private void ConvertToData(List<byte> package)
        {
            int CMD = package[9] * 256 + package[10];

            switch(CMD)
            {
                case FinalConst.CMD_TYPE_CHECK_DEVICE:
                    {
                        break;
                    }
                case FinalConst.CMD_TYPE_READY_STOP:
                    {
                        SerialPortSendCMD(CMDSender.sendCMD_UpdateStop());
                        break;
                    }
                case FinalConst.CMD_TYPE_RESETDEVICE://0xEE
                    {
                        nTimer.Start();
                        break;
                    }
                case 0x10:
                    {
                        ConnectionTimer.Stop();
                        break;
                    }
                case 0x08:
                    {
                        break;
                    }
                case FinalConst.CMD_TYPE_READY_PROGRAM://0xEF
                    {
                        nTimer.Stop();
                        break;
                    }
                case FinalConst.CMD_TYPE_DO_PROGRAM://0xF0
                    {
                        int SegementIndex = package[11];
                        Data.ProgressBarValue = SegementIndex + 1;
                        if(SegementIndex<Data.ProgressBarMax)
                        {
                            byte[] tempByteArray = new byte[FinalConst.MaxLength];
                            for (int i = 0; i < FinalConst.MaxLength; i++)
                            {
                                tempByteArray[i] = FileData[SegementIndex, i];
                            }
                            SerialPortSendCMD(CMDSender.sendCMD_UpdateFirmware(SegementIndex,tempByteArray));
                        }
                        if(SegementIndex+1==Data.ProgressBarMax)
                        {

                        }
                        break;
                    }
                case FinalConst.CMD_TYPE_FINISH_PROGRAM://0xF1
                    {
                        Data.FileInfo += "Update finished,please select Disconnect and close software.Please restart device after 2 minutes\n";
                        Data.UpdateState = false;
                        break;
                    }
                case FinalConst.CMD_TYPE_GETADDRS_RECALL://0x21
                    {
                        Data.FileInfo += "GETADDR\n";
                        Thread.Sleep(1000);
                        byte[] GAADRsendBuf = { 0x01, 0x20, 0x03, 0x00, 0x0E, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x60, 0x40 };
                        SerialPortSendCMD(GAADRsendBuf);
                        break;
                    }
                case FinalConst.CMD_TYPE_READPRESETLIST://0x15
                    {
                        int count = 11;
                        for (int r = 0; r < 16; r++)
                        {
                            for (int i = 0; i < 13; i++)
                            {
                                //MyPreset[r, i] = package[count++];
                                CoreData.m_preset[r, i] = package[count++];
                            }
                        }
                        Data.PresetCollection.Clear();
                        for (int i = 0; i < 16; i++)
                        {
                            Data.PresetCollection.Add(CoreData.gPresetName(i));
                        }
                        Data.ManagePresetState = "List of Global Presets completed!";
                        break;
                    }
                case FinalConst.CMD_TYPE_RecallSinglePreset:
                    {
                        exportPresetDataToFile(package);
                        break;
                    }
                case FinalConst.CMD_TYPE_LoadPreset_fromPC:
                    {

                        break;
                    }
                case FinalConst.CMD_TYPE_MemoryExportFromDevice:
                    {
                        int index = package[11];
                        int Progress = (index + 1) / 16 * 100;
                        Data.DownloadProgress = string.Format("{0}%", Progress);
                        if (index>=0&&index<16)
                        {
                            for (int i = 0; i < FinalConst.Len_Sence_Pack; i++)
                            {
                                CoreData.m_memory[index, i] = package[i];
                            }
                            if(index==15)
                            {
                                ExportMemoryToFile();
                            }
                        }
                        break;
                    }
                case FinalConst.CMD_TYPE_MemoryImportFromPC:
                    {
                        int index = package[11];
                        int Progress = (index + 1) / 16 * 100;
                        Data.UploadProgress = string.Format("{0}%", Progress);
                        if(index<15)
                        {
                            SerialPortSendCMD(CMDSender.sendCMD_MemoryImport_Scence(index + 1));
                        }
                        break;
                    }
                    
            }
        }
        private void exportPresetDataToFile(List<byte> temp)
        {
            SaveDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            SaveDialog.Filter = string.Format("PresetExportFile(*%{0})|*%{0}", FinalConst.PresetFilter);
            if(SaveDialog.ShowDialog()==true)
            {
                string FilePath = SaveDialog.FileName;
                IOBinaryOperation.writeBinaryToFile(FilePath, System.Text.Encoding.ASCII.GetBytes(FinalConst.PresetHeader));
                IOBinaryOperation.writeBinaryToFile(FilePath, temp.ToArray());
            }
        }

        private void ExportMemoryToFile()
        {
            if(MessageBox.Show("Global Presets downloaded, are you sure to save as file?","Save Global Presets as file",MessageBoxButton.YesNo)==MessageBoxResult.Yes)
            {
                SaveDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                SaveDialog.Filter = "MemoryExportFile(*.ept)|*.ept";
                if (SaveDialog.ShowDialog()==true)
                {
                    string FilePath = SaveDialog.FileName;
                    IOBinaryOperation.writeBinaryToFile(FilePath, System.Text.Encoding.ASCII.GetBytes(FinalConst.MemoryHeader));
                    byte[] temp = new byte[CoreData.m_memory.GetLength(0) * CoreData.m_memory.GetLength(1)];
                    int count = 0;
                    for (int i = 0; i < CoreData.m_memory.GetLength(0); i++)
                    {
                        for (int j = 0; j < CoreData.m_memory.GetLength(1); j++)
                        {
                            temp[count++] = CoreData.m_memory[i, j];
                        }
                    }
                    IOBinaryOperation.writeBinaryToFile(FilePath, temp);
                }
            }
        }
        #region Command Processing

        #endregion

        #region Manage Presets

        private bool CanCreateSinglePresetFile()
        {
            return _SelectedPresetIndex >= 0 && _SelectedPresetIndex < 16;
        }

        private void CreateSinglePreserFile()
        {
            SerialPortSendCMD(CMDSender.sendCMD_RecallSinglePreset(_SelectedPresetIndex));
        }


        private void UploadSelectedPresetFile()
        {
            if(ReadSinglePresetFromFile())
            {
                SerialPortSendCMD(CMDSender.sendCMD_LoadPresetFromLocal());
            }
        }

        private bool ReadSinglePresetFromFile()
        {
            bool Result = false;
            OpenDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            OpenDialog.Filter = string.Format( "PresetExportFile({0})|*.{0}",FinalConst.PresetFilter);
            if (OpenDialog.ShowDialog() == true)
            {
                string filepath = OpenDialog.FileName;
                if (File.Exists(filepath))
                {
                    int filelen = (int)IOBinaryOperation.fileLength(filepath);
                    if (filelen != (FinalConst.Len_Sence_Pack + 16))
                        return false;
                    //PresetLoadData = IOBinaryOperation.readBinaryFile(filepath, filelen);
                    CoreData.SetLocalPreset(IOBinaryOperation.readBinaryFile(filepath, filelen));
                    Result = true;
                }
            }
            return Result;
        }

        private void ResetPresets()
        {
            SerialPortSendCMD(CMDSender.sendCMD_DeleteSinglePreset(_SelectedPresetIndex));
        }

        private void SavePresetsFile()
        {
            SerialPortSendCMD(CMDSender.sendCMD_MemoryExport(0));
        }

        private void UploadPresetsToDevice()
        {
            if (LoadMemoryFile())
            {
                SerialPortSendCMD(CMDSender.sendCMD_MemoryImport_Scence(0));
            }
        }

        private bool LoadMemoryFile()
        {
            bool Result = false;
            OpenDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            OpenDialog.Filter = "MemoryExportFile(*.ept)|*.ept";
            if (OpenDialog.ShowDialog() == true)
            {
                string filepath = OpenDialog.FileName;
                if (File.Exists(filepath))
                {
                    int filelen = (int)IOBinaryOperation.fileLength(filepath);
                    if (filelen != (FinalConst.Len_Sence_Pack*16 + 16))
                        return false;
                    MemoryLoadData = IOBinaryOperation.readBinaryFile(filepath, filelen);
                    CoreData.SetMemory(MemoryLoadData);
                    Result = true;
                }
            }

            return Result;
        }
        #endregion
    }
}
