using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Collections;

namespace scriptASS
{


    public enum AviSynthColorspace : int
    {
        Unknown = 0,
        YV12 = -1610612728,
        RGB24 = +1342177281,
        RGB32 = +1342177282,
        YUY2 = -1610612740,
        I420 = -1610612720,
        IYUV = I420
    }

    public class AviSynthException : PerrySubException
    {


        public AviSynthException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public AviSynthException(string message)
            : base(message)
        {
        }

        public AviSynthException()
            : base()
        {
        }

        public AviSynthException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }


    public enum AudioSampleType : int
    {
        Unknown = 0,
        INT8 = 1,
        INT16 = 2,
        INT24 = 4,    // Int24 is a very stupid thing to code, but it's supported by some hardware.
        INT32 = 8,
        FLOAT = 16
    };

    public sealed class AviSynthScriptEnvironment : IDisposable
    {
        public static string GetLastError()
        {
            return null;
        }

        public AviSynthScriptEnvironment()
        {
        }

        public IntPtr Handle
        {
            get
            {
                return new IntPtr(0);
            }
        }

        public AviSynthClip OpenScriptFile(string filePath, AviSynthColorspace forceColorspace)
        {
            return new AviSynthClip("Import", filePath, forceColorspace, this);
        }

        public AviSynthClip ParseScript(string script, AviSynthColorspace forceColorspace)
        {
            return new AviSynthClip("Eval", script, forceColorspace, this);
        }

        public AviSynthClip New(AviSynthColorspace forceColorspace)
        {
            return new AviSynthClip(forceColorspace, this);
        }

        public AviSynthClip New()
        {
            return New(AviSynthColorspace.RGB32);
        }


        public AviSynthClip OpenScriptFile(string filePath)
        {
            return OpenScriptFile(filePath, AviSynthColorspace.RGB24);
        }

        public AviSynthClip ParseScript(string script)
        {
            return ParseScript(script, AviSynthColorspace.RGB24);
        }


        void IDisposable.Dispose()
        {

        }
    }

    /// <summary>
    /// Summary description for AviSynthClip.
    /// </summary>
    public class AviSynthClip : IDisposable
    {
        #region PInvoke related stuff
        [StructLayout(LayoutKind.Sequential)]
        struct AVSDLLVideoInfo
        {
            public int width;
            public int height;
            public int raten;
            public int rated;
            public int aspectn;
            public int aspectd;
            public int interlaced_frame;
            public int top_field_first;
            public int num_frames;
            public AviSynthColorspace pixel_type;

            // Audio
            public int audio_samples_per_second;
            public AudioSampleType sample_type;
            public int nchannels;
            public int num_audio_frames;
            public long num_audio_samples;
        }


        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avs_init(ref IntPtr avs, string func, string arg, ref AVSDLLVideoInfo vi, ref AviSynthColorspace originalColorspace, ref AudioSampleType originalSampleType, string cs);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avs_destroy(ref IntPtr avs);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avs_getlasterror(IntPtr avs, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sb, int len);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avs_getaframe(IntPtr avs, IntPtr buf, long sampleNo, long sampleCount);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avs_getvframe(IntPtr avs, int clp, IntPtr buf, int stride, int frm);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avs_getintvariable(IntPtr avs, string name, ref int val);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avs_invoke(IntPtr avs, int clp, string func, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, bool clip, ref AVSDLLVideoInfo vi);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avs_cliptoclip(IntPtr avs, int clp1, int clp2);
        [DllImport("AvisynthWrapper", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern bool avs_funcexists(IntPtr avs, string func);

        #endregion

        private IntPtr _avs;
        private AVSDLLVideoInfo _vi;
        public AviSynthColorspace _colorSpace;
        private AudioSampleType _sampleType;

#if dimzon

        #region syncronization staff

        private class EnvRef
        {
            public IntPtr env;
            public long refCount;
            public object lockObj;
            public int threadID;

            public EnvRef(IntPtr e, int threadID)
            {
                this.env = e;
                refCount = 1;
                lockObj = new object();
                this.threadID = threadID;
            }
        }
        private static Hashtable _threadHash = new Hashtable();

        private EnvRef createNewEnvRef(int threadId)
        {
            //TODO:
            return new EnvRef(new IntPtr(0), threadId);
        }

        private void destroyEnvRef(EnvRef r)
        {
            //TODO:
        }

        private EnvRef addRef()
        {
            int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            lock (_threadHash.SyncRoot)
            {
                EnvRef r;
                if(_threadHash.ContainsKey(threadId))
                {
                    r = (EnvRef)_threadHash[threadId];
                    lock(r.lockObj)
                    {
                        if (0 == r.refCount)
                        {
                            r = createNewEnvRef(threadId);
                            _threadHash.Remove(threadId);
                            _threadHash.Add(threadId, r);
                        }
                        else
                        {
                            ++r.refCount;
                        }
                    }
                }
                else
                {
                    r = createNewEnvRef(threadId);
                    _threadHash.Add(threadId, r);
                }
                return r;
            }
        }

        private void Release()
        {
            if (_avsEnv == null)
                return;
            lock (_avsEnv.lockObj)
            {
                --_avsEnv.refCount;
                if (0 == _avsEnv.refCount)
                {
                    destroyEnvRef(_avsEnv);
                }
            }
            _avsEnv = null;
        }

        private EnvRef _avsEnv;


        #endregion
#endif

        public string getLastError()
        {
            const int errlen = 1024;
            StringBuilder sb = new StringBuilder(errlen);
            sb.Length = avs_getlasterror(_avs, sb, errlen);
            return sb.ToString();
        }

        #region Clip Properties

        public bool HasVideo
        {
            get
            {
                return VideoWidth > 0 && VideoHeight > 0;
            }
        }

        public int VideoWidth
        {
            get
            {
                return _vi.width;
            }
        }
        public int VideoHeight
        {
            get
            {
                return _vi.height;
            }
        }
        public int raten
        {
            get
            {
                return _vi.raten;
            }
        }
        public int rated
        {
            get
            {
                return _vi.rated;
            }
        }
        public int aspectn
        {
            get
            {
                return _vi.aspectn;
            }
        }
        public int aspectd
        {
            get
            {
                return _vi.aspectd;
            }
        }
        public int interlaced_frame
        {
            get
            {
                return _vi.interlaced_frame;
            }
        }
        public int top_field_first
        {
            get
            {
                return _vi.top_field_first;
            }
        }
        public int num_frames
        {
            get
            {
                return _vi.num_frames;
            }
        }
        // Audio
        public int AudioSampleRate
        {
            get
            {
                return _vi.audio_samples_per_second;
            }
        }

        public long SamplesCount
        {
            get
            {
                return _vi.num_audio_samples;
            }
        }
        public AudioSampleType SampleType
        {
            get
            {
                return _vi.sample_type;
            }
        }
        public short ChannelsCount
        {
            get
            {
                return (short)_vi.nchannels;
            }
        }

        public AviSynthColorspace PixelType
        {
            get
            {
                return _vi.pixel_type;
            }
        }

        public AviSynthColorspace OriginalColorspace
        {
            get
            {
                return _colorSpace;
            }
        }
        public AudioSampleType OriginalSampleType
        {
            get
            {
                return _sampleType;
            }
        }


        #endregion


        public IntPtr GetAVS()
        {
            return this._avs;
        }

        public bool FuncExists(IntPtr avs, string func)
        {
            return avs_funcexists(avs, func);
        }

        public void AviSynthInvoke(IntPtr avs, int clp, string func, bool clip, params string[] args)
        {
            int i;
            //AviSynthClip res = new AviSynthClip(avs);

            string[] arg = new string[10];
            for (i = 0; i < args.Length; i++)
            {
                arg[i] = args[i];
            }
            for (i = args.Length; i < 10; i++)
            {
                arg[i] = "Nulo";
            }

            if (0 != avs_invoke(avs, clp, func, arg[0], arg[1], arg[2], arg[3], arg[4], arg[5], arg[6], arg[7], arg[8], arg[9], clip, ref _vi))
            {
                string err = getLastError();
                cleanup(false);
                throw new AviSynthException(err);
            }
            /* res._vi.aspectd = _vi.aspectd;
             res._vi.aspectn = _vi.aspectn;
             res._vi.audio_samples_per_second = _vi.audio_samples_per_second;
             res._vi.height = _vi.height;
             res._vi.interlaced_frame = _vi.interlaced_frame;
             res._vi.nchannels = _vi.nchannels;
             res._vi.num_audio_frames = _vi.num_audio_frames;
             res._vi.num_audio_samples = _vi.num_audio_samples;
             res._vi.num_frames = _vi.num_frames;
             res._vi.pixel_type = _vi.pixel_type;
             res._vi.rated = _vi.rated;
             res._vi.raten = _vi.raten;
             res._vi.sample_type = _vi.sample_type;
             res._vi.top_field_first = _vi.top_field_first;
             res._vi.width= _vi.width;*/




        }

        public int GetIntVariable(string variableName, int defaultValue)
        {
            int v = 0;
            int res = 0;
            res = avs_getintvariable(this._avs, variableName, ref v);
            if (res < 0)
                throw new AviSynthException(getLastError());
            return (0 == res) ? v : defaultValue;
        }

        public void ReadAudio(IntPtr addr, long offset, int count)
        {
            if (0 != avs_getaframe(_avs, addr, offset, count))
                throw new AviSynthException(getLastError());

        }

        public void ReadAudio(byte[] buffer, long offset, int count)
        {
            GCHandle h = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                ReadAudio(h.AddrOfPinnedObject(), offset, count);
            }
            finally
            {
                h.Free();
            }
        }

        public void ReadFrame(IntPtr addr, int clp, int stride, int frame)
        {
            if (0 != avs_getvframe(_avs, clp, addr, stride, frame))
                throw new AviSynthException(getLastError());
        }


        public void ClipExchange(IntPtr avs, int clp1, int clp2)
        {

            if (0 != avs_cliptoclip(avs, clp1, clp2))
                throw new AviSynthException(getLastError());

        }

        public AviSynthClip(string func, string arg, AviSynthColorspace forceColorspace, AviSynthScriptEnvironment env)
        {

            _vi = new AVSDLLVideoInfo();
            _avs = new IntPtr(0);
            _colorSpace = AviSynthColorspace.Unknown;
            _sampleType = AudioSampleType.Unknown;
            if (0 != avs_init(ref _avs, func, arg, ref _vi, ref _colorSpace, ref _sampleType, forceColorspace.ToString()))
            {
                string err = getLastError();
                cleanup(false);
                throw new AviSynthException(err);
            }
        }

        public AviSynthClip(AviSynthColorspace forceColorspace, AviSynthScriptEnvironment env)
        {

            _vi = new AVSDLLVideoInfo();
            _avs = new IntPtr(0);
            _colorSpace = AviSynthColorspace.Unknown;
            _sampleType = AudioSampleType.Unknown;
            if (0 != avs_init(ref _avs, "blankclip", "", ref _vi, ref _colorSpace, ref _sampleType, forceColorspace.ToString()))
            {
                string err = getLastError();
                cleanup(false);
                throw new AviSynthException(err);
            }
        }

        public AviSynthClip(IntPtr avs)
        {

            _vi = new AVSDLLVideoInfo();
            this._avs = avs;
            _colorSpace = AviSynthColorspace.RGB32;
            _sampleType = AudioSampleType.INT8;

        }


        public void cleanup(bool disposing)
        {
            avs_destroy(ref _avs);
            _avs = new IntPtr(0);
            if (disposing)
                GC.SuppressFinalize(this);
        }

        ~AviSynthClip()
        {
            cleanup(false);
        }

        void IDisposable.Dispose()
        {
            cleanup(true);
        }
        public short BitsPerSample
        {
            get
            {
                return (short)(BytesPerSample * 8);
            }
        }
        public short BytesPerSample
        {
            get
            {
                switch (SampleType)
                {
                    case AudioSampleType.INT8:
                        return 1;
                    case AudioSampleType.INT16:
                        return 2;
                    case AudioSampleType.INT24:
                        return 3;
                    case AudioSampleType.INT32:
                        return 4;
                    case AudioSampleType.FLOAT:
                        return 4;
                    default:
                        throw new ArgumentException(SampleType.ToString());
                }
            }
        }

        public int AvgBytesPerSec
        {
            get
            {
                return AudioSampleRate * ChannelsCount * BytesPerSample;
            }
        }

        public long AudioSizeInBytes
        {
            get
            {
                return SamplesCount * ChannelsCount * BytesPerSample;
            }
        }

    }

}
