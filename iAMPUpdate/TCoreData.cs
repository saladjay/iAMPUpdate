using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iAMPUpdate
{
    public class TCoreData
    {
        private byte[,] _m_preset = new byte[16,13];
        public byte[,] m_preset
        {
            get { return _m_preset; }
            set { _m_preset = value; }
        }

        private byte[,] _m_memory = new byte[16, 301];
        public byte[,] m_memory
        {
            get { return _m_memory; }
            set { _m_memory = value; }
        }

        public void resetPreSetList()
        {
            for (int i = 0; i < _m_preset.GetLength(0); i++)
            {
                for (int j = 0; j < _m_preset.GetLength(1); j++)
                {
                    _m_preset[i, j] = 0;
                }
            }
        }

        public string gPresetName(int preindex,bool isPre=true)
        {
            string str = "";
            for (int i = 0; i < _m_preset.GetLength(0)-4; i++)
            {
                if(preindex==0)
                {
                    str = _m_preset[preindex, i].ToString("x2");
                    Debug.WriteLine(str);
                }
            }
            string res = preindex.ToString("x2");
            if (isPre)
                res += "";
            return res;
        }

        public void setPresetName(int preindex,string Name)
        {
            if(preindex>=0&&preindex<16)
            {

            }
        }
    }
}
