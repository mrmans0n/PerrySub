using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace scriptASS
{
    class IMM32Wrapper
    {
        public const int WM_CHAR = 0x102;
        public const int WM_KEYUP = 0x101;
        public const int WM_IME_STARTCOMPOSITION = 0x10D;
        public const int WM_IME_ENDCOMPOSITION = 0x10E;
        public const int WM_IME_COMPOSITION = 0x10F;
        public const int GCS_COMPREADSTR = 0x1;
        public const int GCS_COMPREADATTR = 0x2;
        public const int GCS_COMPREADCLAUSE = 0x4;
        public const int GCS_COMPSTR = 0x8;
        public const int GCS_COMPATTR = 0x10;
        public const int GCS_COMPCLAUSE = 0x20;
        public const int GCS_CURSORPOS = 0x80;
        public const int GCS_DELTASTART = 0x100;
        public const int GCS_RESULTREADSTR = 0x200;
        public const int GCS_RESULTREADCLAUSE = 0x400;
        public const int GCS_RESULTSTR = 0x800;
        public const int GCS_RESULTCLAUSE = 0x1000;

        public const int SCS_SETSTR = 0x80;
        
        public const int IMM_ERROR_NODATA = -1;
        public const int IMM_ERROR_GENERAL = -2;


        [DllImport("imm32.dll")] 
        public static extern int ImmGetContext(int hwnd);
        [DllImport("imm32.dll")]
        public static extern int ImmReleaseContext(int hwnd, int himc);
        [DllImport("imm32.dll")]
        public static extern int ImmGetCompositionStringW(int himc, int dwindex, byte[] lpbuf, int buflen);
        [DllImport("imm32.dll")]
        public static extern int ImmSetCompositionStringW(int himc, int dwindex, byte[] lpComp, int lpCompLen ,byte[] lpRead, int lpReadLen);
        [DllImport("imm32.dll")]
        public static extern int ImmGetOpenStatus(int himc);

        [DllImport("imm32.dll")]
        public static extern bool ImmSetOpenStatus(int himc, bool fOpen);


    }
}
