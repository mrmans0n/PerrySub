using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace PerrySubUpdater
{
    class VersionInfo
    {
        private string filename;

        public string FileName
        {
            get { return filename; }
            set { filename = value; }
        }
        private long crc32;

        public long CRC32
        {
            get { return crc32; }
            set { crc32 = value; }
        }
        private string version;

        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        public VersionInfo(string f, string crc, string v)
        {
            FileName = f;
            Version = v;
            CRC32 = long.Parse(crc,NumberStyles.HexNumber);
        }
    }
}
