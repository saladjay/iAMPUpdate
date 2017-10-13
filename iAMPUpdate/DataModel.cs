using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iAMPUpdate
{
    public class DataModel:NotificationObject
    {
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
    }
}
