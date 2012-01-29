//Title: FFMS2 .NET Wrapper Alpha v 0.1
//Author: Francisco José Soto Portillo (_TheAway)
//You are free to use and share this code

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Collections;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using FFMS2;

namespace FFMS2
{
    #region Model data
    /**
     *  Contiene los datos internos sobre cada track del video
     * */
    enum FFMS_CPUFeatures : int
    {
        FFMS_CPU_CAPS_MMX = 0x01,
        FFMS_CPU_CAPS_MMX2 = 0x02,
        FFMS_CPU_CAPS_3DNOW = 0x04,
        FFMS_CPU_CAPS_ALTIVEC = 0x08,
        FFMS_CPU_CAPS_BFIN = 0x10
    };

    public unsafe struct FFMS_ErrorInfo
    {
        int ErrorType;
        int SubType;
        int BufferSize;
        char* Buffer;
    };

    enum FFMS_TrackType : int
    {
        FFMS_TYPE_UNKNOWN = -1,
        FFMS_TYPE_VIDEO = 0,
        FFMS_TYPE_AUDIO = 1,
        FFMS_TYPE_DATA = 2,
        FFMS_TYPE_SUBTITLE = 3,
        FFMS_TYPE_ATTACHMENT = 4
    };

    #endregion

    public class FFMSWrapper : IDisposable
    {
        private IntPtr Indexer;
        private IntPtr Index;
        private FFMS_ErrorInfo Error;
        private IntPtr VideoSource;
        private int PrivateCallBack;
        public bool isIndexing;
        private bool OnlyVideo;
        public FFMS_SourceData SourceData;
        public ProgressBar ProgressBarIndex;

        public delegate int CallBack(string SourceFile, int Track, ref FFMS_AudioProperties AP, IntPtr FileName, int FNSize, IntPtr Private);
        public delegate int CallBack2(long Current, long Total, IntPtr ICPrivate);

        /* CAMBIOS MRM */

        public delegate void IndexingProgressChange(object sender, IndexingProgressChangeEventArgs e);
        public delegate void IndexingFinished(object sender);

        public class IndexingProgressChangeEventArgs : EventArgs
        {
            private long actualIndex;
            private long totalIndex;

            public long ActualIndex
            {
                get { return actualIndex; }
            }

            public long TotalIndex
            {
                get { return totalIndex; }
            }

            public IndexingProgressChangeEventArgs(long ActualIndex, long TotalIndex)
            {
                actualIndex = ActualIndex;
                totalIndex = TotalIndex;
            }
        }

        public event IndexingProgressChange UpdateIndexProgress;
        public event IndexingFinished IndexingCompleted;

        #region dinamic library functions

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int FFMS_Init(int fcpu);
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr FFMS_CreateIndexer(string SourceFile, ref FFMS_ErrorInfo Error);
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int FFMS_GetNumTracksI(IntPtr Indexer);
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int FFMS_GetTrackTypeI(IntPtr Indexer, int Track);
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr FFMS_GetCodecNameI(IntPtr Indexer, int Track);
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr FFMS_DoIndexing(IntPtr Indexer, int IndexMask, int DumpMask,
        CallBack TAudioNameCallback, IntPtr ANCPrivate, bool IgnoreDecodeErrors, CallBack2 TIndexCallback, IntPtr ICPrivate,
        ref FFMS_ErrorInfo ErrorInfo);
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern void FFMS_CancelIndexing(IntPtr Indexer);
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr FFMS_ReadIndex(string IndexFile, ref FFMS_ErrorInfo ErrorInfo);
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int FFMS_WriteIndex(string IndexFile, IntPtr TrackIndices, ref FFMS_ErrorInfo ErrorInfo);
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr FFMS_CreateVideoSource(string SourceFile, int Track, IntPtr Index,
        int Threads, int SeekMode, ref FFMS_ErrorInfo ErrorInfo);

        public enum CoInit
        {
            MultiThreaded = 0x0,
            ApartmentThreaded = 0x2,
            DisableOLE1DDE = 0x4,
            SpeedOverMemory = 0x8
        }

        [DllImport("ole32.dll")]
        public static extern int CoInitializeEx(IntPtr pvReserved, CoInit coInit);
        [DllImport("ole32.dll")]
        static extern int CoInitialize(IntPtr pvReserved);

        [DllImport("kernel32.dll")]
        static extern bool IsProcessorFeaturePresent(uint ProcessorFeature);

        private const int PF_MMX_INSTRUCTIONS_AVAILABLE = 3;        // MMX (1 supongo, 2 no veo que este)
        private const int PF_3DNOW_INSTRUCTIONS_AVAILABLE = 7;      // 3DNOW
        private const int PF_XMMI_INSTRUCTIONS_AVAILABLE = 6;       // SSE
        private const int PF_XMMI64_INSTRUCTIONS_AVAILABLE = 10;    // SSE2
        private const int PF_SSE3_INSTRUCTIONS_AVAILABLE = 13;      // SSE3

        #endregion
        #region FFMSWrapper Initialization and destruction
        public FFMSWrapper()
            : this(false)
        {
        }

        public FFMSWrapper(bool OnlyVideo)
        {
            this.isIndexing = false;
            this.PrivateCallBack = 0;
            this.OnlyVideo = OnlyVideo;
            CoInitializeEx(IntPtr.Zero, CoInit.MultiThreaded);

            // detección CPU
            int flags = 0;
            if (IsProcessorFeaturePresent(PF_3DNOW_INSTRUCTIONS_AVAILABLE))
                flags &= (int)FFMS_CPUFeatures.FFMS_CPU_CAPS_3DNOW;
            if (IsProcessorFeaturePresent(PF_MMX_INSTRUCTIONS_AVAILABLE))
            {
                flags &= (int)FFMS_CPUFeatures.FFMS_CPU_CAPS_MMX2 & (int)FFMS_CPUFeatures.FFMS_CPU_CAPS_MMX;
            }

            FFMS_Init(flags);

        }

        ~FFMSWrapper()
        {
            cleanup(false);
        }

        void IDisposable.Dispose()
        {
            cleanup(true);
        }

        public void cleanup(bool disposing)
        {
            this.PrivateCallBack = 1;
            while (this.isIndexing) ;
            if (Indexer != IntPtr.Zero)
                FFMS_CancelIndexing(Indexer);
            if (disposing)
                GC.SuppressFinalize(this);
        }

        #endregion

        #region get/set

        public void setPrivateCallBack(int PBC)
        {
            this.PrivateCallBack = PBC;
        }

        public int getPrivateCallBack()
        {
            return this.PrivateCallBack;
        }

        #endregion

        #region Wrapper Indexer
        /**
         * 
         * Dado un fichero multimedia rellenará el atributo público SourceData con los datos del fichero.
         * Aparte se inicializará la precarga de los codecs.
         * 
         * */
        public void FFMSIndexer(string SourceFile)
        {
            FFMS_ErrorInfo ErrorInd = new FFMS_ErrorInfo();
            int tracks = 0;

            unsafe
            {
                Indexer = FFMS_CreateIndexer(SourceFile, ref ErrorInd);
                tracks = FFMS_GetNumTracksI(Indexer);
            }


            if (Indexer == IntPtr.Zero)
                Error = ErrorInd;
            SourceData.NumTracks = tracks;
            SourceData.Source = SourceFile;
            ArrayList TrackData = new ArrayList();
            if (this.OnlyVideo)
            {
                for (int i = 0; i < tracks; i++)
                {
                    try
                    {
                        int type = FFMS_GetTrackTypeI(Indexer, i);
                        if (type == (int)FFMS_TrackType.FFMS_TYPE_VIDEO)
                        {
                            string codec = Marshal.PtrToStringAnsi(FFMS_GetCodecNameI(Indexer, i));
                            TrackData data = new TrackData();
                            data.Codec = codec;
                            data.TrackType = type;
                            TrackData.Add(data);
                        }
                    }
                    catch (System.AccessViolationException) { }
                }
            }
            else
            {
                for (int i = 0; i < tracks; i++)
                {
                    try
                    {
                        int type = FFMS_GetTrackTypeI(Indexer, i);
                        string codec = Marshal.PtrToStringAnsi(FFMS_GetCodecNameI(Indexer, i));
                        TrackData data = new TrackData();
                        data.Codec = codec;
                        data.TrackType = type;
                        TrackData.Add(data);
                    }
                    catch (System.AccessViolationException) { }
                }
            }
            SourceData.Tracks = TrackData;
        }

        /**
         * 
         * Devuelve el IntPtr con el Index del fichero
         * 
         * */

        public IntPtr GetIndex()
        {
            return Index;
        }
        /**
         * 
         * Dado un fichero multimedia ya inicializado.
         * Realiza el estudio de Keyframes y funciones del video.
         * 
         * */
        public void FFMSDoIndex(bool DumpActive)
        {
            this.isIndexing = true;
            if (Indexer == IntPtr.Zero)
                throw new Exception("Indexer no cargado.");
            int IndexMask = -1;
            int DumpMask = 0;
            if (DumpActive)
                DumpMask = 1;
            bool IgnoreDecodeErrors = true;
            CallBack TAudioNameCallback = new CallBack(AudioName);
            CallBack2 IndexCB = new CallBack2(IndexCBI);
            Index = FFMS_DoIndexing(Indexer, IndexMask, DumpMask, TAudioNameCallback, System.IntPtr.Zero, IgnoreDecodeErrors, IndexCB, IntPtr.Zero, ref Error);
            this.isIndexing = false;
            Indexer = IntPtr.Zero;
        }

        /**
         * 
         * Escribe el Index del fichero actual al fichero indicado.
         * 
         * */
        public void FFMSWriteIndex(string IndexFile)
        {
            if (Index != IntPtr.Zero)
                FFMS_WriteIndex(IndexFile, Index, ref Error);
        }

        /**
         * 
         * Lee el Index del fichero indicado.
         * Cuidado, si no estas seguro que es el mismo fichero esto puede dar a graves errores al descodificar el video.
         * 
         * */
        public void FFMSReadIndex(string IndexFile)
        {
            Index = FFMS_ReadIndex(IndexFile, ref Error);
        }
        #endregion
        #region Callbacks functions
        /**
         * Callback para crear el nombre del fichero del dump de audio.
         * 
         * En la primera llamada FileName es null y ha de devolverse el numero de caracteres que tendrá tu nombre más uno
         * 
         * En la segunda pasada FileName es un string y ha de rellenarse con el nombre generado. 
         * Ha de devolverse entonces el número de caracteres del nombre generado más uno.
         * 
         * SourceFile es el nombre de origen, y siempre es enviado.
         * Track es el número de la pista.
         * AP es un struct de FFMS_AudioProperties con los datos del audio.
         * FNSize es el tamaño de FileName.
         * Private es un puntero enviado por Do_Indexing
         * */
        public int AudioName(string SourceFile, int Track, ref FFMS_AudioProperties AP, IntPtr FileName, int FNSize, IntPtr Private)
        {
            if (FileName == IntPtr.Zero)
                return 5;
            else
            {
                int i = 0;
                while (File.Exists("as." + Convert.ToByte(i)))
                    i++;
                Marshal.WriteByte(FileName, 0, Convert.ToByte('a'));
                Marshal.WriteByte(FileName, 1, Convert.ToByte('s'));
                Marshal.WriteByte(FileName, 2, Convert.ToByte('.'));
                Marshal.WriteByte(FileName, 3, Convert.ToByte(i));
            }
            return 5;
        }
        /**
                * Callback para realizar alguna acción mientras se carga el audio.
                * 
                * Si se devuelve 0 continua el Indexing, si es otro valor se para.
                * Cada X muestras cargadas llama a esta función, es valida para crear una barra de procentaje.
                * 
                * Current el actual.
                * Total es el tamaño total del audio.
                * ICPrivate es un puntero enviado por Do_Indexing
                * */
        public int IndexCBI(long Current, long Total, IntPtr ICPrivate)
        {
            lock (this)
            {
                int ret = 0;
//#if DEBUG
//                Console.WriteLine(Current + " of " + Total); //You can do something with this
//#endif
                if (this.PrivateCallBack != 0)
                    ret = 1;
                if (UpdateIndexProgress != null)
                    UpdateIndexProgress(this, new IndexingProgressChangeEventArgs(Current, Total));

                if (IndexingCompleted != null)
                    if (Current == Total) // aquí no llega nunca (deberúa pero no), hay que buscar otro lugar para poner el evento
                        IndexingCompleted(this);

                return ret;
            }
        }
        #endregion
    }
}