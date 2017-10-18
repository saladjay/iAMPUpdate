using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iAMPUpdate
{
    public class CMDSender
    {
        private static TCoreData CoreData = null;
        static CMDSender()
        {
            CoreData = TCoreData.GetInstance();
        }

        private static byte[] CreateByteArray(int count,int type)
        {
            byte[] Msg = new byte[count];
            Msg[0] = FinalConst.UTRAL_H0;
            Msg[1] = FinalConst.UTRAL_H1;
            Msg[2] = FinalConst.UTRAL_H2;
            Msg[3] = (byte)(count / 256);
            Msg[4] = (byte)(count % 256);
            Msg[5] = (byte)(FinalConst.ID_MACHINE / 256);
            Msg[6] = (byte)(FinalConst.ID_MACHINE % 256);
            Msg[7] = FinalConst.LineID;
            Msg[8] = FinalConst.MachineID;
            Msg[9] = (byte)(type / 256);
            Msg[10] = (byte)(type % 256);
            Msg[count - 1] = FinalConst.UTRAL_TAIL;
            //byte CheckByte = Msg[0];
            //for (int i = 1; i < count; i++)
            //{
            //    CheckByte ^= Msg[i];
            //}
            return Msg;
        }

        private static byte[] CalculateCheckBit(byte[] ByteArray)
        {
            byte CheckBit = ByteArray[0];
            for (int i = 1; i < ByteArray.Length; i++)
            {
                CheckBit ^= ByteArray[i];
            }
            ByteArray[ByteArray.Length - 2] = CheckBit;
            return ByteArray;
        }

        public static byte[] sendCMD_finish_program_notice()
        {
            return CreateByteArray(FinalConst.CMD_LEN_FINISH_PROGRAM,FinalConst.CMD_TYPE_FINISH_PROGRAM);
        }

        public static byte[] sendCMD_GotoReset()
        {
            return CreateByteArray(FinalConst.CMD_LEN_RESETDEVICE, FinalConst.CMD_TYPE_RESETDEVICE);
        }

        public static byte[] sendCMD_UpdateStop()
        {
            return CreateByteArray(FinalConst.CMD_LEN_READY_STOP, FinalConst.CMD_TYPE_READY_STOP);
        }

        public static byte[] sendCMD_ReadPresetList()
        {
            return CreateByteArray(FinalConst.CMD_LEN_READPRESETLIST, FinalConst.CMD_TYPE_READPRESETLIST);
        }

        public static byte[] sendCMD_ReadyToProgram(int filelength)
        {
            byte[] ByteArray = CreateByteArray(FinalConst.CMD_LEN_READY_PROGRAM, FinalConst.CMD_TYPE_READY_PROGRAM);
            ByteArray[11] = (byte)filelength;
            return CalculateCheckBit(ByteArray);
        }

        public static byte[] sendCMD_BeforeUpdateFirmware()
        {
            return CreateByteArray(FinalConst.CMD_LEN_DO_PROGRAM, FinalConst.CMD_TYPE_DO_PROGRAM);
        }

        public static byte[] sendCMD_UpdateFirmware(int index,byte[] FileSegement)
        {
            byte[] ByteArray = CreateByteArray(1024 + 15, FinalConst.CMD_TYPE_DO_PROGRAM);
            ByteArray[11] = (byte)(index + 1);
            ByteArray[12] = (byte)(0xFF - index - 1);
            Array.Copy(FileSegement, 0, ByteArray, 13, FileSegement.Length);
            return CalculateCheckBit(ByteArray);
        }

        public static byte[] sendCMD_DeleteSinglePreset(int index)
        {
            byte[] ByteArray = CreateByteArray(FinalConst.CMD_LEN_DeletePreset, FinalConst.CMD_TYPE_DeletePreset);
            ByteArray[11] = (byte)index;
            return CalculateCheckBit(ByteArray);
        }

        public static byte[] sendCMD_RecallSinglePreset(int index)
        {
            byte[] ByteArray = CreateByteArray(FinalConst.CMD_LEN_RecallSinglePreset, FinalConst.CMD_TYPE_RecallSinglePreset);
            ByteArray[11] = (byte)index;
            return CalculateCheckBit(ByteArray);
        }

        public static byte[] sendCMD_RenamePreset(int preindex)
        {
            return CreateByteArray(FinalConst.CMD_LEN_RenamePresetFromPC, FinalConst.CMD_TYPE_RenamePresetFromPC);
        }

        public static byte[] sendCMD_MemoryExport(int index)
        {
            byte[] ByteArray = CreateByteArray(FinalConst.CMD_LEN_MemoryExportFromDevice, FinalConst.CMD_LEN_MemoryExportFromDevice);
            ByteArray[11] = (byte)index;
            return CalculateCheckBit(ByteArray);
        }

        public static byte[] sendCMD_MemoryImport_Scence(int index)
        {
            //return CreateByteArray(FinalConst.CMD_LEN_MemoryImportFromPC, FinalConst.CMD_TYPE_MemoryImportFromPC);
            byte[] ByteArray = new byte[FinalConst.Len_Sence_Pack];
            for (int i = 0; i < FinalConst.Len_Sence_Pack; i++)
            {
                ByteArray[i] = CoreData.m_memory[index, i];
            }
            ByteArray[9] = FinalConst.CMD_TYPE_MemoryImportFromPC / 256;
            ByteArray[10] = FinalConst.CMD_TYPE_MemoryImportFromPC % 256;
            return CalculateCheckBit(ByteArray);
        }

        public static byte[] sendCMD_LoadPresetFromLocal()
        {
            byte[] ByteArray = new byte[FinalConst.Len_Sence_Pack];
            Array.Copy(CoreData.m_LocalPreset, ByteArray, FinalConst.Len_Sence_Pack);
            ByteArray[9] = FinalConst.CMD_TYPE_LoadPreset_fromPC / 256;
            ByteArray[10] = FinalConst.CMD_TYPE_LoadPreset_fromPC % 256;
            return CalculateCheckBit(ByteArray);
        }
    }
}
