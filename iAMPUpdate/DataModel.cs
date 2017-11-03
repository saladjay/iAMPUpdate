using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iAMPUpdate
{
    public class DataModel:NotificationObject
    {
        private int _ProgressBarMax = int.MaxValue;
        public int ProgressBarMax
        {
            get { return _ProgressBarMax; }
            set
            {
                _ProgressBarMax = value;
                OnPropertyChanged("ProgressBarMax");
            }
        }

        private double _ProgressBarValue = 0;
        public double ProgressBarValue
        {
            get { return _ProgressBarValue; }
            set
            {
                _ProgressBarValue = value;
                OnPropertyChanged("ProgressBarValue");
            }
        }

        private bool _FirmwareModule = true;
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

        private string _ManagePresetState;
        public string ManagePresetState
        {
            get { return _ManagePresetState; }
            set
            {
                _ManagePresetState = value;
                OnPropertyChanged("ManagePresetState");
            }
        }

        private ObservableCollection<string> _PresetCollection = new ObservableCollection<string>();
        public ObservableCollection<string> PresetCollection
        {
            get { return _PresetCollection; }
        }

        private bool _ConnectionState;
        public bool ConnectionState
        {
            get { return _ConnectionState; }
            set
            {
                _ConnectionState = value;
                OnPropertyChanged("ConnectionState");
            }
        }

        private bool _UpdateState;
        public bool UpdateState
        {
            get { return _UpdateState; }
            set
            {
                _UpdateState = value;
                OnPropertyChanged("UpdateState");
            }
        }

        private string _DownloadProgress;
        public string DownloadProgress
        {
            get { return _DownloadProgress; }
            set
            {
                _DownloadProgress = value;
                OnPropertyChanged("DownloadProgress");
            }
        }

        private string _UploadProgress;
        public string UploadProgress
        {
            get { return _UploadProgress; }
            set
            {
                _UploadProgress = value;
                OnPropertyChanged("UploadProgress");
            }
        }

        private string _IP;
        public string IP
        {
            get { return _IP; }
            set
            {
                _IP = value;
                OnPropertyChanged("IP");
            }
        }

        private bool _NetConnection;
        public bool NetConnection
        {
            get { return _NetConnection; }
            set
            {
                _NetConnection = value;
                OnPropertyChanged("ConnectionType");
            }
        }

        private bool _AdditionalFunction;
        public bool AdditionalFunction
        {
            get { return _AdditionalFunction; }
            set
            {
                _AdditionalFunction = value;
                OnPropertyChanged("AdditionalFunction");
            }
        }

        public string SoftwareName
        {
            get
            {
                return _SoftwareName;
            }

            set
            {
                _SoftwareName = value;
                //OnPropertyChanged("SoftwareName");
            }
        }

        private string _SoftwareName;
    }
}
