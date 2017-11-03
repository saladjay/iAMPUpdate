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
using System.Diagnostics;
using System.Windows.Input;
using System.Xml;

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

        public RelayCommand IPScanCommand { get; private set; }

        public RelayCommand<object> ConnectionTypeSwitchCommand { get; private set; }

        public DataModel Data { get; private set; }

        public string Password { get; set; }

        public TCoreData CoreData { get; set; }

        private bool ConnectionType = true;
        private int ModuleType;
        private byte[,] FileData;
        //private byte[] PresetLoadData;
        private byte[] MemoryLoadData;
        private SerialPort MySerialPort = new SerialPort();
        //private IoSocketProces MyIoSocketPort;
        private NetCilent MyNetPort;
        private OpenFileDialog OpenDialog = new OpenFileDialog();
        private SaveFileDialog SaveDialog = new SaveFileDialog();
        private ManagePresets ManagePresetsWin = null;
        private List<byte> Message = new List<byte>();
        private Queue<byte> ByteQueue = new Queue<byte>();
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
            //Data.PropertyChanged += Data_PropertyChanged;
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


            //MyIoSocketPort = IoSocketProces.shareIoSocket();
            //IoSocketProces.ioProc.ReceiveByteEvent += new IoSocketProces.socketReceive(OnReceiveByte);
            //IoSocketProces.ioProc.SockConnectEvent += new IoSocketProces.socketConnected(socketConnected);
            //IoSocketProces.ioProc.showSockLog = true;
            IPScanCommand = new RelayCommand(IPScan);

            ConnectionTypeSwitchCommand = new RelayCommand<object>(ConnectionTypeSwitch);

            MyNetPort = NetCilent.shareCilent();
            MyNetPort.initSocket();
            MyNetPort.onConnectedEvent += MyNetPort_onConnectedEvent;
            MyNetPort.onDisconnectEvent += MyNetPort_onDisconnectEvent;
            MyNetPort.ReceiveByteEvent += MyNetPort_ReceiveByteEvent;
            Thread Converter = new Thread(new ThreadStart(ConverterLoop));
            Converter.IsBackground = true;
            Converter.Start();
            ConnectionTimer.AutoReset = false;
            ConnectionTimer.Elapsed += ConnectionTimer_Elapsed;
            nTimer.Elapsed += NTimer_Elapsed;
            Password = "1234";
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
            //Additional function
            if(File.Exists(AppDomain.CurrentDomain.BaseDirectory+"initialfile.xml"))
            {
                XmlDocument initialXml = new XmlDocument();
                initialXml.Load(AppDomain.CurrentDomain.BaseDirectory + "initialfile.xml");
                XmlElement root = (XmlElement)initialXml.SelectSingleNode("Software");
                if(root !=null&&root.HasChildNodes)
                {
                    foreach (XmlNode item in root.ChildNodes)
                    {
                        if (item.Name.Equals("AdditionalFunction"))
                        {
                            if (item.InnerText.ToLower() == "true")
                                Data.AdditionalFunction = true;
                            else
                                Data.AdditionalFunction = false;
                        }
                        if (item.Name.Equals("Name"))
                        {
                            Data.SoftwareName = ((XmlElement)item).FirstChild.InnerText;
                        }
                    }
                }
            }
            else
            {
                XmlDocument InitialXml = new XmlDocument();
                XmlNode Declaration = InitialXml.CreateXmlDeclaration("1.0", "utf-8", "yes");
                InitialXml.AppendChild(Declaration);
                XmlNode Software = InitialXml.CreateElement("Software");
                InitialXml.AppendChild(Software);

                XmlElement Name = InitialXml.CreateElement("Name");
                Name.AppendChild(InitialXml.CreateTextNode("AR4 UpdateFirmware"));

                XmlElement AdditionalFunction = InitialXml.CreateElement("AdditionalFunction");
                AdditionalFunction.AppendChild(InitialXml.CreateTextNode("true"));

                Software.AppendChild(Name);
                Software.AppendChild(AdditionalFunction);

                InitialXml.Save(AppDomain.CurrentDomain.BaseDirectory + "initialfile.xml");
            }
        }

        //private void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
            
        //}

        private void ConnectionTypeSwitch(object obj)
        {
            ConnectionType = int.Parse(obj.ToString()) == 0;
        }

        private void MyNetPort_ReceiveByteEvent(int cilentindex, byte[] m_rData, int dlen, EventArgs e)
        {
            byte[] newbyte = new byte[dlen];
            for (int i = 0; i < dlen; i++)
            {
                //ByteQueue.Enqueue(m_rData[i]);
                newbyte[i] = m_rData[i];
            }
            ProcessByteArray(newbyte);
        }

        private void MyNetPort_onDisconnectEvent(int cilentindex, EventArgs e)
        {
            Debug.WriteLine("Net disConnect");
            Data.ConnectionState = MyNetPort.isConnected();
            Data.NetConnection = MyNetPort.isConnected();
            RefreshCanExucute();
        }

        private void MyNetPort_onConnectedEvent(int cilentindex, string conIP, EventArgs e)
        {
            Debug.WriteLine("Net Connect");
            Data.ConnectionState = MyNetPort.isConnected();
            Data.NetConnection = MyNetPort.isConnected();
            RefreshCanExucute();
        }

        private void RefreshCanExucute()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                CommandManager.InvalidateRequerySuggested();
            });
        }

        //private void socketConnected(string conIP, EventArgs e)
        //{
        //    Data.ConnectionState = true;
        //}

        //private void OnReceiveByte(byte[] m_rData, int dlen, EventArgs e)
        //{
        //    ProcessByteArray(m_rData);
        //}

    

        private void IPScan()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                WScanForm scanform = new WScanForm();
                var sres = scanform.ShowDialog();
                if (sres == true)
                {
                    if (!String.IsNullOrEmpty(scanform.getScanedIP()) && scanform.getScanedIP().Count() != 0)
                        Data.IP = scanform.getScanedIP().Trim();
                }
            });
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
            byte[] TempByteArray = new byte[MySerialPort.BytesToRead];
            MySerialPort.Read(TempByteArray, 0, TempByteArray.Count());
            //foreach (byte item in TempByteArray)
            //{
            //    ByteQueue.Enqueue(item);
            //    //Debug.Write(item.ToString("x2"));
            //    //Debug.Write("-");
            //}
            ////Debug.Write("\n");

            ProcessByteArray(TempByteArray);
        }

        private void ProcessByteArray(byte[] TempByteArray)
        {
            foreach (byte item in TempByteArray)
            {
                if (Message.Count == 0 && item != FinalConst.UTRAL_H0)
                {
                    Message.Clear();
                    break;
                }
                if (Message.Count == 1 && item != FinalConst.UTRAL_H1)
                {
                    Message.Clear();
                    break;
                }
                if (Message.Count == 2 && item != FinalConst.UTRAL_H2)
                {
                    Message.Clear();
                    break;
                }
                Message.Add(item);
                if (Message.Count > 5)
                {
                    int len = Message[3] * 256 + Message[4];
                    if (Message.Count == len && Message[len - 1] == FinalConst.UTRAL_TAIL)
                    {
                        ConvertToData(Message);
                        Debug.WriteLine("========================================================");
                        foreach (byte a in Message)
                        {
                            Debug.Write(a.ToString("x2"));
                            Debug.Write("-");
                        }
                        Debug.Write("\n");
                        Debug.WriteLine("========================================================");
                        //ReceiveData = true;
                        Message.Clear();
                    }
                }
            }
        }

        private void ProcessByteArray(byte item)
        {

            if (Message.Count == 0 && item != FinalConst.UTRAL_H0)
            {
                Message.Clear();
                return;
            }
            if (Message.Count == 1 && item != FinalConst.UTRAL_H1)
            {
                Message.Clear();
                return;
            }
            if (Message.Count == 2 && item != FinalConst.UTRAL_H2)
            {
                Message.Clear();
                return;
            }
            Message.Add(item);
            if (Message.Count > 5)
            {
                int len = Message[3] * 256 + Message[4];
                if (Message.Count == len && Message[len - 1] == FinalConst.UTRAL_TAIL)
                {
                    ConvertToData(Message);
                    Debug.WriteLine("========================================================");
                    foreach (byte a in Message)
                    {
                        Debug.Write(a.ToString("x2"));
                        Debug.Write("-");
                    }
                    Debug.Write("\n");
                    Debug.WriteLine("========================================================");
                    //ReceiveData = true;
                    Message.Clear();
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
            
            if (ConnectionType)
            {
                if (_SelectedBaudRateIndex == -1 || _SelectedSerialPortIndex == -1)
                {
                    MessageBox.Show("please select baud and serialport");
                    return;
                }
                if (MySerialPort.IsOpen)
                {
                    MySerialPort.Close();
                    Data.UpdateState = false;                   
                }
                else
                {
                    MySerialPort.PortName = SerialPortNames[_SelectedSerialPortIndex];
                    MySerialPort.BaudRate = int.Parse(_BaudStringArray[_SelectedBaudRateIndex]);
                    try
                    {
                        MySerialPort.Open();
                        Data.NetConnection = false;
                    }
                    catch
                    {
                        MessageBox.Show("Fail to connect device");
                    }
                }
                Data.ConnectionState = MySerialPort.IsOpen;
            }
            else
            {
                try
                {
                    if (MyNetPort.isConnected())
                    {
                        //disconnnect
                        MyNetPort.disConnect();
                        Data.UpdateState = false;
                    }
                    else
                    {
                        string strip = Data.IP;
                        if (string.IsNullOrEmpty(strip))
                        {
                            MessageBox.Show("Ip cannot be empty, please scan device firstly");
                            return;
                        }
                        else if (!UtilCover.IPCheck(strip))
                        {
                            MessageBox.Show("Ip inputed is invalided");
                            return;
                        }
                        else if (MyNetPort != null)
                        {
                            MyNetPort.connect(Data.IP.Trim());
                            Data.NetConnection = true;
                        }
                    }
                }
                catch (Exception ec)
                {
                    Debug.WriteLine(" proces error  {0}", ec.ToString());
                }
            }
        }

        private void TestSerialPort()
        {
            //if (_SelectedBaudRateIndex == -1 || _SelectedSerialPortIndex == -1)
            //{
            //    MessageBox.Show("please select baud and serialport");
            //    return;
            //}
            byte[] ary = { 0x01, 0x20, 0x03, 0x00, 0x0e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x7c, 0x40 };
            SerialPortSendCMD(ary);
            ConnectionTimer.Start();

        }

        private void ChangePassword()
        {
            //if (_SelectedBaudRateIndex == -1 || _SelectedSerialPortIndex == -1)
            //{
            //    MessageBox.Show("please select baud and serialport");
            //    return;
            //}
            byte[] ary = { 0x01, 0x20, 0x03, 0x00, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x01, 0x02, 0x03, 0x04, 0x00, 0x7c, 0x40 };
            SerialPortSendCMD(ary);

 
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
            if (Data.UpdateState)
                return false;
            else
                return Data.ConnectionState;
        }

        private bool CanSendupdateFirmware()
        {
            return CanSendCMD() && FileData != null && FileData.Length != 0;
        }

        private void SerialPortSendCMD(byte[] message)
        {
            if(Data.NetConnection)
            {
                if (MyNetPort.isConnected())
                    MyNetPort.sendByte(message);
            }
            else
            {
                int count = message.Length;
                MySerialPort.Write(message, 0, count);
                MySerialPort.DiscardOutBuffer();
            }
        }

        private void ConverterLoop()
        {
            while (true)
            {
                if (ByteQueue.Count!=0)
                {
                    ProcessByteArray(ByteQueue.Dequeue());
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
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Data.PresetCollection.Clear();
                            for (int i = 0; i < 16; i++)
                            {
                                Data.PresetCollection.Add(CoreData.gPresetName(i));
                            }
                        });

                        Data.ManagePresetState = "List of Global Presets completed!";
                        break;
                    }
                case FinalConst.CMD_TYPE_RecallSinglePreset:
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            exportPresetDataToFile(package);
                        });
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
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    ExportMemoryToFile();
                                });
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
            Message.Clear();
        }

        private void exportPresetDataToFile(List<byte> temp)
        {
            SaveDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            SaveDialog.Filter = string.Format("PresetExportFile(*.{0})|*.{0}", FinalConst.PresetFilter);
            SaveDialog.DefaultExt = string.Format(".{0}", FinalConst.PresetFilter);
            SaveDialog.AddExtension = true;
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
