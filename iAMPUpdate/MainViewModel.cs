using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommLibrary;
using Microsoft.Win32;
using System.IO;

namespace iAMPUpdate
{
    public class MainViewModel
    {
        public RelayCommand<string> RadioBtnCommand { get; private set; }

        public RelayCommand OpenFileCommand { get; private set; }

        public DataModel Data { get; private set; }

        public string Password { get; set; }

        private int ModuleType;
        private byte[] FileData;

        private OpenFileDialog OpenDialog = new OpenFileDialog();

        public MainViewModel()
        {
            Data = new DataModel();
        }

        private void RadioPGType2Click(string Param)
        {
            Data.ProgressBarValue = 0;
            int TempModuleType = int.Parse(Param);
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

        private string OpenFile()
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
                    return filepath;
                }
            }
            return null;
        }
    }
}
