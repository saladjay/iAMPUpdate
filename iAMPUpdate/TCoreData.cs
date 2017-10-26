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
        }

        private byte[,] _m_Memory = new byte[16, 301];
        public byte[,] m_memory
        {
            get { return _m_Memory; }
        }

        private byte[] _m_LocalPreset = new byte[301];
        public byte[] m_LocalPreset
        {
            get { return _m_LocalPreset; }
        }

        private static TCoreData _CoreDate = null;
        private TCoreData()
        {

        }

        public static TCoreData GetInstance()
        {
            if (_CoreDate == null)
            {
                _CoreDate = new TCoreData();
            }
            return _CoreDate;
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
            byte[] tempByteArray = new byte[13];
            for (int i = 0; i < 13; i++)
            {
                tempByteArray[i] = _m_preset[preindex, i];
                Debug.Write(tempByteArray[i].ToString("x2"));
                Debug.Write("-");
            }
            Debug.Write("\n");
            string res = preindex.ToString("x2");
            if (isPre)
                res += System.Text.Encoding.ASCII.GetString(tempByteArray, 0, FinalConst.MaxPresetLength - 4);
            else
                res = System.Text.Encoding.ASCII.GetString(tempByteArray, 0, FinalConst.MaxPresetLength - 4);
            return res;
        }

        public void setPresetName(int preindex,string Name)
        {
            if(preindex>=0&&preindex<16)
            {

            }
        }

        public void SetLocalPreset(byte[] ByteArray)
        {
            int count = 16;
            for (int i = 0; i < FinalConst.Len_Sence_Pack; i++)
            {
                _m_LocalPreset[i] = ByteArray[count++];
            }
        }

        public void SetMemory(byte[] ByteArray)
        {
            int count = 16;
            for (int i = 0; i < _m_Memory.GetLength(0); i++)
            {
                for (int j = 0; j < _m_Memory.GetLength(1); j++)
                {
                    _m_Memory[i, j] = ByteArray[count++];
                }
            }
        }
    }
}
