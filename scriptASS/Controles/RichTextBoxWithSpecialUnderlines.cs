using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace scriptASS
{

    public class RichTextBoxWithSpecialUnderlines : RichTextBox
    {
        private const int SCF_SELECTION = 1;
  
        private const int CFM_UNDERLINETYPE = 8388608;
        private const int CFE_UNDERLINE = 4;

        private const int EM_SETCHARFORMAT = 1092;
        private const int EM_GETCHARFORMAT = 1082;

        [StructLayout(LayoutKind.Sequential)]
        private struct CHARFORMAT
        {
            public int cbSize;
            public uint dwMask;
            public uint dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szFaceName;

            // CHARFORMAT2 from here onwards.
            public short wWeight;
            public short sSpacing;
            public int crBackColor;
            public int LCID;
            public uint dwReserved;
            public short sStyle;
            public short wKerning;
            public byte bUnderlineType;
            public byte bAnimation;
            public byte bRevAuthor;
        }

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef hWnd, int msg,
                                               int wParam, ref CHARFORMAT lp);

        // <summary>
        /// Gets or sets the underline style to apply to the
        /// current selection or insertion point.
        /// </summary>
        /// <remarks>
        /// Underline styles can be set to any value of the
        /// <see cref="UnderlineStyle"/> enumeration.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UnderlineStyle SelectionUnderlineStyle
        {
            get
            {
                CHARFORMAT fmt = new CHARFORMAT();
                fmt.cbSize = Marshal.SizeOf(fmt);

                // Get the underline style.
                SendMessage(new HandleRef(this, Handle), EM_GETCHARFORMAT,
                             SCF_SELECTION, ref fmt);

                // Default to no underline.
                if ((fmt.dwMask & CFM_UNDERLINETYPE) == 0)
                    return UnderlineStyle.None;

                if ((fmt.dwEffects & CFE_UNDERLINE) == 0)
                    return UnderlineStyle.None;

                byte style = (byte)(fmt.bUnderlineType & 0x0F);

                return (UnderlineStyle)style;
            }

            set
            {
                // Ensure we don't alter the color by accident.
                UnderlineColor color = SelectionUnderlineColor;

                // Ensure we don't show it if it shouldn't be shown.
                if (value == UnderlineStyle.None)
                    color = UnderlineColor.Black;

                CHARFORMAT fmt = new CHARFORMAT();
                fmt.cbSize = Marshal.SizeOf(fmt);
                fmt.dwMask = CFM_UNDERLINETYPE;
                if (value != UnderlineStyle.None)
                {
                    fmt.dwEffects = CFE_UNDERLINE;
                }
                fmt.bUnderlineType = (byte)((byte)value | (byte)color);

                // Set the underline type.
                SendMessage(new HandleRef(this, Handle), EM_SETCHARFORMAT,
                             SCF_SELECTION, ref fmt);
            }
        }

        /// <summary>
        /// Specifies the style of underline that should be
        /// applied to the text.
        /// </summary>
        public enum UnderlineStyle
        {
            /// <summary>
            /// No underlining.
            /// </summary>
            None = 0,

            /// <summary>
            /// Standard underlining across all words.
            /// </summary>
            Normal = 1,

            /// <summary>
            /// Standard underlining broken between words.
            /// </summary>
            Word = 2,

            /// <summary>
            /// Double line underlining.
            /// </summary>
            Double = 3,

            /// <summary>
            /// Dotted underlining.
            /// </summary>
            Dotted = 4,

            /// <summary>
            /// Dashed underlining.
            /// </summary>
            Dash = 5,

            /// <summary>
            /// Dash-dot ("-.-.") underlining.
            /// </summary>
            DashDot = 6,

            /// <summary>
            /// Dash-dot-dot ("-..-..") underlining.
            /// </summary>
            DashDotDot = 7,

            /// <summary>
            /// Wave underlining (like spelling mistakes in MS Word).
            /// </summary>
            Wave = 8,

            /// <summary>
            /// Extra thick standard underlining.
            /// </summary>
            Thick = 9,

            /// <summary>
            /// Extra thin standard underlining.
            /// </summary>
            HairLine = 10,

            /// <summary>
            /// Double thickness wave underlining.
            /// </summary>
            DoubleWave = 11,

            /// <summary>
            /// Thick wave underlining.
            /// </summary>
            HeavyWave = 12,

            /// <summary>
            /// Extra long dash underlining.
            /// </summary>
            LongDash = 13
        }

        /// <summary>
        /// Gets or sets the underline color to apply to the
        /// current selection or insertion point.
        /// </summary>
        /// <remarks>
        /// Underline colors can be set to any value of the
        /// <see cref="UnderlineColor"/> enumeration.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UnderlineColor SelectionUnderlineColor
        {
            get
            {
                CHARFORMAT fmt = new CHARFORMAT();
                fmt.cbSize = Marshal.SizeOf(fmt);

                // Get the underline color.
                SendMessage(new HandleRef(this, Handle), EM_GETCHARFORMAT,
                             SCF_SELECTION, ref fmt);

                // Default to black.
                if ((fmt.dwMask & CFM_UNDERLINETYPE) == 0)
                    return UnderlineColor.Black;

                if ((fmt.dwEffects & CFE_UNDERLINE) == 0)
                    return UnderlineColor.Black;

                byte style = (byte)(fmt.bUnderlineType & 0xF0);

                return (UnderlineColor)style;
            }

            set
            {
                // Ensure we don't alter the style.
                UnderlineStyle style = SelectionUnderlineStyle;

                // Ensure we don't show it if it shouldn't be shown.
                if (style == UnderlineStyle.None)
                    value = UnderlineColor.Black;

                CHARFORMAT fmt = new CHARFORMAT();
                fmt.cbSize = Marshal.SizeOf(fmt);
                fmt.dwMask = CFM_UNDERLINETYPE;
                if (style != UnderlineStyle.None)
                {
                    fmt.dwEffects = CFE_UNDERLINE;
                }
                
                fmt.bUnderlineType = (byte)((byte)style | (byte)value);

                // Set the underline color.
                SendMessage(new HandleRef(this, Handle), EM_SETCHARFORMAT,
                             SCF_SELECTION, ref fmt);
            }
        }

        /// <summary>
        /// Specifies the color of underline that should be
        /// applied to the text.
        /// </summary>
        /// <remarks>
        /// I named these colors by their appearance, so some
        /// of them might not be what you expect. Please email
        /// me if you feel one should be changed.
        /// </remarks>
        public enum UnderlineColor
        {
            /// <summary>Black.</summary>
            Black = 0x00,

            /// <summary>Blue.</summary>
            Blue = 0x10,

            /// <summary>Cyan.</summary>
            Cyan = 0x20,

            /// <summary>Lime green.</summary>
            LimeGreen = 0x30,

            /// <summary>Magenta.</summary>
            Magenta = 0x40,

            /// <summary>Red.</summary>
            Red = 0x50,

            /// <summary>Yellow.</summary>
            Yellow = 0x60,

            /// <summary>White.</summary>
            White = 0x70,

            /// <summary>DarkBlue.</summary>
            DarkBlue = 0x80,

            /// <summary>DarkCyan.</summary>
            DarkCyan = 0x90,

            /// <summary>Green.</summary>
            Green = 0xA0,

            /// <summary>Dark magenta.</summary>
            DarkMagenta = 0xB0,

            /// <summary>Brown.</summary>
            Brown = 0xC0,

            /// <summary>Olive green.</summary>
            OliveGreen = 0xD0,

            /// <summary>Dark gray.</summary>
            DarkGray = 0xE0,

            /// <summary>Gray.</summary>
            Gray = 0xF0
        }
    }
}
