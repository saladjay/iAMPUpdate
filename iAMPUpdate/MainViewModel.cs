using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iAMPUpdate
{
    public class MainViewModel
    {
        public RelayCommand<string> RadioBtnComman { get; private set; }

        public DataModel Data { get; private set; }

        private int ModuleType;

        public MainViewModel()
        {

        }

        private void RadioPGType2Click(string Param)
        {
            Data.ProgressBarValue = 0;
            ModuleType = int.Parse(Param);
            if(ModuleType==1)
            {

            }
        }
    }
}
