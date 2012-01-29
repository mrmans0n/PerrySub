using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace scriptASS
{
    class AVSTextBox : RichTextBox
    {
        private const int WMisParsing = 0x00f;
        private const int WM_USER = 0x400;
        private const int EM_GETSCROLLPOS = (WM_USER + 221);
        private const int EM_SETSCROLLPOS = (WM_USER + 222);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);
        [DllImport("user32")]
        public static extern int PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        [DllImport("user32")]
        public static extern int LockWindowUpdate(IntPtr hwnd);

        public static bool isParsing = true;

        private string[] avs_core = 
          { "AddBorders","Amplify","AmplifydB","Animate","ApplyRange","AssumeFrameBased","AssumeFieldBased","AssumeBFF","AssumeTFF","AssumeSampleRate","AssumeScaledFPS","AudioDub","AudioDubEx","AVISource","OpenDMLSource","AVIFileSource","WAVSource",
            "BlankClip", "Blackness","Blur","Sharpen","Bob","ColorBars","ColorYUV","ComplementParity","Compare","ConditionalFilter","FrameEvaluate","ScriptClip","ConditionalReader","ConvertToBackYUY2","ConvertToRGB","ConvertToRGB24","ConvertToRGB32", "ConvertToY8","ConvertToYUY2","ConvertToYV12",
            "ConvertToYV16","ConvertToYV24","ConvertToYV411","ConvertAudioTo8bit","ConvertAudioTo16bit","ConvertAudioTo24bit","ConvertAudioTo32bit","ConvertAudioToFloat",
            "ConvertToMono", "Crop", "Cropbottom", "DelayAudio", "DeleteFrame", "DirectShowSource", "Dissolve", "DoubleWeave", "DuplicateFrame", "EnsureVBRMP3Sync", "FadeIn0", "FadeIn", "FadeIn2", "FadeIO0", "FadeIO", "FadeIO2", "FadeOut0", "FadeOut", "FadeOut2", "FixBrokenChromaUpsampling",
            "FixLuminance", "FlipHorizontal", "FlipVertical","AssumeFPS", "ChangeFPS", "ConvertFPS", "FreezeFrame", "GeneralConvolution", "GetChannel", "GetChannels", "Greyscale", "Histogram", "ImageReader", "ImageSource", "ImageWriter", "Info", "Interleave", "Invert", "KillAudio", "Layer",
            "Mask", "ResetMask", "ColorKeyMask", "Letterbox", "Levels", "Limiter", "Loop", "MergeARGB", "MergeRGB", "MergeChannels", "Merge", "MergeChroma", "MergeLuma", "MessageClip", "MixAudio", "Normalize", "Overlay", "PeculiarBlend", "Pulldown", "RGBAdjust", "HorizontalReduceBy2", "VerticalReduceBy2", 
            "ReduceBy2", "ResampleAudio", "BilinearResize", "BicubicResize", "LanczosResize","Lanczos4Resize","Spline16Resize", "Spline36Resize", "GaussResize", "PointResize", "Reverse", "SegmentedAVISource", "SegmentedDirectShowSource", "SelectEven", "SelectOdd", "SelectEvery", "SelectRangeEver", "SeparateFields",
            "ShowAlpha", "ShowRed", "ShowGreen", "ShowBlue", "ShowFiveVersions", "ShowFrameNumber", "ShowSMPTE", "SpatialSoften", "TemporalSoften", "AlignedSplice", "UnAlignedSplice", "SSRC", "StackHorizontal", "StackVertical", "Subtitle", "Subtract", "SuperEQ", "SwapUV", "UToY", "UToY8", "VToY", "VToY8","YToUV", 
            "SwapFields", "TCPServer", "TCPSource", "TimeStretch", "Tone", "Trim", "TurnLeft", "TurnRight", "Turn180", "Tweak", "Version", "Weave", "WriteFile", "WriteFileIf", "WriteFileStart", "WriteFileEnd","import","function","defined","default","return"
          };
        private string[] avs_reserved = {
            "bool","string","int","float","clip","true","false"
        };

        private char commentToken = '#';

        private Color resWords = Color.Red;
        private Color coreWords = Color.Blue;
        private Color sWords = Color.Brown;
        private Color cWords = Color.Green;
        private Color integers = Color.Crimson;

        public Color CoreColor
        {
            get { return coreWords; }
            set { coreWords = value; }
        }

        public Color IntegerColor
        {
            get { return integers; }
            set { integers = value; }
        }
        
        public Color CommentColor
        {
            get { return cWords; }
            set { cWords = value; }
        }
        public Color StringColor
        {
            get { return sWords; }
            set { sWords = value; }
        }

        public Color ReservedWordsColor
        {
            get { return resWords; }
            set { resWords = value; }
        }

        /*
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            
            if (m.Msg == WMisParsing)
            {
                if (isParsing) base.WndProc(ref m);
                else
                    m.Result = IntPtr.Zero;
            }
            else 
                base.WndProc(ref m);
        }
        */
        
        private void PaintReservedWords(AVSTextBox avstext)
        {
            string actual = avstext.Text;
            avstext.SelectAll();
            avstext.SelectionColor = avstext.ForeColor;

            Regex r;
            MatchCollection mc;
            for (int j = 0; j < avs_core.Length; j++)
            {
                r = new Regex(@"\b" + avs_core[j].ToLower() + @"\b");
                mc = r.Matches(actual.ToLower());

                foreach (Match m in mc)
                {
                    avstext.Select(m.Index, m.Length);
                    avstext.SelectionColor = CoreColor;
                    avstext.SelectionFont = new Font(Font, FontStyle.Bold);
                }

            }

            for (int j = 0; j < avs_reserved.Length; j++)
            {
                r = new Regex(@"\b" + avs_reserved[j].ToLower() + @"\b");
                mc = r.Matches(actual.ToLower());

                foreach (Match m in mc)
                {
                    avstext.Select(m.Index, m.Length);
                    avstext.SelectionColor = ReservedWordsColor;
                    avstext.SelectionFont = new Font(Font, FontStyle.Bold);
                }

            }


            r = new Regex("\\b(?:[0-9]*\\.)?[0-9]+\\b");
            mc = r.Matches(actual.ToLower());

            foreach (Match m in mc)
            {
                avstext.Select(m.Index, m.Length);
                avstext.SelectionColor = IntegerColor;
            }

            r = new Regex("\"[^\"\\\\\\r\\n]*(?:\\\\.[^\"\\\\\\r\\n]*)*\"");
            mc = r.Matches(actual.ToLower());

            foreach (Match m in mc)
            {
                avstext.Select(m.Index, m.Length);
                avstext.SelectionColor = StringColor;
                avstext.SelectionFont = new Font(Font, FontStyle.Bold);
            }

            r = new Regex(commentToken + @".*");
            mc = r.Matches(actual.ToLower());

            foreach (Match m in mc)
            {
                avstext.Select(m.Index, m.Length);
                avstext.SelectionColor = CommentColor;
                avstext.SelectionFont = new Font(Font, FontStyle.Bold);
            }

        }

        private unsafe POINT GetScrollPos()
        {
            POINT res = new POINT();
            IntPtr ptr = new IntPtr(&res);
            SendMessage(this.Handle, EM_GETSCROLLPOS, 0, ptr);
            return res;

        }

        private unsafe void SetScrollPos(POINT point)
        {
            IntPtr ptr = new IntPtr(&point);
            SendMessage(this.Handle, EM_SETSCROLLPOS, 0, ptr);

        }


        private void ParseText()
        {
            isParsing = false;

            LockWindowUpdate(this.Handle);
            
            POINT scrollPos = GetScrollPos();

            int sel_start = SelectionStart;
            int sel_length = SelectionLength;

            PaintReservedWords(this);

            SelectionStart = sel_start;
            SelectionLength = sel_length;

            SetScrollPos(scrollPos);
            LockWindowUpdate((IntPtr)0);

            isParsing = true;
        }

        public void ForceRefresh()
        {
            ParseText();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            ParseText();
        }



        protected override void OnVScroll(EventArgs e)
        {
            if (!isParsing) return;
            base.OnVScroll(e);
        }

    }
}
