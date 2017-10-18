using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iAMPUpdate
{
    public class ManagePresetViewModel
    {
        public RelayCommand CreateSinglePresetFileCommand { get; private set; }
        public RelayCommand UploadSelectedPresetFileCommand { get; private set; }
        public RelayCommand ResetPresetsCommand { get; private set; }
        public RelayCommand SavePresetsFileCommand { get; private set; }
        public RelayCommand UploadPresetsToDeviceCommand { get; private set; }

        public ManagePresetViewModel()
        {
            CreateSinglePresetFileCommand = new iAMPUpdate.RelayCommand(CreateSinglePreserFile, CanCreateSinglePresetFile);
        }

        private bool CanCreateSinglePresetFile()
        {
            throw new NotImplementedException();
        }

        private void CreateSinglePreserFile()
        {
            throw new NotImplementedException();
        }
    }
}
