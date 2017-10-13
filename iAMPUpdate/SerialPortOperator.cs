using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using ExtendedString;
namespace iAMPUpdate
{
    public class SerialPortOperator
    {
        private SerialPort _Serialport = null;
        public SerialPort Serialport
        {
            get { return _Serialport; }
        }

        public bool IsConnected { get; set; }

        public SerialPortOperator()
        {
            
        }

        public static SerialPortOperator GetSerialPort()
        {
            return SingleTon<SerialPortOperator>.GetInstance();
        }
    }
}
