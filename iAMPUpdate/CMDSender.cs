using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iAMPUpdate
{
    public class CMDSender
    {
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

        public static void sendCMD_finish_program_notice()
        {
            byte[] Msg = CreateByteArray(FinalConst.CMD_LEN_FINISH_PROGRAM,FinalConst.CMD_TYPE_FINISH_PROGRAM);

        }

        public static void sendCMD_GotoReset()
        {
            byte[] Msg = CreateByteArray(FinalConst.CMD_LEN_RESETDEVICE, FinalConst.CMD_TYPE_RESETDEVICE);
        }

        public static void sendCMD_UpdateStop()
        {
            byte[] Msg = CreateByteArray(FinalConst.CMD_LEN_READY_STOP, FinalConst.CMD_TYPE_READY_STOP);
        }

        public static void sendCMD_ReadPresetList()
        {
            byte[] Msg = CreateByteArray(FinalConst.CMD_LEN_READPRESETLIST, FinalConst.CMD_TYPE_READPRESETLIST);
        }

        public static void sendCMD_ReadyToProgram()
        {
            byte[] Msg = CreateByteArray(FinalConst.CMD_LEN_READY_PROGRAM, FinalConst.CMD_TYPE_READY_PROGRAM);
        }

        public static void sendCMD_BeforeUpdateFirmware()
        {
            byte[] Msg = CreateByteArray(FinalConst.CMD_LEN_DO_PROGRAM, FinalConst.CMD_TYPE_DO_PROGRAM);
        }

        public static void sendCMD_UpdateFirmware()
        {
            byte[] Msg = CreateByteArray(1024 + 15, FinalConst.CMD_TYPE_DO_PROGRAM);
        }

        public static void sendCMD_DeleteSinglePreset()
        {
            byte[] Msg = CreateByteArray(FinalConst.CMD_LEN_DeletePreset, FinalConst.CMD_TYPE_DeletePreset);
        }

        public static void sendCMD_RecallSinglePreset()
        {
            byte[] Msg = CreateByteArray(FinalConst.CMD_LEN_RecallSinglePreset, FinalConst.CMD_TYPE_RecallSinglePreset);
        }

        public static void sendCMD_RenamePreset(int preindex)
        {
            byte[] Msg = CreateByteArray(FinalConst.CMD_LEN_RenamePresetFromPC, FinalConst.CMD_TYPE_RenamePresetFromPC);
        }

        public static void sendCMD_MemoryExport()
        {
            byte[] Msg = CreateByteArray(FinalConst.CMD_LEN_MemoryExportFromDevice, FinalConst.CMD_LEN_MemoryExportFromDevice);
        }

    }
}
