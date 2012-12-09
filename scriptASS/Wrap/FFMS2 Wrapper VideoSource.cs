//Title: FFMS2 .NET Wrapper Alpha v 0.1
//Author: Francisco José Soto Portillo (_TheAway)
//You are free to use and share this code

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Collections;
using System.Drawing;

namespace FFMS2
{
    #region Model data
    /**
     *  Contiene los datos internos sobre cada track del video
     * */
    public struct TrackData
    {
        public int TrackType;
        public string Codec;
    }

    public struct FFMS_FrameInfo
    {
        public long PTS;
        public int RepeatPict;
        public int KeyFrame;
    } ;

    public class Track
    {
        public IntPtr PTrack;
        public string TrackName;
        public int TrackType;
    }

    enum FFMS_SeekMode {
	FFMS_SEEK_LINEAR_NO_RW  = -1,
	FFMS_SEEK_LINEAR        = 0,
	FFMS_SEEK_NORMAL        = 1,
	FFMS_SEEK_UNSAFE        = 2,
	FFMS_SEEK_AGGRESSIVE    = 3
};


    public unsafe struct FFMS_SourceData
    {
        public int NumTracks;
        public int NumFrames;
        public string Source;
        public ArrayList Tracks; //Arraylist de TrackData
    }

    public struct FFMS_Frame
    {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	public IntPtr[] Data;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
	public int[] Linesize;
	public int EncodedWidth;
	public int EncodedHeight;
	public int EncodedPixelFormat;
	public int ScaledWidth;
	public int ScaledHeight;
	public int ConvertedPixelFormat;
	public int KeyFrame;
	public int RepeatPict;
	public int InterlacedFrame;
	public int TopFieldFirst;
	public char PictType;
    }

    enum FFMS_Resizers
    {
        FFMS_RESIZER_FAST_BILINEAR = 0x01,
        FFMS_RESIZER_BILINEAR = 0x02,
        FFMS_RESIZER_BICUBIC = 0x04,
        FFMS_RESIZER_X = 0x08,
        FFMS_RESIZER_POINT = 0x10,
        FFMS_RESIZER_AREA = 0x20,
        FFMS_RESIZER_BICUBLIN = 0x40,
        FFMS_RESIZER_GAUSS = 0x80,
        FFMS_RESIZER_SINC = 0x100,
        FFMS_RESIZER_LANCZOS = 0x200,
        FFMS_RESIZER_SPLINE = 0x400
    };

    public struct  FFMS_VideoProperties{
	public int FPSDenominator;
	public int FPSNumerator;
	public int RFFDenominator;
	public int RFFNumerator;
	public int NumFrames;
	public int SARNum;
	public int SARDen;
	public int CropTop;
	public int CropBottom;
	public int CropLeft;
	public int CropRight;
	public int TopFieldFirst;
	public int ColorSpace;
	public int ColorRange;
	public double FirstTime;
	public double LastTime;
    };

    public struct FFMS_AudioProperties{
	public int SampleFormat;
	public int SampleRate;
	public int BitsPerSample;
	public int Channels;
	public IntPtr ChannelLayout;
	public IntPtr NumSamples;
	public double FirstTime;
	public double LastTime;
    };

    #endregion

    #region VideoSource
    public class FFMS2Track
    {
        ArrayList TrackList;
        FFMS_SourceData Data;
        IntPtr VideoSource;
        IntPtr AudioSource;
        FFMS_ErrorInfo Error;



        #region dinamic library functions
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr FFMS_GetTrackFromIndex(IntPtr Index, int Track);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr FFMS_GetFrameInfo(IntPtr Track, int Frame);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int FFMS_GetNumFrames(IntPtr Track);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr FFMS_CreateVideoSource(string SourceFile, int Track, IntPtr FFMS_Index, int Threads, int SeekMode, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr FFMS_CreateAudioSource(string SourceFile, int Track, IntPtr Index, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr FFMS_GetFrame(IntPtr VideoSource, int n, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int FFMS_SetOutputFormatV(IntPtr VideoSource, Int64 TargetFormats, int Width, int Height, int Resizer, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int FFMS_GetPixFmt(string Name);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr FFMS_GetVideoProperties(IntPtr VideoSource);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr FFMS_GetAudioProperties(IntPtr AudioSource);
        #endregion

        #region Track

        public FFMS2Track(FFMS_SourceData Data)
        {
            this.Data = Data;
            TrackList = new ArrayList();
            this.Error = new FFMS_ErrorInfo();
        }

        public void GetTrack(IntPtr Index, int Track)
        {
            Track Dummytrack = new Track();
            Dummytrack.PTrack = FFMS_GetTrackFromIndex(Index, Track);
            Dummytrack.TrackName = (string)((TrackData)Data.Tracks[Track]).Codec;
            Dummytrack.TrackType = ((TrackData)Data.Tracks[Track]).TrackType;
            TrackList.Add(Dummytrack);
        }

        public FFMS_ErrorInfo GetError()
        {
            return Error;
        }

        public FFMS_FrameInfo GetFrameInfo(int Track, int Frame)
        {
            IntPtr track = ((Track)TrackList[Track]).PTrack;
            IntPtr ffmsfi = FFMS_GetFrameInfo(track, Frame);
            FFMS_FrameInfo ffmsifi1 = (FFMS_FrameInfo)Marshal.PtrToStructure(ffmsfi, typeof(FFMS_FrameInfo));
            return ffmsifi1;
        }

        public int GetNumFrames(int Track)
        {
            IntPtr track = ((Track)TrackList[Track]).PTrack;
            return FFMS_GetNumFrames(track);
        }

        #endregion
        #region VideoSource
        /**
         * Create a new VideoSource from File 
         * 
         * If Track integer isn't a video track throw an exception 
         * 
         * */

        public void CreateVideoSource(string File, int Track, IntPtr Index, int Threads, int SeekMode)
        {
            Track track = (Track)TrackList[Track];
            if (track.TrackType != (int)FFMS_TrackType.FFMS_TYPE_VIDEO)
                this.VideoSource = FFMS_CreateVideoSource(File, Track, Index, Threads, SeekMode, ref Error);
            else
                throw new Exception("Track n#" + Track + " no es una pista de Video");
        }

        /**
       * Selecciona el formato de salida del video.
       * 
       * Debe inicializarse ante el VideoSource
       * 
       * devuelve 0 si no hay errores
       * 
       * */
        public int SetVideoOutput(int Width, int Height, int Resizer, string TargetFormat)
        {
            int ret = 0;

            if (VideoSource == IntPtr.Zero)
                throw new Exception("No se ha inicializado el Video, use CreateVideoSource");

            Int64 TargetFormats = (1 << FFMS_GetPixFmt(TargetFormat));
            if (TargetFormats == -1)
                throw new Exception("El formato de salida de video indicado no existe.");
            ret = FFMS_SetOutputFormatV(VideoSource, TargetFormats, Width, Height, Resizer, ref Error);
            return ret;
        }

        /**
        * Selecciona y devuelve un frame del video
         * 
         * Debe inicializarse ante el VideoSource
         * 
         * devuelve el objeto FFMS_Frame
         * 
         * */

        public FFMS_Frame GetFrame(int NumFrame)
        {
            if (VideoSource == IntPtr.Zero)
                throw new Exception("No se ha inicializado el Video, use CreateVideoSource");

            IntPtr pFrame = FFMS_GetFrame(this.VideoSource, NumFrame, ref Error);
            FFMS_Frame frame = (FFMS_Frame)Marshal.PtrToStructure(pFrame, typeof(FFMS_Frame));

            return frame;
        }

        public FFMS_VideoProperties GetVideoProperties()
        {
            if (VideoSource == IntPtr.Zero)
                throw new Exception("No se ha inicializado el Video, use CreateVideoSource");

            IntPtr pVideo = FFMS_GetVideoProperties(this.VideoSource);
            FFMS_VideoProperties video = (FFMS_VideoProperties)Marshal.PtrToStructure(pVideo, typeof(FFMS_VideoProperties));

            return video;
        }

        public Bitmap ConvertToBitmap(FFMS_Frame frame)
        {
            Bitmap ret = null;
            if (frame.ConvertedPixelFormat == 30)
                ret = new Bitmap(frame.ScaledWidth, frame.ScaledHeight, frame.ScaledWidth * 4, System.Drawing.Imaging.PixelFormat.Format32bppRgb, frame.Data[0]);
            return ret;
        }


        #endregion
        #region AudioSource
        /**
         * Create a new AudioSource from File 
         * 
         * If Track integer isn't a audio track throw an exception 
         * 
         * */

        public void CreateAudioSource(string File, int Track, IntPtr Index)
        {
            Track track = (Track)TrackList[Track];
            if (track.TrackType != (int)FFMS_TrackType.FFMS_TYPE_AUDIO)
                this.AudioSource = FFMS_CreateAudioSource(File, Track, Index, ref Error);
            else
                throw new Exception("Track n#" + Track + " no es una pista de Audio");
        }

        public FFMS_AudioProperties GetAudioProperties()
        {
            if (VideoSource == IntPtr.Zero)
                throw new Exception("No se ha inicializado el Audio, use CreateAudioSource");

            IntPtr pAudio = FFMS_GetVideoProperties(this.VideoSource);
            FFMS_AudioProperties audio = (FFMS_AudioProperties)Marshal.PtrToStructure(pAudio, typeof(FFMS_AudioProperties));

            return audio;
        }

        #endregion

    }
    #endregion
}