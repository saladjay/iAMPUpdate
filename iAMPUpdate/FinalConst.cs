using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iAMPUpdate
{
    class FinalConst
    {
        public static string PresetFilter = "DSP2188P";
        public static string PresetHeader = "****DSP2188P****";
        public static string MemoryHeader = "**MemoryDSP218**";
        public const int MaxLength = 1024;
        public const int Len_Sence_Pack = 301;
        public const int MaxPresetLength = 13;

        public const int UTRAL_H0 = 0x01;
        public const int UTRAL_H1 = 0x20;
        public const int UTRAL_H2 = 0x03;

        public const int UTRAL_TAIL = 0x40;

        public static int ID_MACHINE;

        public static byte LineID;
        public static byte MachineID;

        public const int CMD_TYPE_FINISH_PROGRAM = 0xF1;
        public const int CMD_LEN_FINISH_PROGRAM = 0x0D;

        public const int CMD_TYPE_RESETDEVICE = 0xEE;
        public const int CMD_LEN_RESETDEVICE = 0x0D;

        public const int CMD_TYPE_READY_STOP = 0xe0;
        public const int CMD_LEN_READY_STOP = 0x0D;

        public const int CMD_TYPE_READPRESETLIST = 0x15;
        public const int CMD_LEN_READPRESETLIST = 0x0D;

        public const int CMD_TYPE_READY_PROGRAM = 0xEF;
        public const int CMD_LEN_READY_PROGRAM = 0x0E;

        public const int CMD_TYPE_DO_PROGRAM = 0xF0;
        public const int CMD_LEN_DO_PROGRAM = 0x10;

        public const int CMD_TYPE_DeletePreset = 0x12;
        public const int CMD_LEN_DeletePreset = 0x0E;

        public const int CMD_TYPE_RecallSinglePreset = 0x0E;
        public const int CMD_LEN_RecallSinglePreset = 0x0E;

        public const int CMD_TYPE_RenamePresetFromPC = 0x16;
        public const int CMD_LEN_RenamePresetFromPC = 0x17;

        public const int CMD_TYPE_MemoryExportFromDevice = 0x13;
        public const int CMD_LEN_MemoryExportFromDevice = 0x0E;

        public const int CMD_TYPE_FinishProgram = 0xF1;
        public const int CMD_LEN_FinishProgram = 0x0D;

        public const int CMD_TYPE_MemoryImportFromPC = 0x14;
        public const int CMD_LEN_MemoryImportFromPC = 301;

        public const int CMD_TYPE_LoadPreset_fromPC = 0x11;

        public const int CMD_TYPE_GETADDRS_RECALL = 0x21;

        public const int CMD_TYPE_CHECK_DEVICE = 0xaa;
    }
}
