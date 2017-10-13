using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iAMPUpdate
{
    public class DataModel:NotificationObject
    {
        private int _ProgressBarMax;
        public int ProgressBarMax
        {
            get { return _ProgressBarMax; }
            set
            {
                _ProgressBarMax = value;
                OnPropertyChanged("ProgressBarMax");
            }
        }

        private double _ProgressBarValue;
        public double ProgressBarValue
        {
            get { return _ProgressBarValue; }
            set
            {
                _ProgressBarValue = value;
                OnPropertyChanged("ProgressBarValue");
            }
        }

        private bool _FirmwareModule;
        public bool FirmwareModule
        {
            get { return _FirmwareModule; }
            set
            {
                _FirmwareModule = value;
                OnPropertyChanged("FirmwareModule");
            }
        }

        private bool _ServiceOnlyModule;
        public bool ServiceOnlyModule
        {
            get { return _ServiceOnlyModule; }
            set
            {
                _ServiceOnlyModule = value;
                OnPropertyChanged("ServiceOnlyModule");
            }
        }

        private string _FileInfo;
        public string FileInfo
        {
            get { return _FileInfo; }
            set
            {
                _FileInfo = value;
                OnPropertyChanged("FileInfo");
            }
        }
    }
}
