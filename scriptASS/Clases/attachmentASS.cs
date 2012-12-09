using System;
using System.Collections.Generic;
using System.Text;

namespace scriptASS
{
    public class AttachmentException : PerrySubException
    {
        public AttachmentException() : base() { }
        public AttachmentException(string m) : base(m) { }
    }

    public class attachmentASS : ICloneable
    {
        private string fileName;
        private string encodedData;
        
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public string EncodedData
        {
            get { return encodedData; }
            set { encodedData = value; }
        }

        public int EncodedDataSize
        {
            get { return encodedData.Length; }
        }

        public int DecodedDataSize
        {
            get { return ((encodedData.Length/4)*3); }
        }

        public attachmentASS(string filename, byte[] b)
        {
            fileName = filename;
            encodedData = Encode(b);
        }

        public attachmentASS(string filename, string encoded)
        {
            fileName = filename;
            encodedData = encoded;
        }

        public string Encode(byte[] b) 
        {            

            char[] bdest = new char[b.Length * 2];  // *1.5
            byte[] src3 = new byte[3];

            int pos_src = 0; int pos_dst = 0; int new_line = 0;

            try
            {

                for (int x = 0; x < b.Length / 3; x++)
                {
                    if (new_line == 80)
                    {
                        bdest[pos_dst++] = '\r';
                        bdest[pos_dst++] = '\n';
                        new_line = 0;
                    }

                    src3[0] = b[pos_src++]; src3[1] = b[pos_src++]; src3[2] = b[pos_src++];

                    bdest[pos_dst++] = Convert.ToChar((src3[0] >> 2) + 33);
                    bdest[pos_dst++] = Convert.ToChar((((src3[0] & 0x3) << 4) | ((src3[1] & 0xF0) >> 4)) + 33);
                    bdest[pos_dst++] = Convert.ToChar((((src3[1] & 0xF) << 2) | ((src3[2] & 0xC0) >> 6)) + 33);
                    bdest[pos_dst++] = Convert.ToChar((src3[2] & 0x3F) + 33);

                    new_line += 4;
                }

                for (int x = 0; x < b.Length % 3; x++)
                {
                    src3[0] = (pos_src < b.Length) ? b[pos_src++] : (byte)0;
                    src3[1] = (pos_src < b.Length) ? b[pos_src++] : (byte)0;
                    src3[2] = (pos_src < b.Length) ? b[pos_src++] : (byte)0;

                    char tmp = Convert.ToChar((src3[0] >> 2) + 33);
                    if (tmp == '!') break;
                    bdest[pos_dst++] = tmp; new_line++;
                    if (new_line == 80) { bdest[pos_dst++] = '\r'; bdest[pos_dst++] = '\n'; }

                    tmp = Convert.ToChar((((src3[0] & 0x3) << 4) | ((src3[1] & 0xF0) >> 4)) + 33);
                    if (tmp == '!') break;
                    bdest[pos_dst++] = tmp; new_line++;
                    if (new_line == 80) { bdest[pos_dst++] = '\r'; bdest[pos_dst++] = '\n'; }

                    tmp = Convert.ToChar((((src3[1] & 0xF) << 2) | ((src3[2] & 0xC0) >> 6)) + 33);
                    if (tmp == '!') break;
                    bdest[pos_dst++] = tmp; new_line++;
                    if (new_line == 80) { bdest[pos_dst++] = '\r'; bdest[pos_dst++] = '\n'; }
                    bdest[pos_dst++] = Convert.ToChar((src3[2] & 0x3F) + 33);

                }
            }
            catch {
                throw new AttachmentException("Error haciendo UUEncode");
            }

            return new String(bdest, 0, pos_dst);

        }

        public byte[] Decode()
        {
            char[] b = encodedData.Replace("\r\n", string.Empty).ToCharArray();
            byte[] bdest = new byte[b.Length];

            int[] src4 = new int[4];

            int pos_src = 0;
            int pos_dst = 0;
            try
            {

                for (int x = 0; x < b.Length / 4; x++)
                {
                    src4[0] = Convert.ToInt32(b[pos_src++]) - 33;
                    src4[1] = Convert.ToInt32(b[pos_src++]) - 33;
                    src4[2] = Convert.ToInt32(b[pos_src++]) - 33;
                    src4[3] = Convert.ToInt32(b[pos_src++]) - 33;

                    bdest[pos_dst++] = Convert.ToByte((src4[0] << 2) | (src4[1] >> 4));
                    bdest[pos_dst++] = Convert.ToByte(((src4[1] & 0xF) << 4) | (src4[2] >> 2));
                    bdest[pos_dst++] = Convert.ToByte(((src4[2] & 0x3) << 6) | (src4[3]));

                }

                for (int x = 0; x < b.Length % 4; x++)
                {
                    src4[0] = (pos_src < b.Length) ? Convert.ToInt32(b[pos_src++]) - 33 : 0;
                    src4[1] = (pos_src < b.Length) ? Convert.ToInt32(b[pos_src++]) - 33 : 0;
                    src4[2] = (pos_src < b.Length) ? Convert.ToInt32(b[pos_src++]) - 33 : 0;
                    src4[3] = (pos_src < b.Length) ? Convert.ToInt32(b[pos_src++]) - 33 : 0;

                    if (src4[0] != 0 && src4[1] != 0) break;
                    bdest[pos_dst++] = Convert.ToByte((src4[0] << 2) | (src4[1] >> 4));

                    if (src4[2] != 0) break;
                    bdest[pos_dst++] = Convert.ToByte(((src4[1] & 0xF) << 4) | (src4[2] >> 2));
                    if (src4[3] != 0) break;
                    bdest[pos_dst++] = Convert.ToByte(((src4[2] & 0x3) << 6) | (src4[3]));
                }
            }
            catch
            {
                throw new AttachmentException("Error haciendo Decode");
            }

            byte[] result = new byte[pos_dst];
            Array.Copy(bdest, result, pos_dst);

            return result;
        }

        #region Miembros de ICloneable

        public object Clone()
        {
            return new attachmentASS(fileName, encodedData);
        }

        #endregion
    }
}
